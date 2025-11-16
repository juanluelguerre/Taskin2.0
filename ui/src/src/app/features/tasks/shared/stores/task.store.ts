import { Injectable, inject, computed } from '@angular/core'
import { Router } from '@angular/router'
// Use custom tapResponse since @ngrx/operators is not available
const tapResponse = <T>(callbacks: {
  next: (value: T) => void,
  error: (error: any) => void
}) => {
  return (source: any) => source.pipe(
    tap({
      next: callbacks.next,
      error: callbacks.error
    })
  )
}
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'
import { pipe, switchMap, exhaustMap, debounceTime, distinctUntilChanged, tap } from 'rxjs'
import { 
  Task, 
  TaskStatus, 
  TaskPriority,
  TaskFilters, 
  TaskStats, 
  CreateTaskRequest, 
  UpdateTaskRequest,
  TaskSearchRequest,
  TaskListResponse,
  TaskViewModel,
  TaskStatsViewModel
} from '../types/task.types'
import { TaskService } from '../services/task.service'

type TaskState = {
  // Data
  tasks: Task[]
  selectedTask: Task | null
  
  // Loading states
  loading: boolean
  saving: boolean
  deleting: boolean
  
  // Pagination
  currentPage: number
  pageSize: number
  totalCount: number
  
  // Filters and search
  searchTerm: string
  filters: TaskFilters
  
  // Stats
  stats: TaskStats | null
  
  // UI state
  viewMode: 'list' | 'grid' | 'kanban'
  sortBy: keyof Task
  sortDirection: 'asc' | 'desc'
  
  // Error handling
  error: string | null
}

const initialState: TaskState = {
  tasks: [],
  selectedTask: null,
  loading: false,
  saving: false,
  deleting: false,
  currentPage: 1,
  pageSize: 25,
  totalCount: 0,
  searchTerm: '',
  filters: {
    status: undefined,
    priority: undefined,
    projectId: undefined,
    assigneeId: undefined,
    tags: undefined,
    isOverdue: undefined,
    isCompleted: undefined
  },
  stats: null,
  viewMode: 'list',
  sortBy: 'createdAt',
  sortDirection: 'desc',
  error: null,
}

