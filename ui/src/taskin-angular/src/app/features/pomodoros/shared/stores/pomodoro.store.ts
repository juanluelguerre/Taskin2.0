import { Injectable, inject, computed } from '@angular/core'
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'
import { pipe, switchMap, tap } from 'rxjs'
import { 
  Pomodoro,
  PomodoroStatus,
  PomodoroType,
  PomodoroStats,
  PomodoroTimer,
  PomodoroSettings,
  CreatePomodoroRequest,
  UpdatePomodoroRequest
} from '../types/pomodoro.types'
import { PomodoroService } from '../services/pomodoro.service'

type PomodoroState = {
  pomodoros: Pomodoro[]
  selectedPomodoro: Pomodoro | null
  activePomodoro: Pomodoro | null
  timer: PomodoroTimer
  settings: PomodoroSettings | null
  loading: boolean
  saving: boolean
  deleting: boolean
  error: string | null
}

const initialTimer: PomodoroTimer = {
  isRunning: false,
  isPaused: false,
  currentType: PomodoroType.Work,
  timeRemaining: 25 * 60,
  totalTime: 25 * 60,
  currentPomodoroId: undefined,
  currentSessionId: undefined
}

const initialSettings: PomodoroSettings = {
  workDuration: 25,
  shortBreakDuration: 5,
  longBreakDuration: 15,
  pomodorosUntilLongBreak: 4,
  autoStartBreaks: false,
  autoStartPomodoros: false,
  notificationsEnabled: true,
  soundEnabled: true,
  tickingSoundEnabled: false
}

