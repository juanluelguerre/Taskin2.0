import { ChangeDetectionStrategy, Component, ViewEncapsulation, OnInit, OnDestroy, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PomodoroStore } from '../../shared/stores/pomodoro.store';
import { PomodoroType, PomodoroStatus } from '../../shared/types/pomodoro.types';
import { FormatTimePipe } from '@shared/pipes/format-time.pipe';
import { SessionTypeDisplayPipe } from '@shared/pipes/session-type-display.pipe';

@Component({
  selector: 'app-pomodoros',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    FormatTimePipe,
    SessionTypeDisplayPipe
  ],
  templateUrl: './pomodoros.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [PomodoroStore]
})
export class PomodorosComponent implements OnInit, OnDestroy {
  private readonly store = inject(PomodoroStore)

  // Expose store signals for template
  readonly timer = this.store.timer
  readonly settings = this.store.settings
  readonly todayPomodoros = this.store.todayPomodoros
  readonly pomodoroStatistics = this.store.pomodoroStatistics
  readonly timerProgress = this.store.timerProgress
  readonly formattedTimeRemaining = this.store.formattedTimeRemaining
  readonly isTimerActive = this.store.isTimerActive
  readonly loading = this.store.loading
  readonly error = this.store.error
  
  // Circle properties for progress indicator
  circumference = 2 * Math.PI * 45 // radius = 45
  
  // Computed properties
  readonly completedWorkPomodoros = computed(() => {
    return this.todayPomodoros().filter(p => 
      p.status === PomodoroStatus.Completed && p.type === PomodoroType.Work
    ).length
  })

  readonly inProgressPomodoro = computed(() => {
    return this.todayPomodoros().find(p => p.status === PomodoroStatus.InProgress) || null
  })

  readonly currentSessionType = computed(() => {
    return this.timer().currentType
  })

  readonly strokeDashoffset = computed(() => {
    const progress = this.timerProgress()
    return this.circumference * (1 - (progress / 100))
  })

  readonly totalFocusTime = computed(() => {
    const completedWorkPomodoros = this.todayPomodoros().filter(p => 
      p.status === PomodoroStatus.Completed && p.type === PomodoroType.Work
    )
    const totalMinutes = completedWorkPomodoros.reduce((sum, p) => sum + (p.actualDuration || p.plannedDuration || 25), 0)
    const hours = Math.floor(totalMinutes / 60)
    const minutes = totalMinutes % 60
    return hours > 0 ? `${hours}h ${minutes}m` : `${minutes}m`
  })

  readonly averageSessionLength = computed(() => {
    const completedPomodoros = this.todayPomodoros().filter(p => p.status === PomodoroStatus.Completed)
    if (completedPomodoros.length === 0) return '0m'
    const totalMinutes = completedPomodoros.reduce((sum, p) => sum + (p.actualDuration || p.plannedDuration || 25), 0)
    const average = Math.round(totalMinutes / completedPomodoros.length)
    return `${average}m`
  })

  readonly productivityScore = computed(() => {
    const completedPomodoros = this.todayPomodoros().filter(p => p.status === PomodoroStatus.Completed)
    const plannedPomodoros = this.todayPomodoros().length
    if (plannedPomodoros === 0) return 100
    return Math.round((completedPomodoros.length / plannedPomodoros) * 100)
  })

  readonly todaySessions = computed(() => {
    const pomodoros = this.todayPomodoros()
    const sessions: { type: 'work' | 'break' | 'upcoming' }[] = []
    
    // Add completed pomodoros
    pomodoros.forEach(p => {
      if (p.status === PomodoroStatus.Completed) {
        sessions.push({
          type: p.type === PomodoroType.Work ? 'work' : 'break'
        })
      }
    })
    
    // Add upcoming sessions up to 8 total
    while (sessions.length < 8) {
      sessions.push({ type: 'upcoming' })
    }
    
    return sessions
  })

  // Expose enums for template
  readonly PomodoroType = PomodoroType
  readonly PomodoroStatus = PomodoroStatus

  ngOnInit() {
    // Load initial data using available methods
    this.store.refreshPomodoros()
  }

  ngOnDestroy() {
    // Timer cleanup is handled in the store
  }

  toggleTimer(): void {
    if (this.timer().isRunning) {
      this.store.pauseTimer()
    } else if (this.timer().isPaused) {
      this.store.resumeTimer()
    } else {
      this.store.startTimer()
    }
  }

  resetTimer(): void {
    this.store.resetTimer()
  }

  skipSession(): void {
    this.store.completeCurrentTimer()
  }

  adjustWorkTime(delta: number): void {
    if (this.timer().isRunning) return
    
    const currentSettings = this.settings()
    if (!currentSettings) return
    
    const newMinutes = Math.max(1, Math.min(60, currentSettings.workDuration + delta))
    this.store.updateSettings({
      ...currentSettings,
      workDuration: newMinutes
    })
  }

  adjustBreakTime(delta: number): void {
    if (this.timer().isRunning) return
    
    const currentSettings = this.settings()
    if (!currentSettings) return
    
    const newMinutes = Math.max(1, Math.min(30, currentSettings.shortBreakDuration + delta))
    this.store.updateSettings({
      ...currentSettings,
      shortBreakDuration: newMinutes
    })
  }

  startWorkPomodoro(): void {
    // Create a new work pomodoro and start it
    this.store.createPomodoro({
      taskId: 'temp-task-id', // This should come from selected task
      type: PomodoroType.Work,
      plannedDuration: this.settings()?.workDuration || 25,
      notes: undefined
    })
  }


  clearError(): void {
    this.store.clearError()
  }
}
