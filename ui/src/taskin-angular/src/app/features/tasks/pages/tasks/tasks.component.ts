import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ViewEncapsulation,
  computed,
  effect,
  inject,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { NotificationService } from '@core/services/notification.service';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { TaskCardComponent, TaskFiltersComponent, TaskStatsComponent } from '../../components';
import { TaskFilters, TaskStatus, TaskStore, TaskViewModel } from '../../shared';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatButtonToggleModule,
    TaskCardComponent,
    TaskStatsComponent,
    TaskFiltersComponent,
  ],
  templateUrl: './tasks.component.html',
  styles: `
    .loading-bar {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      z-index: 10;
    }
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TasksComponent implements OnInit {
  // Dependencies
  private readonly taskStore = inject(TaskStore);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);

  // Store selectors - exposed as signals
  readonly tasks = this.taskStore.taskViewModels;
  readonly stats = this.taskStore.taskStatistics;
  readonly loading = this.taskStore.loading;
  readonly saving = this.taskStore.saving;
  readonly error = this.taskStore.error;
  readonly filters = this.taskStore.filters;
  readonly searchTerm = this.taskStore.searchTerm;
  readonly viewMode = this.taskStore.viewMode;
  readonly totalPages = this.taskStore.totalPages;
  readonly currentPage = this.taskStore.currentPage;
  readonly hasNextPage = this.taskStore.hasNextPage;
  readonly hasPreviousPage = this.taskStore.hasPreviousPage;

  // Mock data for dropdowns (in real app, these would come from services)
  readonly projects = [
    { id: '1', name: 'Taskin 2.0' },
    { id: '2', name: 'E-commerce Platform' },
    { id: '3', name: 'Mobile App Redesign' },
    { id: '4', name: 'API Documentation' },
  ];

  readonly assignees = [
    { id: '1', name: 'John Doe' },
    { id: '2', name: 'Jane Smith' },
    { id: '3', name: 'Mike Johnson' },
    { id: '4', name: 'Sarah Wilson' },
  ];

  // Computed properties for template expressions
  readonly hasActiveFilters = computed(() => {
    const filters = this.filters();
    return Object.keys(filters).some(key => filters[key as keyof typeof filters]);
  });

  readonly showingFromIndex = computed(() => (this.currentPage() - 1) * 25 + 1);

  readonly showingToIndex = computed(() => Math.min(this.currentPage() * 25, this.tasks().length));

  readonly cancelledTasksCount = computed(
    () => this.tasks().filter(t => t.status === TaskStatus.Cancelled).length
  );

  // Enum references for template use
  readonly TaskStatus = TaskStatus;

  ngOnInit(): void {
    // Load initial data from backend
    this.taskStore.loadTasks();
    this.taskStore.loadTaskStats();

    // React to errors using effect
    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.general', { error });
      }
    });
  }

  // Event handlers for task actions
  onTaskToggled(task: TaskViewModel): void {
    this.taskStore.toggleTaskCompletion(task.id);
  }

  onTaskEdited(task: TaskViewModel): void {
    this.router.navigate(['/tasks', task.id, 'edit']);
  }

  onTaskDuplicated(task: TaskViewModel): void {
    this.taskStore.duplicateTask(task.id);
    this.notificationService.notifySuccess('tasks.messages.duplicated', { name: task.title });
  }

  onTaskViewed(task: TaskViewModel): void {
    this.router.navigate(['/tasks', task.id]);
  }

  onTaskDeleted(task: TaskViewModel): void {
    const confirmationRef = this.confirmationService.open({
      title: 'Delete Task',
      message: `Are you sure you want to delete "${task.title}"? This action cannot be undone.`,
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
        this.taskStore.deleteTask(task.id);
        this.notificationService.notifySuccess('tasks.messages.deleted', { name: task.title });
      }
    });
  }

  // Event handlers for filters
  onFiltersChanged(filters: TaskFilters): void {
    this.taskStore.setFilters(filters);
    this.taskStore.loadTasks();
  }

  onSearchChanged(searchTerm: string): void {
    this.taskStore.searchTasks(searchTerm);
  }

  onFiltersCleared(): void {
    this.taskStore.clearFilters();
    this.taskStore.loadTasks();
  }

  // Navigation and view actions
  onNewTaskClick(): void {
    this.router.navigate(['/tasks/new']);
  }

  onViewModeChanged(viewMode: 'list' | 'grid' | 'kanban'): void {
    this.taskStore.setViewMode(viewMode);
  }

  // Pagination
  onPageChanged(page: number): void {
    this.taskStore.setPage(page);
    this.taskStore.loadTasks();
  }

  onNextPage(): void {
    if (this.hasNextPage()) {
      this.taskStore.setPage(this.currentPage() + 1);
      this.taskStore.loadTasks();
    }
  }

  onPreviousPage(): void {
    if (this.hasPreviousPage()) {
      this.taskStore.setPage(this.currentPage() - 1);
      this.taskStore.loadTasks();
    }
  }

  // Utility methods
  onRefresh(): void {
    this.taskStore.loadTasks();
  }

  onDismissError(): void {
    this.taskStore.clearError();
  }
}
