export enum PomodoroStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Paused = 'Paused'
}

export enum PomodoroType {
  Work = 'Work',
  ShortBreak = 'ShortBreak',
  LongBreak = 'LongBreak'
}

export interface Pomodoro {
  id: string
  taskId: string
  taskTitle?: string
  type: PomodoroType
  status: PomodoroStatus
  plannedDuration: number // in minutes
  actualDuration?: number // in minutes
  startTime?: Date
  endTime?: Date
  pausedTime?: number // total paused time in seconds
  notes?: string
  interruptions?: number
  createdAt: Date
  updatedAt: Date
}

export interface PomodoroSession {
  id: string
  pomodoroId: string
  sessionNumber: number
  startTime: Date
  endTime?: Date
  duration: number // in seconds
  isActive: boolean
}

export interface PomodoroStats {
  totalPomodoros: number
  completedPomodoros: number
  totalFocusTime: number // in minutes
  averageSessionLength: number // in minutes
  todayPomodoros: number
  thisWeekPomodoros: number
  productivityScore: number
  mostProductiveHour: number
  averageInterruptions: number
}

export interface PomodoroTimer {
  isRunning: boolean
  isPaused: boolean
  currentType: PomodoroType
  timeRemaining: number // in seconds
  totalTime: number // in seconds
  currentPomodoroId?: string
  currentSessionId?: string
}

export interface PomodoroSettings {
  workDuration: number // in minutes
  shortBreakDuration: number // in minutes
  longBreakDuration: number // in minutes
  pomodorosUntilLongBreak: number
  autoStartBreaks: boolean
  autoStartPomodoros: boolean
  soundEnabled: boolean
  notificationsEnabled: boolean
  tickingSoundEnabled: boolean
}

export interface CreatePomodoroRequest {
  taskId: string
  type: PomodoroType
  plannedDuration: number
  notes?: string
}

export interface UpdatePomodoroRequest {
  id?: string
  status?: PomodoroStatus
  actualDuration?: number
  endTime?: Date
  notes?: string
  interruptions?: number
}

export interface PomodoroListResponse {
  items: Pomodoro[]
  totalCount: number
  currentPage: number
  pageSize: number
}

export interface PomodoroFilters {
  taskId?: string
  type?: PomodoroType
  status?: PomodoroStatus
  dateFrom?: Date
  dateTo?: Date
}

export interface PomodoroSearchRequest {
  query?: string
  filters?: PomodoroFilters
  sortBy?: keyof Pomodoro
  sortDirection?: 'asc' | 'desc'
  page?: number
  size?: number
}

// View Models for computed properties
export interface PomodoroViewModel extends Pomodoro {
  statusColor: string
  typeColor: string
  completionPercentage: number
  isOverdue: boolean
  formattedDuration: string
  efficiency: number // actual vs planned duration
}

export interface PomodoroStatsViewModel {
  total: number
  completed: number
  inProgress: number
  cancelled: number
  todayCompleted: number
  weekCompleted: number
  totalFocusTime: string // formatted time
  averageEfficiency: number
  streakCount: number
}

export interface DailyPomodoroSummary {
  date: Date
  pomodorosCompleted: number
  totalFocusTime: number
  efficiency: number
  interruptions: number
}