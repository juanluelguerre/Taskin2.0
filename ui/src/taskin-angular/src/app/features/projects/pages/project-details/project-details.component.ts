import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService, ProjectDetailsDto } from '../../services/project.service';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatCardModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    DatePipe,
    TitleCasePipe
  ],
  templateUrl: './project-details.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectDetailsComponent implements OnInit {
  private readonly projectService = inject(ProjectService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  // State signals
  project = signal<ProjectDetailsDto | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    const projectId = this.route.snapshot.params['id'];
    if (projectId) {
      this.loadProject(projectId);
    }
  }

  loadProject(id: string) {
    this.loading.set(true);
    this.error.set(null);

    this.projectService.getProject(id).subscribe({
      next: (project) => {
        this.project.set(project);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Error loading project details. Please try again.');
        this.loading.set(false);
        console.error('Error loading project:', err);
      }
    });
  }

  onEdit() {
    const project = this.project();
    if (project) {
      this.router.navigate(['/projects', project.id, 'edit']);
    }
  }

  onDelete() {
    const project = this.project();
    if (project && confirm('Are you sure you want to delete this project?')) {
      this.loading.set(true);
      this.projectService.deleteProject(project.id).subscribe({
        next: () => {
          this.router.navigate(['/projects']);
        },
        error: (err) => {
          this.error.set('Error deleting project. Please try again.');
          this.loading.set(false);
          console.error('Error deleting project:', err);
        }
      });
    }
  }

  onBackToProjects() {
    this.router.navigate(['/projects']);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'active': return 'bg-green-100 text-green-800';
      case 'completed': return 'bg-blue-100 text-blue-800';
      case 'onhold': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusDisplayName(status: string): string {
    switch (status.toLowerCase()) {
      case 'onhold': return 'On Hold';
      default: return status;
    }
  }

  getTaskStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'done': return 'bg-green-100 text-green-800';
      case 'inprogress': return 'bg-blue-100 text-blue-800';
      case 'todo': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getTaskStatusDisplayName(status: string): string {
    switch (status.toLowerCase()) {
      case 'inprogress': return 'In Progress';
      case 'todo': return 'To Do';
      default: return status;
    }
  }

  getPriorityColor(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'high': return 'bg-red-100 text-red-800';
      case 'medium': return 'bg-yellow-100 text-yellow-800';
      case 'low': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
