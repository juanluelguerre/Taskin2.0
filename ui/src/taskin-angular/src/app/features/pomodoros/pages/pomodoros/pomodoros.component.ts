import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

interface Session {
  type: 'work' | 'break' | 'upcoming';
  duration: number;
  completed: boolean;
}

@Component({
  selector: 'app-pomodoros',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './pomodoros.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PomodorosComponent implements OnDestroy {
  currentSession = signal<'work' | 'break'>('work');
  workMinutes = signal(25);
  breakMinutes = signal(5);
  timeLeft = signal(25 * 60); // 25 minutes in seconds
  isRunning = signal(false);
  completedPomodoros = signal(6);
  
  // Timer properties
  private timerInterval: any;
  private totalTime = signal(25 * 60);
  
  // Circle properties for progress indicator
  circumference = 2 * Math.PI * 45; // radius = 45
  
  // Session tracking
  todaySessions = signal<Session[]>([
    { type: 'work', duration: 25, completed: true },
    { type: 'break', duration: 5, completed: true },
    { type: 'work', duration: 25, completed: true },
    { type: 'break', duration: 5, completed: true },
    { type: 'work', duration: 25, completed: true },
    { type: 'break', duration: 5, completed: true },
    { type: 'work', duration: 25, completed: false },
    { type: 'upcoming', duration: 5, completed: false },
  ]);

  // Statistics
  totalFocusTime = signal('2h 30m');
  averageSessionLength = signal('24m');
  productivityScore = signal(87);

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
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
      this.currentSession.set('break');
      this.timeLeft.set(this.breakMinutes() * 60);
      this.totalTime.set(this.breakMinutes() * 60);
    } else {
      this.currentSession.set('work');
      this.timeLeft.set(this.workMinutes() * 60);
      this.totalTime.set(this.workMinutes() * 60);
    }
    
    // Show notification or play sound here
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

  private playNotificationSound(): void {
    // Create a simple notification sound
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
