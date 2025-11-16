import { Injectable, inject } from '@angular/core'
import { HttpClient, HttpParams } from '@angular/common/http'
import { Observable } from 'rxjs'
import { map } from 'rxjs/operators'
import { 
  Task, 
  TaskStatus,
  CreateTaskRequest, 
  UpdateTaskRequest, 
  TaskListResponse, 
  TaskStats,
  TaskSearchRequest,
  TaskFilters
} from '../types/task.types'

// Base repository interface
export interface IRepository<T, TCreate, TUpdate> {
  getAll(params?: any): Observable<TaskListResponse>
  getById(id: string): Observable<T>
  create(item: TCreate): Observable<T>
  update(id: string, item: TUpdate): Observable<T>
  delete(id: string): Observable<void>
  search(request: TaskSearchRequest): Observable<TaskListResponse>
}

// Task-specific service interface
export interface ITaskService extends IRepository<Task, CreateTaskRequest, UpdateTaskRequest> {
  getTasksByProjectId(projectId: string): Observable<Task[]>
  getTaskStats(): Observable<TaskStats>
  toggleTaskCompletion(id: string): Observable<Task>
  duplicateTask(id: string): Observable<Task>
  getOverdueTasks(): Observable<Task[]>
  getTasksByAssignee(assigneeId: string): Observable<Task[]>
  bulkUpdateStatus(taskIds: string[], status: TaskStatus): Observable<void>
  addTagToTask(taskId: string, tag: string): Observable<Task>
  removeTagFromTask(taskId: string, tag: string): Observable<Task>
}

@Injectable({
  providedIn: 'root'
})
export class TaskService implements ITaskService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = '/api/tasks'

  getAll(params?: { page?: number; size?: number; projectId?: string }): Observable<TaskListResponse> {
    let httpParams = new HttpParams()
    
    if (params?.page !== undefined) httpParams = httpParams.set('page', params.page.toString())
    if (params?.size !== undefined) httpParams = httpParams.set('size', params.size.toString())
    if (params?.projectId) httpParams = httpParams.set('projectId', params.projectId)

    return this.http.get<TaskListResponse>(this.baseUrl, { params: httpParams })
  }

  getById(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}/${id}`)
  }

  create(request: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.baseUrl, request)
  }

  update(id: string, request: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.baseUrl}/${id}`, request)
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
  }

  search(request: TaskSearchRequest): Observable<TaskListResponse> {
    return this.http.post<TaskListResponse>(`${this.baseUrl}/search`, request)
  }

  getTasksByProjectId(projectId: string): Observable<Task[]> {
    const params = new HttpParams().set('projectId', projectId)
    return this.http.get<Task[]>(this.baseUrl, { params })
  }

  getTaskStats(): Observable<TaskStats> {
    return this.http.get<TaskStats>(`${this.baseUrl}/stats`)
  }

  toggleTaskCompletion(id: string): Observable<Task> {
    return this.http.post<Task>(`${this.baseUrl}/${id}/toggle-completion`, {})
  }

  duplicateTask(id: string): Observable<Task> {
    return this.http.post<Task>(`${this.baseUrl}/${id}/duplicate`, {})
  }

  getOverdueTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.baseUrl}/overdue`)
  }

  getTasksByAssignee(assigneeId: string): Observable<Task[]> {
    const params = new HttpParams().set('assigneeId', assigneeId)
    return this.http.get<Task[]>(this.baseUrl, { params })
  }

  bulkUpdateStatus(taskIds: string[], status: TaskStatus): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/bulk-update-status`, {
      taskIds,
      status
    })
  }

  addTagToTask(taskId: string, tag: string): Observable<Task> {
    return this.http.post<Task>(`${this.baseUrl}/${taskId}/tags`, { tag })
  }

  removeTagFromTask(taskId: string, tag: string): Observable<Task> {
    return this.http.delete<Task>(`${this.baseUrl}/${taskId}/tags/${tag}`)
  }
}

// Task Repository (extending base functionality)
@Injectable({
  providedIn: 'root'
})
export class TaskRepository {
  private readonly taskService = inject(TaskService)

  // Delegate to service
  getAll = this.taskService.getAll.bind(this.taskService)
  getById = this.taskService.getById.bind(this.taskService)
  create = this.taskService.create.bind(this.taskService)
  update = this.taskService.update.bind(this.taskService)
  delete = this.taskService.delete.bind(this.taskService)

  // Additional repository methods for complex operations
  getTasksWithRelations(projectId?: string): Observable<Task[]> {
    // Could implement caching, transformation, etc.
    return this.taskService.getTasksByProjectId(projectId || '')
  }

  getTasksForDashboard(): Observable<Task[]> {
    // Get recent and important tasks for dashboard
    return this.taskService.search({
      query: '',
      filters: {
        status: TaskStatus.InProgress
      },
      sortBy: 'updatedAt',
      sortDirection: 'desc',
      page: 1,
      size: 5
    }).pipe(
      map((response: TaskListResponse) => response.items)
    )
  }
}