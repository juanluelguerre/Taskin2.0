import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal, OnInit, inject, computed } from '@angular/core';
import { CommonModule, TitleCasePipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectService, ProjectListDto, ProjectStatsDto, CollectionResponse } from '../../services/project.service';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonToggleModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    TitleCasePipe,
    DatePipe,
    FormsModule
  ],
  templateUrl: './projects.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectsComponent implements OnInit {
  private readonly projectService = inject(ProjectService);
  private readonly router = inject(Router);
  
  // State signals
  projects = signal<ProjectListDto[]>([]);
  stats = signal<ProjectStatsDto>({ total: 0, active: 0, completed: 0, onHold: 0 });
  loading = signal(false);
  error = signal<string | null>(null);
  
  // Filter signals
  selectedFilter = signal('all');
  searchTerm = signal('');
  currentPage = signal(1);
  pageSize = signal(12);
  totalCount = signal(0);

  // Computed values
  filteredProjects = computed(() => {
    const filter = this.selectedFilter();
    const search = this.searchTerm().toLowerCase();
    let filtered = this.projects();

    // Apply status filter
    if (filter !== 'all') {
      filtered = filtered.filter(p => p.status === filter);
    }

    // Apply search filter (already handled by API, but keeping for local filtering if needed)
    if (search) {
      filtered = filtered.filter(p => 
        p.name.toLowerCase().includes(search) || 
        (p.description && p.description.toLowerCase().includes(search))
      );
    }

    return filtered;
  });

  ngOnInit() {
    this.loadProjects();
    this.loadStats();
  }

  loadProjects() {
    this.loading.set(true);
    this.error.set(null);
    
    const filters = {
      page: this.currentPage(),
      size: this.pageSize(),
      status: this.selectedFilter() === 'all' ? undefined : this.selectedFilter(),
      search: this.searchTerm() || undefined
    };

    this.projectService.getProjects(filters).subscribe({
      next: (response: CollectionResponse<ProjectListDto>) => {
        this.projects.set(response.data);
        this.totalCount.set(response.total);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Error loading projects. Please try again.');
        this.loading.set(false);
        console.error('Error loading projects:', err);
      }
    });
  }

  loadStats() {
    this.projectService.getProjectStats().subscribe({
      next: (stats) => {
        this.stats.set(stats);
      },
      error: (err) => {
        console.error('Error loading project stats:', err);
      }
    });
  }

  onFilterChange(filter: string) {
    this.selectedFilter.set(filter);
    this.currentPage.set(1); // Reset to first page
    this.loadProjects();
  }

  onSearch(searchTerm: string | Event) {
    const search = typeof searchTerm === 'string' ? searchTerm : (searchTerm.target as HTMLInputElement).value;
    this.searchTerm.set(search);
    this.currentPage.set(1); // Reset to first page
    this.loadProjects();
  }

  onNewProject() {
    this.router.navigate(['/projects/new']);
  }

  onViewProject(projectId: string) {
    this.router.navigate(['/projects', projectId]);
  }

  onEditProject(projectId: string) {
    this.router.navigate(['/projects', projectId, 'edit']);
  }

  onDeleteProject(projectId: string) {
    if (confirm('Are you sure you want to delete this project?')) {
      this.loading.set(true);
      this.projectService.deleteProject(projectId).subscribe({
        next: () => {
          this.loadProjects(); // Reload projects after deletion
          this.loadStats(); // Reload stats
        },
        error: (err) => {
          this.error.set('Error deleting project. Please try again.');
          this.loading.set(false);
          console.error('Error deleting project:', err);
        }
      });
    }
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
}
