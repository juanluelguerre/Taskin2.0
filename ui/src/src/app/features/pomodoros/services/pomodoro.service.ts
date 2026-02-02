import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { environment } from '@env/environment'

export interface CreatePomodoroRequest {
  taskId: string
  startTime: string
  durationInMinutes: number
}

export interface PomodoroDto {
  id: string
  taskId: string
  startTime: string
  durationInMinutes: number
  createdAt: string
}

@Injectable({
  providedIn: 'root'
})
export class PomodoroService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = `${environment.apiUrl}/api/Pomodoros`

  create(request: CreatePomodoroRequest): Observable<string> {
    return this.http.post<string>(this.baseUrl, request)
  }

  getToday(): Observable<PomodoroDto[]> {
    return this.http.get<PomodoroDto[]>(`${this.baseUrl}/today`)
  }
}
