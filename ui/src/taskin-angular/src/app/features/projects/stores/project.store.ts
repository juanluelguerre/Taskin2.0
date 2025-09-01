import { Injectable, inject, computed } from '@angular/core'
import { Router } from '@angular/router'
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals'
import { rxMethod } from '@ngrx/signals/rxjs-interop'
import { pipe, switchMap, exhaustMap, debounceTime, distinctUntilChanged, tap, map } from 'rxjs'
import { 
  ProjectService,
  ProjectListDto,
  ProjectDetailsDto,
  ProjectStatsDto,
  CreateProjectCommand,
  UpdateProjectCommand,
  ProjectFilters,
  CollectionResponse,
  ActionResponse
} from '../services/project.service'
import { NotificationService } from '@core/services/notification.service'

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

export type ProjectStatus = 'Active' | 'Completed' | 'OnHold'

export interface ProjectViewModel extends ProjectListDto {
  statusColor: string
  progressColor: string
  isOverdue: boolean
  daysUntilDue?: number
}

export interface ProjectStatsViewModel {
  total: number
  active: number
  completed: number
  onHold: number
  completionRate: number
}

type ProjectState = {
  // Data
  projects: ProjectListDto[]
  selectedProject: ProjectDetailsDto | null
  
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
  statusFilter: ProjectStatus | null
  sortBy: string
  sortOrder: 'asc' | 'desc'
  
  // Stats
  stats: ProjectStatsDto | null
  
  // UI state
  viewMode: 'list' | 'grid'
  
  // Error handling
  error: string | null
}

const initialState: ProjectState = {
  projects: [],
  selectedProject: null,
  loading: false,
  saving: false,
  deleting: false,
  currentPage: 1,
  pageSize: 12,
  totalCount: 0,
  searchTerm: '',
  statusFilter: null,
  sortBy: 'createdAt',
  sortOrder: 'desc',
  stats: null,
  viewMode: 'grid',
  error: null,
}

