import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal, OnDestroy, OnInit, inject, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslocoModule } from '@jsverse/transloco';
import { environment } from '@env/environment';
import { NotificationService } from '@core/services/notification.service';
import { CanComponentDeactivate } from '@core/guards/can-deactivate.guard';
import { PomodoroService, PomodoroDto } from '../../services/pomodoro.service';

interface Session {
  type: 'work' | 'break' | 'upcoming';
  duration: number;
  completed: boolean;
}

@Component({
  selector: 'app-pomodoros',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTooltipModule,
    TranslocoModule,
  ],
  templateUrl: './pomodoros.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PomodorosComponent implements OnInit, OnDestroy, CanComponentDeactivate {
  private readonly pomodoroService = inject(PomodoroService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly notificationService = inject(NotificationService);

  // Linked task (from query params)
  linkedTaskId = signal<string | null>(null);
  linkedTaskName = signal<string | null>(null);

  currentSession = signal<'work' | 'break'>('work');
  workMinutes = signal(25);
  breakMinutes = signal(5);
  timeLeft = signal(25 * 60);
  isRunning = signal(false);
  completedPomodoros = signal(0);

  // Timer properties
  private timerInterval: any;
  private totalTime = signal(25 * 60);

  // Circle properties for progress indicator
  circumference = 2 * Math.PI * 45;

  // Session tracking
  todaySessions = signal<Session[]>([]);

  // Statistics
  totalFocusTime = computed(() => {
    const minutes = this.completedPomodoros() * this.workMinutes();
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
  });

  averageSessionLength = signal('25m');
  productivityScore = signal(0);

  ngOnInit(): void {
    this.loadTodaySessions();

    const taskId = this.route.snapshot.queryParamMap.get('taskId');
    const autoStart = this.route.snapshot.queryParamMap.get('autoStart');

    if (taskId) {
      this.linkedTaskId.set(taskId);
      this.http
        .get<{ id: string; title: string }>(`${environment.apiUrl}/api/Tasks/${taskId}`)
        .subscribe(task => this.linkedTaskName.set(task.title));
    }

    if (autoStart === 'true') {
      this.toggleTimer();
    }
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  canDeactivate(): boolean {
    return !this.isRunning();
  }

  private loadTodaySessions(): void {
    this.pomodoroService.getToday().subscribe({
      next: (pomodoros: PomodoroDto[]) => {
        const sessions: Session[] = pomodoros.map(p => ({
          type: 'work' as const,
          duration: p.durationInMinutes,
          completed: true
        }));
        this.todaySessions.set(sessions);
        this.completedPomodoros.set(pomodoros.length);

        if (pomodoros.length > 0) {
          const avgLen = Math.round(pomodoros.reduce((sum, p) => sum + p.durationInMinutes, 0) / pomodoros.length);
          this.averageSessionLength.set(`${avgLen}m`);
          this.productivityScore.set(Math.min(100, Math.round((pomodoros.length / 8) * 100)));
        }
      },
      error: (err) => {
        console.error('Failed to load today sessions:', err);
      }
    });
  }

  get strokeDashoffset(): number {
    const progress = (this.totalTime() - this.timeLeft()) / this.totalTime();
    return this.circumference * (1 - progress);
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
  }

  toggleTimer(): void {
    if (this.isRunning()) {
      this.pauseTimer();
    } else {
      this.startTimer();
    }
  }

  private startTimer(): void {
    this.isRunning.set(true);
    this.timerInterval = setInterval(() => {
      const currentTime = this.timeLeft();
      if (currentTime > 0) {
        this.timeLeft.set(currentTime - 1);
      } else {
        this.completeSession();
      }
    }, 1000);
  }

  private pauseTimer(): void {
    this.isRunning.set(false);
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  private completeSession(): void {
    this.pauseTimer();

    if (this.currentSession() === 'work') {
      this.completedPomodoros.update(count => count + 1);

      // Add to today's sessions
      this.todaySessions.update(sessions => [
        ...sessions,
        { type: 'work', duration: this.workMinutes(), completed: true }
      ]);

      // Save to backend if linked to a task
      if (this.linkedTaskId()) {
        this.pomodoroService
          .create({
            taskId: this.linkedTaskId()!,
            startTime: new Date(Date.now() - this.workMinutes() * 60 * 1000).toISOString(),
            durationInMinutes: this.workMinutes(),
          })
          .subscribe({
            next: () =>
              this.notificationService.notifySuccess('pomodoros.pomodoroSaved'),
            error: () =>
              this.notificationService.notifyError('messages.error'),
          });
      }

      this.currentSession.set('break');
      this.timeLeft.set(this.breakMinutes() * 60);
      this.totalTime.set(this.breakMinutes() * 60);
    } else {
      this.currentSession.set('work');
      this.timeLeft.set(this.workMinutes() * 60);
      this.totalTime.set(this.workMinutes() * 60);
    }

    this.playNotificationSound();
  }

  resetTimer(): void {
    this.pauseTimer();
    const minutes = this.currentSession() === 'work' ? this.workMinutes() : this.breakMinutes();
    this.timeLeft.set(minutes * 60);
    this.totalTime.set(minutes * 60);
  }

  skipSession(): void {
    this.pauseTimer();
    this.completeSession();
  }

  adjustWorkTime(delta: number): void {
    if (this.isRunning()) return;

    const newMinutes = Math.max(1, Math.min(60, this.workMinutes() + delta));
    this.workMinutes.set(newMinutes);

    if (this.currentSession() === 'work') {
      this.timeLeft.set(newMinutes * 60);
      this.totalTime.set(newMinutes * 60);
    }
  }

  adjustBreakTime(delta: number): void {
    if (this.isRunning()) return;

    const newMinutes = Math.max(1, Math.min(30, this.breakMinutes() + delta));
    this.breakMinutes.set(newMinutes);

    if (this.currentSession() === 'break') {
      this.timeLeft.set(newMinutes * 60);
      this.totalTime.set(newMinutes * 60);
    }
  }

  navigateToTask(): void {
    const taskId = this.linkedTaskId();
    if (taskId) {
      this.router.navigate(['/tasks', taskId]);
    }
  }

  private playNotificationSound(): void {
    if ('AudioContext' in window || 'webkitAudioContext' in window) {
      const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();

      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);

      oscillator.frequency.value = 800;
      oscillator.type = 'sine';

      gainNode.gain.setValueAtTime(0, audioContext.currentTime);
      gainNode.gain.linearRampToValueAtTime(0.1, audioContext.currentTime + 0.1);
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

      oscillator.start(audioContext.currentTime);
      oscillator.stop(audioContext.currentTime + 0.5);
    }
  }
}
