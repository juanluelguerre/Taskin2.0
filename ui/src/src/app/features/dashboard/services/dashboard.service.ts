import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { environment } from '@env/environment'

export interface DashboardStats {
  activeProjects: number
  pendingTasks: number
  completedToday: number
  pomodorosToday: number
  weeklyProgress: number
  focusHours: number
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = `${environment.apiUrl}/api/Dashboard`

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.baseUrl}/stats`)
  }
}