@Injectable()
export class ProjectStore extends signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    // Project view models with calculated properties
    projectViewModels: computed((): ProjectViewModel[] => {
      const now = new Date()
      
      return store.projects().map((project: ProjectListDto) => ({
        ...project,
        statusColor: getProjectStatusColor(project.status as ProjectStatus),
        progressColor: getProgressColor(project.progress),
        isOverdue: project.dueDate ? new Date(project.dueDate) < now && project.status !== 'Completed' : false,
        daysUntilDue: project.dueDate ? 
          Math.ceil((new Date(project.dueDate).getTime() - now.getTime()) / (1000 * 60 * 60 * 24)) : 
          undefined,
      }))
    }),
    
    // Statistics computed from current projects
    projectStatistics: computed((): ProjectStatsViewModel => {
      const currentStats = store.stats()
      const total = currentStats?.total || 0
      const completed = currentStats?.completed || 0
      
      return {
        total: currentStats?.total || 0,
        active: currentStats?.active || 0,
        completed,
        onHold: currentStats?.onHold || 0,
        completionRate: total > 0 ? Math.round((completed / total) * 100) : 0
      }
    }),
    
    // Projects by status
    projectsByStatus: computed(() => {
      const projects = store.projects()
      return {
        active: projects.filter(p => p.status === 'Active'),
        completed: projects.filter(p => p.status === 'Completed'),
        onHold: projects.filter(p => p.status === 'OnHold')
      }
    }),
    
    // Filtered projects based on current filters
    filteredProjects: computed(() => {
      let filtered = store.projects()
      const search = store.searchTerm().toLowerCase().trim()
      const status = store.statusFilter()
      
      // Apply search
      if (search) {
        filtered = filtered.filter(project => 
          project.name.toLowerCase().includes(search) ||
          project.description?.toLowerCase().includes(search)
        )
      }
      
      // Apply status filter
      if (status) {
        filtered = filtered.filter(project => project.status === status)
      }
      
      return filtered
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
    projectService = inject(ProjectService),
    notificationService = inject(NotificationService),
    router = inject(Router)
  ) => ({
    
    // Load methods
    loadProjects: rxMethod<void>(
      pipe(
        switchMap(() => {
          patchState(store, { loading: true, error: null })
          
          const filters: ProjectFilters = {
            page: store.currentPage(),
            size: store.pageSize(),
            search: store.searchTerm() || undefined,
            status: store.statusFilter() || undefined,
            sort: store.sortBy(),
            order: store.sortOrder()
          }
          
          return projectService.getProjects(filters).pipe(
            tapResponse({
              next: (response: CollectionResponse<ProjectListDto>) => {
                patchState(store, {
                  projects: response.data,
                  totalCount: response.total,
                  loading: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load projects'
                })
                notificationService.notifyError('projects.errors.loadFailed')
                console.error('Load projects error:', error)
              }
            })
          )
        })
      )
    ),

    loadProject: rxMethod<string>(
      pipe(
        switchMap((id) => {
          patchState(store, { loading: true, error: null })
          
          return projectService.getProject(id).pipe(
            tapResponse({
              next: (project: ProjectDetailsDto) => {
                patchState(store, {
                  selectedProject: project,
                  loading: false
                })
              },
              error: (error: any) => {
                patchState(store, {
                  loading: false,
                  error: 'Failed to load project'
                })
                notificationService.notifyError('projects.errors.loadFailed')
                router.navigate(['/404'], { skipLocationChange: true })
              }
            })
          )
        })
      )
    ),

    loadProjectStats: rxMethod<void>(
      pipe(
        switchMap(() => {
          return projectService.getProjectStats().pipe(
            tapResponse({
              next: (stats: ProjectStatsDto) => {
                patchState(store, { stats })
              },
              error: (error: any) => {
                console.error('Load project stats error:', error)
              }
            })
          )
        })
      )
    ),

    // Search method
    searchProjects: rxMethod<string>(
      pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((term) => {
          patchState(store, { searchTerm: term, currentPage: 1 })
          // Trigger load with new search term
          return projectService.getProjects({ 
            search: term, 
            status: store.statusFilter() || undefined
          }).pipe(
            map(response => ({ data: response.data, totalCount: response.total }))
          )
        })
      )
    ),

    // CRUD operations
    createProject: rxMethod<CreateProjectCommand>(
      pipe(
        exhaustMap((command) => {
          patchState(store, { saving: true, error: null })
          
          return projectService.createProject(command).pipe(
            tapResponse({
              next: (response: ActionResponse) => {
                if (response.success) {
                  patchState(store, { saving: false })
                  notificationService.notifySuccess('projects.messages.created')
                  // Reload projects to get updated list
                  projectService.getProjects().subscribe(response => {
                    patchState(store, { projects: response.data })
                  })
                  router.navigate(['/projects'])
                } else {
                  patchState(store, {
                    saving: false,
                    error: response.message
                  })
                  notificationService.notifyError('projects.errors.createFailed')
                }
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to create project'
                })
                notificationService.notifyError('projects.errors.createFailed')
                console.error('Create project error:', error)
              }
            })
          )
        })
      )
    ),

    updateProject: rxMethod<{ id: string; command: UpdateProjectCommand }>(
      pipe(
        exhaustMap(({ id, command }) => {
          patchState(store, { saving: true, error: null })
          
          return projectService.updateProject(id, command).pipe(
            tapResponse({
              next: (response: ActionResponse) => {
                if (response.success) {
                  patchState(store, { saving: false })
                  notificationService.notifySuccess('projects.messages.updated')
                  // Reload projects to get updated data
                  projectService.getProjects().subscribe(response => {
                    patchState(store, { projects: response.data })
                  })
                  if (store.selectedProject()?.id === id) {
                    projectService.getProject(id).subscribe(project => {
                      patchState(store, { selectedProject: project })
                    })
                  }
                } else {
                  patchState(store, {
                    saving: false,
                    error: response.message
                  })
                  notificationService.notifyError('projects.errors.updateFailed')
                }
              },
              error: (error: any) => {
                patchState(store, {
                  saving: false,
                  error: 'Failed to update project'
                })
                notificationService.notifyError('projects.errors.updateFailed')
                console.error('Update project error:', error)
              }
            })
          )
        })
      )
    ),

    deleteProject: rxMethod<string>(
      pipe(
        exhaustMap((id) => {
          patchState(store, { deleting: true, error: null })
          
          return projectService.deleteProject(id).pipe(
            tapResponse({
              next: (response: ActionResponse) => {
                if (response.success) {
                  patchState(store, {
                    projects: store.projects().filter(project => project.id !== id),
                    selectedProject: store.selectedProject()?.id === id ? null : store.selectedProject(),
                    deleting: false
                  })
                  notificationService.notifySuccess('projects.messages.deleted')
                } else {
                  patchState(store, {
                    deleting: false,
                    error: response.message
                  })
                  notificationService.notifyError('projects.errors.deleteFailed')
                }
              },
              error: (error: any) => {
                patchState(store, {
                  deleting: false,
                  error: 'Failed to delete project'
                })
                notificationService.notifyError('projects.errors.deleteFailed')
                console.error('Delete project error:', error)
              }
            })
          )
        })
      )
    ),

    // Filter and UI state methods
    setSearchTerm: (term: string) => {
      patchState(store, { searchTerm: term, currentPage: 1 })
    },

    setStatusFilter: (status: ProjectStatus | null) => {
      patchState(store, { statusFilter: status, currentPage: 1 })
    },

    setSorting: (sortBy: string, sortOrder: 'asc' | 'desc') => {
      patchState(store, { sortBy, sortOrder, currentPage: 1 })
    },

    setPage: (page: number) => {
      patchState(store, { currentPage: page })
    },

    setPageSize: (pageSize: number) => {
      patchState(store, { pageSize, currentPage: 1 })
    },

    setViewMode: (viewMode: 'list' | 'grid') => {
      patchState(store, { viewMode })
    },

    clearFilters: () => {
      patchState(store, {
        searchTerm: '',
        statusFilter: null,
        currentPage: 1
      })
    },

    // Utility methods
    clearError: () => patchState(store, { error: null }),
    
    clearSelection: () => patchState(store, { selectedProject: null }),

    refreshProjects: () => {
      projectService.getProjects().subscribe(response => {
        patchState(store, { projects: response.data, loading: false })
      })
      projectService.getProjectStats().subscribe(stats => {
        patchState(store, { stats })
      })
    }
  }))
) {}

// Helper functions
function getProjectStatusColor(status: ProjectStatus): string {
  switch (status) {
    case 'Active': return 'text-blue-600 bg-blue-100'
    case 'Completed': return 'text-green-600 bg-green-100'
    case 'OnHold': return 'text-yellow-600 bg-yellow-100'
    default: return 'text-gray-600 bg-gray-100'
  }
}

function getProgressColor(progress: number): string {
  if (progress >= 100) return 'text-green-600'
  if (progress >= 75) return 'text-blue-600'
  if (progress >= 50) return 'text-yellow-600'
  if (progress >= 25) return 'text-orange-600'
  return 'text-red-600'
}