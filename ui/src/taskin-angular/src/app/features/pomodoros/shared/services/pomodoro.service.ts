import { Injectable, inject } from '@angular/core'
import { HttpClient, HttpParams } from '@angular/common/http'
import { Observable } from 'rxjs'
import { environment } from '@env/environment'
import { 
  Pomodoro, 
  PomodoroStatus,
  PomodoroType,
  CreatePomodoroRequest, 
  UpdatePomodoroRequest, 
  PomodoroListResponse, 
  PomodoroStats,
  PomodoroSearchRequest,
  PomodoroFilters,
  PomodoroSettings,
  PomodoroSession,
  DailyPomodoroSummary
} from '../types/pomodoro.types'

// Base repository interface
export interface IRepository<T, TCreate, TUpdate> {
  getAll(params?: any): Observable<PomodoroListResponse>
  getById(id: string): Observable<T>
  create(item: TCreate): Observable<T>
  update(id: string, item: TUpdate): Observable<T>
  delete(id: string): Observable<void>
  search(request: PomodoroSearchRequest): Observable<PomodoroListResponse>
}

// Pomodoro-specific service interface
export interface IPomodoroService extends IRepository<Pomodoro, CreatePomodoroRequest, UpdatePomodoroRequest> {
  getPomodorosByTaskId(taskId: string): Observable<Pomodoro[]>
  getPomodoroStats(): Observable<PomodoroStats>
  getPomodoroStatsForTask(taskId: string): Observable<PomodoroStats>
  startPomodoro(id: string): Observable<PomodoroSession>
  pausePomodoro(id: string): Observable<Pomodoro>
  resumePomodoro(id: string): Observable<Pomodoro>
  completePomodoro(id: string): Observable<Pomodoro>
  cancelPomodoro(id: string): Observable<Pomodoro>
  getActivePomodoro(): Observable<Pomodoro | null>
  getTodayPomodoros(): Observable<Pomodoro[]>
  getDailySummary(date: Date): Observable<DailyPomodoroSummary>
  getWeeklyPomodoros(): Observable<Pomodoro[]>
  getSettings(): Observable<PomodoroSettings>
  updateSettings(settings: PomodoroSettings): Observable<PomodoroSettings>
}

@Injectable({
  providedIn: 'root'
})
export class PomodoroService implements IPomodoroService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = `${environment.apiUrl}/api/pomodoros`

  getAll(params?: { page?: number; size?: number; taskId?: string }): Observable<PomodoroListResponse> {
    let httpParams = new HttpParams()
    
    if (params?.page !== undefined) httpParams = httpParams.set('page', params.page.toString())
    if (params?.size !== undefined) httpParams = httpParams.set('size', params.size.toString())
    if (params?.taskId) httpParams = httpParams.set('taskId', params.taskId)

    return this.http.get<PomodoroListResponse>(this.baseUrl, { params: httpParams })
  }

  getById(id: string): Observable<Pomodoro> {
    return this.http.get<Pomodoro>(`${this.baseUrl}/${id}`)
  }

  create(request: CreatePomodoroRequest): Observable<Pomodoro> {
    return this.http.post<Pomodoro>(this.baseUrl, request)
  }

  update(id: string, request: UpdatePomodoroRequest): Observable<Pomodoro> {
    return this.http.put<Pomodoro>(`${this.baseUrl}/${id}`, request)
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
  }

  search(request: PomodoroSearchRequest): Observable<PomodoroListResponse> {
    return this.http.post<PomodoroListResponse>(`${this.baseUrl}/search`, request)
  }

  getPomodorosByTaskId(taskId: string): Observable<Pomodoro[]> {
    const params = new HttpParams().set('taskId', taskId)
    return this.http.get<Pomodoro[]>(this.baseUrl, { params })
  }

  getPomodoroStats(): Observable<PomodoroStats> {
    return this.http.get<PomodoroStats>(`${this.baseUrl}/stats`)
  }

  getPomodoroStatsForTask(taskId: string): Observable<PomodoroStats> {
    return this.http.get<PomodoroStats>(`${this.baseUrl}/stats/task/${taskId}`)
  }

  startPomodoro(id: string): Observable<PomodoroSession> {
    return this.http.post<PomodoroSession>(`${this.baseUrl}/${id}/start`, {})
  }

  pausePomodoro(id: string): Observable<Pomodoro> {
    return this.http.post<Pomodoro>(`${this.baseUrl}/${id}/pause`, {})
  }

  resumePomodoro(id: string): Observable<Pomodoro> {
    return this.http.post<Pomodoro>(`${this.baseUrl}/${id}/resume`, {})
  }

  completePomodoro(id: string): Observable<Pomodoro> {
    return this.http.post<Pomodoro>(`${this.baseUrl}/${id}/complete`, {})
  }

  cancelPomodoro(id: string): Observable<Pomodoro> {
    return this.http.post<Pomodoro>(`${this.baseUrl}/${id}/cancel`, {})
  }

  getActivePomodoro(): Observable<Pomodoro | null> {
    return this.http.get<Pomodoro | null>(`${this.baseUrl}/active`)
  }

  getTodayPomodoros(): Observable<Pomodoro[]> {
    return this.http.get<Pomodoro[]>(`${this.baseUrl}/today`)
  }

  getDailySummary(date: Date): Observable<DailyPomodoroSummary> {
    const params = new HttpParams().set('date', date.toISOString().split('T')[0])
    return this.http.get<DailyPomodoroSummary>(`${this.baseUrl}/daily-summary`, { params })
  }

  getWeeklyPomodoros(): Observable<Pomodoro[]> {
    return this.http.get<Pomodoro[]>(`${this.baseUrl}/weekly`)
  }

  getSettings(): Observable<PomodoroSettings> {
    return this.http.get<PomodoroSettings>(`${this.baseUrl}/settings`)
  }

  updateSettings(settings: PomodoroSettings): Observable<PomodoroSettings> {
    return this.http.put<PomodoroSettings>(`${this.baseUrl}/settings`, settings)
  }
}

// Pomodoro Repository (extending base functionality)
@Injectable({
  providedIn: 'root'
})
export class PomodoroRepository {
  private readonly pomodoroService = inject(PomodoroService)

  // Delegate to service
  getAll = this.pomodoroService.getAll.bind(this.pomodoroService)
  getById = this.pomodoroService.getById.bind(this.pomodoroService)
  create = this.pomodoroService.create.bind(this.pomodoroService)
  update = this.pomodoroService.update.bind(this.pomodoroService)
  delete = this.pomodoroService.delete.bind(this.pomodoroService)

  // Additional repository methods for complex operations
  getPomodorosWithStats(taskId?: string): Observable<{ pomodoros: Pomodoro[], stats: PomodoroStats }> {
    // Could implement caching, transformation, etc.
    // For now, this would need to be implemented with multiple API calls
    throw new Error('Method not implemented')
  }

  getPomodorosForDashboard(): Observable<Pomodoro[]> {
    // Get recent and active pomodoros for dashboard
    return this.pomodoroService.getTodayPomodoros()
  }
}