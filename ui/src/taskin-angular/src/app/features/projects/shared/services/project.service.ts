import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';

export interface Project {
  id: string;
  name: string;
  description?: string;
  status: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface ProjectListDto {
  id: string;
  name: string;
  description?: string;
  status: string;
}

export interface CollectionResponse<T> {
  items: T[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/projects`;

  getAll(params?: { 
    page?: number; 
    size?: number; 
    search?: string; 
    status?: string; 
    sort?: string; 
    order?: string; 
  }): Observable<CollectionResponse<ProjectListDto>> {
    let httpParams = new HttpParams();
    
    if (params?.page !== undefined) httpParams = httpParams.set('page', params.page.toString());
    if (params?.size !== undefined) httpParams = httpParams.set('size', params.size.toString());
    if (params?.search) httpParams = httpParams.set('search', params.search);
    if (params?.status) httpParams = httpParams.set('status', params.status);
    if (params?.sort) httpParams = httpParams.set('sort', params.sort);
    if (params?.order) httpParams = httpParams.set('order', params.order);

    return this.http.get<CollectionResponse<ProjectListDto>>(this.baseUrl, { params: httpParams });
  }

  getById(id: string): Observable<Project> {
    return this.http.get<Project>(`${this.baseUrl}/${id}`);
  }

  getProjectsForDropdown(): Observable<ProjectListDto[]> {
    const params = new HttpParams()
      .set('page', '1')
      .set('size', '100')
      .set('sort', 'name')
      .set('order', 'asc');
    
    return this.http.get<CollectionResponse<ProjectListDto>>(this.baseUrl, { params }).pipe(
      map(response => response.items)
    );
  }
}