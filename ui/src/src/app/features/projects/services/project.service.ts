import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

// DTOs matching backend
export interface ProjectListDto {
  id: string;
  name: string;
  description: string | null;
  status: string;
  progress: number;
  totalTasks: number;
  completedTasks: number;
  dueDate: string;
  imageUrl: string | null;
  backgroundColor: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface ProjectDetailsDto {
  id: string;
  name: string;
  description: string | null;
  status: string;
  progress: number;
  totalTasks: number;
  completedTasks: number;
  dueDate: string;
  imageUrl: string | null;
  backgroundColor: string | null;
  createdAt: string;
  updatedAt: string;
  tasks: TaskSummaryDto[];
  teamMembers: TeamMemberDto[];
}

export interface TaskSummaryDto {
  id: string;
  title: string;
  status: string;
  priority: string;
}

export interface TeamMemberDto {
  id: string;
  name: string;
  email: string;
  initials: string;
  avatar: string | null;
}

export interface CollectionResponse<T> {
  data: T[];
  total: number;
  page: number;
  size: number;
}

export interface ActionResponse {
  id: string;
  message: string;
  success: boolean;
}

export interface ProjectStatsDto {
  total: number;
  active: number;
  completed: number;
  onHold: number;
}

export interface CreateProjectCommand {
  name: string;
  description?: string;
  dueDate?: string;
  imageUrl?: string;
  backgroundColor?: string;
  status?: number; // 0: Active, 1: Completed, 2: OnHold
}

export interface UpdateProjectCommand extends CreateProjectCommand {
  id: string;
}

export interface ProjectFilters {
  page?: number;
  size?: number;
  search?: string;
  status?: string;
  sort?: string;
  order?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Projects`;

  getProjects(filters: ProjectFilters = {}): Observable<CollectionResponse<ProjectListDto>> {
    let params = new HttpParams();
    
    if (filters.page) params = params.set('page', filters.page.toString());
    if (filters.size) params = params.set('size', filters.size.toString());
    if (filters.search) params = params.set('search', filters.search);
    if (filters.status) params = params.set('status', filters.status);
    if (filters.sort) params = params.set('sort', filters.sort);
    if (filters.order) params = params.set('order', filters.order);

    return this.http.get<CollectionResponse<ProjectListDto>>(this.baseUrl, { params });
  }

  getProject(id: string): Observable<ProjectDetailsDto> {
    return this.http.get<ProjectDetailsDto>(`${this.baseUrl}/${id}`);
  }

  createProject(command: CreateProjectCommand): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.baseUrl, command);
  }

  updateProject(id: string, command: UpdateProjectCommand): Observable<ActionResponse> {
    return this.http.put<ActionResponse>(`${this.baseUrl}/${id}`, { ...command, id });
  }

  deleteProject(id: string): Observable<ActionResponse> {
    return this.http.delete<ActionResponse>(`${this.baseUrl}/${id}`);
  }

  getProjectStats(): Observable<ProjectStatsDto> {
    return this.http.get<ProjectStatsDto>(`${this.baseUrl}/stats`);
  }
}