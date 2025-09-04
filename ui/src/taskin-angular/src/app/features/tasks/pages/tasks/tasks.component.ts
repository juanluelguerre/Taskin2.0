import { CdkDrag, CdkDragDrop, CdkDragPreview, CdkDropList } from '@angular/cdk/drag-drop';
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
    CdkDropList,
    CdkDrag,
    CdkDragPreview,
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
  styleUrls: ['./tasks.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TasksComponent implements OnInit {
  // Dependencies
  private readonly store = inject(TaskStore);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);

  // Store selectors - exposed as signals
  readonly tasks = this.store.taskViewModels;
  readonly stats = this.store.taskStatistics;
  readonly loading = this.store.loading;
  readonly saving = this.store.saving;
  readonly error = this.store.error;
  readonly filters = this.store.filters;
  readonly searchTerm = this.store.searchTerm;
  readonly viewMode = this.store.viewMode;
  readonly totalPages = this.store.totalPages;
  readonly currentPage = this.store.currentPage;
  readonly hasNextPage = this.store.hasNextPage;
  readonly hasPreviousPage = this.store.hasPreviousPage;

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
    () =>
      this.tasks().filter(
        t => t.status === TaskStatus.Cancelled || (t.status as any) === 'cancelled'
      ).length
  );

  // Computed tasks by status for kanban board
  readonly tasksByStatus = computed(() => {
    const tasks = this.tasks();
    return {
      pending: tasks.filter(t => t.status === TaskStatus.Pending),
      inProgress: tasks.filter(t => t.status === TaskStatus.InProgress),
      completed: tasks.filter(t => t.status === TaskStatus.Completed),
      cancelled: tasks.filter(
        t => t.status === TaskStatus.Cancelled || (t.status as any) === 'cancelled'
      ),
    };
  });

  // Drop container IDs for kanban board
  readonly dropListIds = ['pending-list', 'in-progress-list', 'completed-list', 'cancelled-list'];

  // Enum references for template use
  readonly TaskStatus = TaskStatus;

  constructor() {
    // React to errors using effect
    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.general', { error });
      }
    });
  }

  ngOnInit(): void {
    // Load initial data
    this.store.refreshTasks();
    this.store.loadTasks();
  }

  // Event handlers for task actions
  onTaskToggled(task: TaskViewModel): void {
    this.store.toggleTaskCompletion(task.id);
  }

  onTaskEdited(task: TaskViewModel): void {
    this.router.navigate(['/tasks', task.id, 'edit']);
  }

  onTaskDuplicated(task: TaskViewModel): void {
    this.store.duplicateTask(task.id);
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
        this.store.deleteTask(task.id);
        this.notificationService.notifySuccess('tasks.messages.deleted', { name: task.title });
      }
    });
  }

  // Event handlers for filters
  onFiltersChanged(filters: TaskFilters): void {
    this.store.setFilters(filters);
    this.store.loadTasks();
  }

  onSearchChanged(searchTerm: string): void {
    this.store.searchTasks(searchTerm);
  }

  onFiltersCleared(): void {
    this.store.clearFilters();
    this.store.loadTasks();
  }

  // Navigation and view actions
  onNewTaskClick(): void {
    this.router.navigate(['/tasks/new']);
  }

  onViewModeChanged(viewMode: 'list' | 'grid' | 'kanban'): void {
    this.store.setViewMode(viewMode);
  }

  // Pagination
  onPageChanged(page: number): void {
    this.store.setPage(page);
    this.store.loadTasks();
  }

  onNextPage(): void {
    if (this.hasNextPage()) {
      this.store.setPage(this.currentPage() + 1);
      this.store.loadTasks();
    }
  }

  onPreviousPage(): void {
    if (this.hasPreviousPage()) {
      this.store.setPage(this.currentPage() - 1);
      this.store.loadTasks();
    }
  }

  // Utility methods
  onRefresh(): void {
    this.store.loadTasks();
  }

  onDismissError(): void {
    this.store.clearError();
  }

  // Drag & Drop handlers
  onTaskDropped(event: CdkDragDrop<TaskViewModel[]>): void {
    console.log('Task dropped:', event);
    console.log('Previous container:', event.previousContainer.id);
    console.log('Current container:', event.container.id);
    console.log('Previous index:', event.previousIndex);
    console.log('Current index:', event.currentIndex);

    if (event.previousContainer === event.container) {
      // Reordering within the same column - no status change needed
      console.log('Same container, no status change needed');
      return;
    }

    // Task moved between columns - update status
    const task = event.previousContainer.data[event.previousIndex];
    console.log('Task to update:', task);

    const newStatus = this.getStatusFromContainerId(event.container.id);
    console.log('New status:', newStatus);

    if (newStatus && task.status !== newStatus) {
      // Create update request
      const updateRequest = {
        id: task.id,
        status: newStatus,
      };

      console.log('Updating task with request:', updateRequest);
      this.store.updateTask({ id: task.id, request: updateRequest });
      this.notificationService.notifySuccess('Task moved successfully', {
        taskName: task.title,
        newStatus: newStatus,
      });
    }
  }

  private getStatusFromContainerId(containerId: string): TaskStatus | null {
    switch (containerId) {
      case 'pending-list':
        return TaskStatus.Pending;
      case 'in-progress-list':
        return TaskStatus.InProgress;
      case 'completed-list':
        return TaskStatus.Completed;
      case 'cancelled-list':
        return TaskStatus.Cancelled;
      default:
        return null;
    }
  }
}