export const PomodoroStore = signalStore(
  { providedIn: 'root' },
  withState<PomodoroState>({
    pomodoros: [],
    selectedPomodoro: null,
    activePomodoro: null,
    timer: initialTimer,
    settings: initialSettings,
    loading: false,
    saving: false,
    deleting: false,
    error: null
  }),
  withComputed((state) => ({
    todayPomodoros: computed(() => {
      const today = new Date().toDateString()
      return state.pomodoros().filter(p => 
        p.startTime && new Date(p.startTime).toDateString() === today
      )
    }),
    pomodoroStatistics: computed(() => {
      const today = new Date().toDateString()
      const todayPomodoros = state.pomodoros().filter(p => 
        p.startTime && new Date(p.startTime).toDateString() === today
      )
      const completedToday = todayPomodoros.filter(p => p.status === PomodoroStatus.Completed)
      
      return {
        totalPomodoros: state.pomodoros().length,
        completedPomodoros: state.pomodoros().filter(p => p.status === PomodoroStatus.Completed).length,
        totalFocusTime: completedToday.reduce((sum, p) => sum + (p.actualDuration || p.plannedDuration), 0),
        averageSessionLength: completedToday.length > 0 
          ? completedToday.reduce((sum, p) => sum + (p.actualDuration || p.plannedDuration), 0) / completedToday.length
          : 0,
        todayPomodoros: todayPomodoros.length,
        thisWeekPomodoros: 0,
        productivityScore: todayPomodoros.length > 0 
          ? Math.round((completedToday.length / todayPomodoros.length) * 100) 
          : 100,
        mostProductiveHour: 9,
        averageInterruptions: 0
      } as PomodoroStats
    }),
    timerProgress: computed(() => {
      const timer = state.timer()
      if (timer.totalTime === 0) return 0
      return ((timer.totalTime - timer.timeRemaining) / timer.totalTime) * 100
    }),
    formattedTimeRemaining: computed(() => {
      const seconds = state.timer().timeRemaining
      const minutes = Math.floor(seconds / 60)
      const remainingSeconds = seconds % 60
      return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`
    }),
    isTimerActive: computed(() => state.timer().isRunning)
  })),
  withMethods((store, pomodoroService = inject(PomodoroService)) => ({
    // Timer controls
    startTimer: () => {
      const currentTimer = store.timer()
      patchState(store, {
        timer: {
          ...currentTimer,
          isRunning: true,
          isPaused: false
        }
      })
    },

    pauseTimer: () => {
      const currentTimer = store.timer()
      patchState(store, {
        timer: {
          ...currentTimer,
          isRunning: false,
          isPaused: true
        }
      })
    },

    resumeTimer: () => {
      const currentTimer = store.timer()
      patchState(store, {
        timer: {
          ...currentTimer,
          isRunning: true,
          isPaused: false
        }
      })
    },

    resetTimer: () => {
      const settings = store.settings()
      const duration = settings?.workDuration || 25
      patchState(store, {
        timer: {
          ...store.timer(),
          isRunning: false,
          isPaused: false,
          timeRemaining: duration * 60,
          totalTime: duration * 60
        }
      })
    },

    completeCurrentTimer: () => {
      const currentTimer = store.timer()
      const nextType = currentTimer.currentType === PomodoroType.Work 
        ? PomodoroType.ShortBreak 
        : PomodoroType.Work
      
      const settings = store.settings()
      const nextDuration = nextType === PomodoroType.Work 
        ? (settings?.workDuration || 25)
        : (settings?.shortBreakDuration || 5)

      patchState(store, {
        timer: {
          ...currentTimer,
          isRunning: false,
          isPaused: false,
          currentType: nextType,
          timeRemaining: nextDuration * 60,
          totalTime: nextDuration * 60
        }
      })
    },

    updateSettings: (newSettings: PomodoroSettings) => {
      patchState(store, { settings: newSettings })
    },

    // Data operations
    createPomodoro: rxMethod<CreatePomodoroRequest>(
      pipe(
        switchMap((request) => {
          patchState(store, { saving: true, error: null })
          return pomodoroService.create(request).pipe(
            tap({
              next: (pomodoro) => {
                patchState(store, {
                  pomodoros: [...store.pomodoros(), pomodoro],
                  saving: false
                })
              },
              error: (error) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to create pomodoro'
                })
                console.error('Create pomodoro error:', error)
              }
            })
          )
        })
      )
    ),

    updatePomodoro: rxMethod<UpdatePomodoroRequest>(
      pipe(
        switchMap((request) => {
          patchState(store, { saving: true, error: null })
          return pomodoroService.update(request.id || '', request).pipe(
            tap({
              next: (updatedPomodoro) => {
                const updatedPomodoros = store.pomodoros().map((p: Pomodoro) => 
                  p.id === updatedPomodoro.id ? updatedPomodoro : p
                )
                patchState(store, {
                  pomodoros: updatedPomodoros,
                  selectedPomodoro: store.selectedPomodoro()?.id === updatedPomodoro.id ? updatedPomodoro : store.selectedPomodoro(),
                  saving: false
                })
              },
              error: (error) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to update pomodoro'
                })
                console.error('Update pomodoro error:', error)
              }
            })
          )
        })
      )
    ),

    deletePomodoro: rxMethod<string>(
      pipe(
        switchMap((id) => {
          patchState(store, { deleting: true, error: null })
          return pomodoroService.delete(id).pipe(
            tap({
              next: () => {
                const filteredPomodoros = store.pomodoros().filter((p: Pomodoro) => p.id !== id)
                patchState(store, {
                  pomodoros: filteredPomodoros,
                  selectedPomodoro: store.selectedPomodoro()?.id === id ? null : store.selectedPomodoro(),
                  deleting: false
                })
              },
              error: (error) => {
                patchState(store, {
                  deleting: false,
                  error: 'Failed to delete pomodoro'
                })
                console.error('Delete pomodoro error:', error)
              }
            })
          )
        })
      )
    ),

    // Utility methods
    clearError: () => patchState(store, { error: null }),
    clearSelection: () => patchState(store, { selectedPomodoro: null }),
    selectPomodoro: (pomodoro: Pomodoro | null) => patchState(store, { selectedPomodoro: pomodoro }),
    refreshPomodoros: () => {
      patchState(store, { loading: true, error: null })
      patchState(store, { loading: false })
    }
  }))
)