import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface Assignee {
  id: string;
  name: string;
  email?: string;
  avatar?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AssigneeService {
  
  private readonly mockAssignees: Assignee[] = [
    { id: '1', name: 'John Doe', email: 'john.doe@example.com' },
    { id: '2', name: 'Jane Smith', email: 'jane.smith@example.com' },
    { id: '3', name: 'Mike Johnson', email: 'mike.johnson@example.com' },
    { id: '4', name: 'Sarah Wilson', email: 'sarah.wilson@example.com' },
    { id: '5', name: 'David Brown', email: 'david.brown@example.com' },
    { id: '6', name: 'Lisa Garcia', email: 'lisa.garcia@example.com' }
  ];

  getAssigneesForDropdown(): Observable<Assignee[]> {
    return of([...this.mockAssignees]);
  }

  getById(id: string): Observable<Assignee | undefined> {
    const assignee = this.mockAssignees.find(a => a.id === id);
    return of(assignee);
  }

  search(query: string): Observable<Assignee[]> {
    const filtered = this.mockAssignees.filter(assignee =>
      assignee.name.toLowerCase().includes(query.toLowerCase()) ||
      assignee.email?.toLowerCase().includes(query.toLowerCase())
    );
    return of(filtered);
  }
}