@Injectable()
export class TaskStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    // Task view models with calculated properties
    taskViewModels: computed((): TaskViewModel[] => {
      // Apply filters directly here instead of using filteredTasks
      let filtered = store.tasks()
      const currentFilters = store.filters()
      const search = store.searchTerm().toLowerCase().trim()
      
      // Apply search
      if (search) {
        filtered = filtered.filter(task => 
          task.title.toLowerCase().includes(search) ||
          task.description?.toLowerCase().includes(search) ||
          task.tags.some(tag => tag.toLowerCase().includes(search))
        )
      }
      
      // Apply filters
      if (currentFilters.status) {
        filtered = filtered.filter(task => task.status === currentFilters.status)
      }
      
      if (currentFilters.priority) {
        filtered = filtered.filter(task => task.priority === currentFilters.priority)
      }
      
      if (currentFilters.projectId) {
        filtered = filtered.filter(task => task.projectId === currentFilters.projectId)
      }
      
      if (currentFilters.assigneeId) {
        filtered = filtered.filter(task => task.assigneeId === currentFilters.assigneeId)
      }
      
      if (currentFilters.isCompleted !== undefined) {
        filtered = filtered.filter(task => task.isCompleted === currentFilters.isCompleted)
      }
      
      if (currentFilters.isOverdue) {
        const now = new Date()
        filtered = filtered.filter(task => 
          task.dueDate && new Date(task.dueDate) < now && !task.isCompleted
        )
      }
      
      if (currentFilters.tags && currentFilters.tags.length > 0) {
        filtered = filtered.filter(task => 
          currentFilters.tags!.some(tag => task.tags.includes(tag))
        )
      }
      
      const now = new Date()
      
      return filtered.map((task: Task) => ({
        ...task,
        projectName: task.projectName || 'Unknown Project',
        assigneeName: task.assigneeName || undefined,
        isOverdue: task.dueDate ? new Date(task.dueDate) < now && !task.isCompleted : false,
        daysUntilDue: task.dueDate ? 
          Math.ceil((new Date(task.dueDate).getTime() - now.getTime()) / (1000 * 60 * 60 * 24)) : 
          undefined,
        completionPercentage: task.estimatedPomodoros ? 
          Math.round(((task.completedPomodoros || 0) / task.estimatedPomodoros) * 100) : 
          0,
        statusColor: getStatusColor(task.status),
        priorityColor: getPriorityColor(task.priority),
      }))
    }),
    
    // Statistics computed from current tasks
    taskStatistics: computed((): TaskStatsViewModel => {
      const currentTasks = store.tasks()
      const now = new Date()
      
      const total = currentTasks.length
      const pending = currentTasks.filter(t => t.status === TaskStatus.Pending).length
      const inProgress = currentTasks.filter(t => t.status === TaskStatus.InProgress).length
      const completed = currentTasks.filter(t => t.status === TaskStatus.Completed).length
      const overdue = currentTasks.filter(t => 
        t.dueDate && new Date(t.dueDate) < now && !t.isCompleted
      ).length
      
      return {
        total,
        pending,
        inProgress,
        completed,
        overdue,
        completionRate: total > 0 ? Math.round((completed / total) * 100) : 0,
        productivity: store.stats()?.productivityScore || 0
      }
    }),
    
    // Tasks by status for Kanban view
    tasksByStatus: computed(() => {
      const filtered = store.tasks()
      return {
        pending: filtered.filter((t: Task) => t.status === TaskStatus.Pending),
        inProgress: filtered.filter((t: Task) => t.status === TaskStatus.InProgress),
        completed: filtered.filter((t: Task) => t.status === TaskStatus.Completed),
        cancelled: filtered.filter((t: Task) => t.status === TaskStatus.Cancelled)
      }
    }),
    
    // Pagination info
    totalPages: computed(() => {
      return Math.ceil(store.totalCount() / store.pageSize())
    }),
    
    hasNextPage: computed(() => {
      const totalPages = Math.ceil(store.totalCount() / store.pageSize())
      return store.currentPage() < totalPages
    }),
    
    hasPreviousPage: computed(() => {
      return store.currentPage() > 1
    })
  })),
  withMethods((
    store,
    taskService = inject(TaskService),
    router = inject(Router)
  ) => ({
    
    // Load methods
    loadTasks: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null })
          
          const params = {
            page: store.currentPage(),
            size: store.pageSize(),
            projectId: store.filters().projectId
          }
          
          return taskService.getAll(params).pipe(
            tapResponse({
              next: (response: TaskListResponse) => {
                patchState(store, {
                  tasks: response.items,
                  totalCount: response.totalCount,
                  loading: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load tasks'
                })
                console.error('Load tasks error:', error)
              }
            })
          )
        })
      )
    ),

    loadTask: rxMethod<string>(
      pipe(
        switchMap((id) => {
          patchState(store, { loading: true, error: null })
          
          return taskService.getById(id).pipe(
            tapResponse({
              next: (task: Task) => {
                patchState(store, {
                  selectedTask: task,
                  loading: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load task'
                })
                router.navigate(['/404'], { skipLocationChange: true })
              }
            })
          )
        })
      )
    ),

    loadTaskStats: rxMethod<void>(
      pipe(
        switchMap(() => {
          return taskService.getTaskStats().pipe(
            tapResponse({
              next: (stats: TaskStats) => {
                patchState(store, { stats })
              },
              error: (error: any) => {
                console.error('Load task stats error:', error)
              }
            })
          )
        })
      )
    ),

    // Search method
    searchTasks: rxMethod<string>(
      pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((term) => {
          patchState(store, { searchTerm: term, loading: true })
          
          const request: TaskSearchRequest = {
            query: term,
            filters: store.filters(),
            sortBy: store.sortBy(),
            sortDirection: store.sortDirection(),
            page: 1,
            size: store.pageSize()
          }
          
          return taskService.search(request).pipe(
            tapResponse({
              next: (response: TaskListResponse) => {
                patchState(store, {
                  tasks: response.items,
                  totalCount: response.totalCount,
                  currentPage: 1,
                  loading: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  loading: false,
                  error: 'Search failed'
                })
                console.error('Search tasks error:', error)
              }
            })
          )
        })
      )
    ),

    // CRUD operations
    createTask: rxMethod<CreateTaskRequest>(
      pipe(
        exhaustMap((request) => {
          patchState(store, { saving: true, error: null })
          
          return taskService.create(request).pipe(
            tapResponse({
              next: (newTask: Task) => {
                patchState(store, {
                  tasks: [newTask, ...store.tasks()],
                  saving: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to create task'
                })
                console.error('Create task error:', error)
              }
            })
          )
        })
      )
    ),

    updateTask: rxMethod<{ id: string; request: UpdateTaskRequest }>(
      pipe(
        exhaustMap(({ id, request }) => {
          patchState(store, { saving: true, error: null })
          
          return taskService.update(id, request).pipe(
            tapResponse({
              next: (updatedTask: Task) => {
                patchState(store, {
                  tasks: store.tasks().map(task => 
                    task.id === id ? updatedTask : task
                  ),
                  selectedTask: store.selectedTask()?.id === id ? updatedTask : store.selectedTask(),
                  saving: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to update task'
                })
                console.error('Update task error:', error)
              }
            })
          )
        })
      )
    ),

    deleteTask: rxMethod<string>(
      pipe(
        exhaustMap((id) => {
          patchState(store, { deleting: true, error: null })
          
          return taskService.delete(id).pipe(
            tapResponse({
              next: () => {
                patchState(store, {
                  tasks: store.tasks().filter(task => task.id !== id),
                  selectedTask: store.selectedTask()?.id === id ? null : store.selectedTask(),
                  deleting: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  deleting: false,
                  error: 'Failed to delete task'
                })
                console.error('Delete task error:', error)
              }
            })
          )
        })
      )
    ),

    // Task actions
    toggleTaskCompletion: rxMethod<string>(
      pipe(
        exhaustMap((id) => {
          return taskService.toggleTaskCompletion(id).pipe(
            tapResponse({
              next: (updatedTask: Task) => {
                patchState(store, {
                  tasks: store.tasks().map(task => 
                    task.id === id ? updatedTask : task
                  ),
                  selectedTask: store.selectedTask()?.id === id ? updatedTask : store.selectedTask()
                })
              },
              error: (error: any) => {
                patchState(store, {
                  error: 'Failed to toggle task completion'
                })
                console.error('Toggle task completion error:', error)
              }
            })
          )
        })
      )
    ),

    duplicateTask: rxMethod<string>(
      pipe(
        exhaustMap((id) => {
          patchState(store, { saving: true })
          
          return taskService.duplicateTask(id).pipe(
            tapResponse({
              next: (duplicatedTask: Task) => {
                patchState(store, {
                  tasks: [duplicatedTask, ...store.tasks()],
                  saving: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to duplicate task'
                })
                console.error('Duplicate task error:', error)
              }
            })
          )
        })
      )
    ),

    // Filter and UI state methods
    setFilters: (filters: Partial<TaskFilters>) => {
      patchState(store, { 
        filters: { ...store.filters(), ...filters },
        currentPage: 1 
      })
    },

    clearFilters: () => {
      patchState(store, { 
        filters: initialState.filters,
        searchTerm: '',
        currentPage: 1 
      })
    },

    setPage: (page: number) => {
      patchState(store, { currentPage: page })
    },

    setPageSize: (pageSize: number) => {
      patchState(store, { 
        pageSize, 
        currentPage: 1 
      })
    },

    setSorting: (sortBy: keyof Task, sortDirection: 'asc' | 'desc') => {
      patchState(store, { sortBy, sortDirection })
    },

    setViewMode: (viewMode: 'list' | 'grid' | 'kanban') => {
      patchState(store, { viewMode })
    },

    // Utility methods
    clearError: () => patchState(store, { error: null }),
    
    clearSelection: () => patchState(store, { selectedTask: null }),

    refreshTasks: () => {
      // For now, let's load mock data until backend is ready
      patchState(store, { loading: true })
      
      // Mock data
      const mockTasks: Task[] = [
        {
          id: '1',
          title: 'Implement task management UI',
          description: 'Create the Angular components for task management using Signal Store architecture',
          status: TaskStatus.InProgress,
          priority: TaskPriority.High,
          projectId: '1',
          projectName: 'Taskin 2.0',
          assigneeId: '1',
          assigneeName: 'Juan Luis',
          dueDate: new Date('2024-01-15'),
          estimatedPomodoros: 8,
          completedPomodoros: 3,
          tags: ['angular', 'ui', 'frontend'],
          isCompleted: false,
          createdAt: new Date('2024-01-01'),
          updatedAt: new Date('2024-01-10')
        },
        {
          id: '2',
          title: 'Setup database migrations',
          description: 'Create EF migrations for task and project entities',
          status: TaskStatus.Completed,
          priority: TaskPriority.Medium,
          projectId: '1',
          projectName: 'Taskin 2.0',
          assigneeId: '2',
          assigneeName: 'Developer 2',
          dueDate: new Date('2024-01-10'),
          estimatedPomodoros: 4,
          completedPomodoros: 4,
          tags: ['backend', 'database'],
          isCompleted: true,
          completedAt: new Date('2024-01-09'),
          createdAt: new Date('2023-12-28'),
          updatedAt: new Date('2024-01-09')
        },
        {
          id: '3',
          title: 'API documentation',
          description: 'Complete OpenAPI documentation for all endpoints',
          status: TaskStatus.Pending,
          priority: TaskPriority.Low,
          projectId: '2',
          projectName: 'API Documentation',
          assigneeId: '3',
          assigneeName: 'Technical Writer',
          dueDate: new Date('2024-01-20'),
          estimatedPomodoros: 6,
          completedPomodoros: 0,
          tags: ['documentation', 'api'],
          isCompleted: false,
          createdAt: new Date('2024-01-02'),
          updatedAt: new Date('2024-01-02')
        },
        {
          id: '4',
          title: 'Mobile app redesign',
          description: 'Update mobile app UI to match new design system',
          status: TaskStatus.InProgress,
          priority: TaskPriority.Critical,
          projectId: '3',
          projectName: 'Mobile App Redesign',
          dueDate: new Date('2024-01-05'), // Overdue
          estimatedPomodoros: 12,
          completedPomodoros: 2,
          tags: ['mobile', 'ui', 'design'],
          isCompleted: false,
          createdAt: new Date('2023-12-15'),
          updatedAt: new Date('2024-01-08')
        },
        {
          id: '5',
          title: 'E-commerce integration',
          description: 'Integrate payment gateway and shopping cart functionality',
          status: TaskStatus.Cancelled,
          priority: TaskPriority.Medium,
          projectId: '4',
          projectName: 'E-commerce Platform',
          assigneeId: '1',
          assigneeName: 'Juan Luis',
          estimatedPomodoros: 15,
          completedPomodoros: 1,
          tags: ['ecommerce', 'payment', 'backend'],
          isCompleted: false,
          createdAt: new Date('2023-12-20'),
          updatedAt: new Date('2024-01-05')
        }
      ]

      const mockStats: TaskStats = {
        totalTasks: mockTasks.length,
        pendingTasks: mockTasks.filter(t => t.status === TaskStatus.Pending).length,
        inProgressTasks: mockTasks.filter(t => t.status === TaskStatus.InProgress).length,
        completedTasks: mockTasks.filter(t => t.status === TaskStatus.Completed).length,
        overdueTasks: mockTasks.filter(t => t.dueDate && new Date(t.dueDate) < new Date() && !t.isCompleted).length,
        tasksCompletedThisWeek: 2,
        averageCompletionTime: 3.5,
        productivityScore: 85
      }

      setTimeout(() => {
        patchState(store, {
          tasks: mockTasks,
          stats: mockStats,
          totalCount: mockTasks.length,
          loading: false
        })
      }, 500) // Simulate API delay
    },

    // Bulk operations
    bulkUpdateStatus: rxMethod<{ taskIds: string[]; status: TaskStatus }>(
      pipe(
        exhaustMap(({ taskIds, status }) => {
          patchState(store, { saving: true })
          
          return taskService.bulkUpdateStatus(taskIds, status).pipe(
            tapResponse({
              next: () => {
                // Update local state
                patchState(store, {
                  tasks: store.tasks().map(task => 
                    taskIds.includes(task.id) 
                      ? { ...task, status, isCompleted: status === TaskStatus.Completed }
                      : task
                  ),
                  saving: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to update tasks'
                })
                console.error('Bulk update error:', error)
              }
            })
          )
        })
      )
    )
  }))
) {}

// Helper functions
function getStatusColor(status: TaskStatus): string {
  switch (status) {
    case TaskStatus.Pending: return 'text-gray-600 bg-gray-100'
    case TaskStatus.InProgress: return 'text-blue-600 bg-blue-100'
    case TaskStatus.Completed: return 'text-green-600 bg-green-100'
    case TaskStatus.Cancelled: return 'text-red-600 bg-red-100'
    default: return 'text-gray-600 bg-gray-100'
  }
}

function getPriorityColor(priority: TaskPriority): string {
  switch (priority) {
    case TaskPriority.Low: return 'text-gray-600 bg-gray-100'
    case TaskPriority.Medium: return 'text-yellow-600 bg-yellow-100'
    case TaskPriority.High: return 'text-orange-600 bg-orange-100'
    case TaskPriority.Critical: return 'text-red-600 bg-red-100'
    default: return 'text-gray-600 bg-gray-100'
  }
}