import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ViewEncapsulation,
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { StatusColorPipe, StatusDisplayPipe } from '@shared/pipes';
import { ProjectStatus, ProjectStore } from '../../stores/project.store';

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
    FormsModule,
    StatusColorPipe,
    StatusDisplayPipe,
  ],
  providers: [ProjectStore],
  templateUrl: './projects.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectsComponent implements OnInit {
  private readonly projectStore = inject(ProjectStore);
  private readonly router = inject(Router);
  private readonly confirmationService = inject(UiConfirmationService);

  // Expose store selectors
  readonly projects = this.projectStore.projectViewModels;
  readonly stats = this.projectStore.projectStatistics;
  readonly loading = this.projectStore.loading;
  readonly saving = this.projectStore.saving;
  readonly deleting = this.projectStore.deleting;
  readonly error = this.projectStore.error;
  readonly searchTerm = this.projectStore.searchTerm;
  readonly statusFilter = this.projectStore.statusFilter;
  readonly currentPage = this.projectStore.currentPage;
  readonly totalPages = this.projectStore.totalPages;
  readonly hasNextPage = this.projectStore.hasNextPage;
  readonly hasPreviousPage = this.projectStore.hasPreviousPage;
  readonly viewMode = this.projectStore.viewMode;
  readonly selectedFilter = this.projectStore.statusFilter;

  ngOnInit() {
    this.projectStore.loadProjects();
    this.projectStore.loadProjectStats();
  }

  onFilterChange(filter: string) {
    const status = filter === 'all' ? null : (filter as ProjectStatus);
    this.projectStore.setStatusFilter(status);
    this.projectStore.loadProjects();
  }

  onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.projectStore.searchProjects(target.value);
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

  onDeleteProject(projectId: string, projectName: string) {
    const confirmationRef = this.confirmationService.open({
      title: 'Delete Project',
      message: `Are you sure you want to delete the project "${projectName}"? This action cannot be undone.`,
      icon: {
        show: true,
        name: 'delete',
        color: 'warn',
      },
      actions: {
        confirm: {
          show: true,
          label: 'Delete',
          color: 'warn',
        },
        cancel: {
          show: true,
          label: 'Cancel',
        },
      },
      dismissible: true,
    });

    confirmationRef.afterClosed().subscribe(result => {
      if (result === 'confirmed') {
        this.projectStore.deleteProject(projectId);
      }
    });
  }

  onPageChange(page: number) {
    this.projectStore.setPage(page);
    this.projectStore.loadProjects();
  }


  onToggleViewMode() {
    const newMode = this.viewMode() === 'grid' ? 'list' : 'grid';
    this.projectStore.setViewMode(newMode);
  }

  onClearFilters() {
    this.projectStore.clearFilters();
    this.projectStore.loadProjects();
  }

  onRefresh() {
    this.projectStore.refreshProjects();
  }
}
