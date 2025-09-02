// Base entity interface
export interface BaseEntity {
  id: string
  createdAt: Date
  updatedAt: Date
  createdBy?: string
  updatedBy?: string
}

// Enums - matching backend .NET enums
export enum TaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3,
}


// Main entity
export interface Task extends BaseEntity {
  title: string
  description?: string
  status: TaskStatus
  priority: TaskPriority
  projectId: string
  projectName?: string
  assigneeId?: string
  assigneeName?: string
  dueDate?: Date
  estimatedPomodoros?: number
  completedPomodoros?: number
  tags: string[]
  isCompleted: boolean
  completedAt?: Date
}

// Related interfaces
export interface TaskStatistics {
  totalPomodoros: number
  completedPomodoros: number
  totalTimeSpent: number // in minutes
  averagePomodoroLength: number
  productivity: number // percentage
}

// Request/Response types
export interface CreateTaskRequest {
  title: string
  description?: string
  status: TaskStatus
  priority: TaskPriority
  projectId: string
  assigneeId?: string
  dueDate?: Date
  estimatedPomodoros?: number
  tags?: string[]
}

export interface UpdateTaskRequest extends Partial<CreateTaskRequest> {
  id: string
  isCompleted?: boolean
  completedAt?: Date
}

export interface TaskListResponse {
  items: Task[]
  totalCount: number
  page: number
  size: number
}

export interface TaskStats {
  totalTasks: number
  pendingTasks: number
  inProgressTasks: number
  completedTasks: number
  overdueTasks: number
  tasksCompletedThisWeek: number
  averageCompletionTime: number
  productivityScore: number
}

// Filter and search types
export interface TaskFilters {
  status?: TaskStatus
  priority?: TaskPriority
  projectId?: string
  assigneeId?: string
  tags?: string[]
  dueDateRange?: {
    start: Date
    end: Date
  }
  isOverdue?: boolean
  isCompleted?: boolean
}

export interface TaskSearchRequest {
  query: string
  filters?: TaskFilters
  sortBy?: keyof Task
  sortDirection?: 'asc' | 'desc'
  page: number
  size: number
}

// Form types
export interface TaskFormData {
  title: string
  description: string
  status: TaskStatus
  priority: TaskPriority
  projectId: string
  assigneeId: string
  dueDate: string
  estimatedPomodoros: number
  tags: string[]
}

// Validation schemas
export const TASK_VALIDATION = {
  title: {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  description: {
    maxLength: 1000,
  },
  estimatedPomodoros: {
    min: 1,
    max: 50,
  },
  tags: {
    maxItems: 10,
    maxLength: 30, // per tag
  },
} as const

// Type guards
export function isTask(obj: any): obj is Task {
  return obj && typeof obj.id === 'string' && typeof obj.title === 'string'
}

export function isCreateTaskRequest(obj: any): obj is CreateTaskRequest {
  return obj && typeof obj.title === 'string' && Object.values(TaskStatus).includes(obj.status)
}

// Utility types
export type TaskKeys = keyof Task
export type TaskSortableFields = 'title' | 'createdAt' | 'updatedAt' | 'status' | 'priority' | 'dueDate'
export type TaskTableColumn = {
  key: TaskKeys
  label: string
  sortable: boolean
  width?: string
}

// View models for UI
export interface TaskViewModel extends Task {
  projectName: string
  assigneeName?: string
  isOverdue: boolean
  daysUntilDue?: number
  completionPercentage: number
  statusColor: string
  priorityColor: string
}

export interface TaskStatsViewModel {
  total: number
  pending: number
  inProgress: number
  completed: number
  overdue: number
  completionRate: number
  productivity: number
}