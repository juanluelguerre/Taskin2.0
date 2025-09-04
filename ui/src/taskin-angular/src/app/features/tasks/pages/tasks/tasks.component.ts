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
import { 
  CdkDragDrop, 
  CdkDropList, 
  CdkDrag, 
  CdkDragPreview,
  moveItemInArray, 
  transferArrayItem 
} from '@angular/cdk/drag-drop';
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
  styles: `
    .loading-bar {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      z-index: 10;
    }

    /* Drag & Drop Styles */
    .cdk-drop-list {
      transition: background-color 250ms ease;
    }

    .cdk-drop-list.cdk-drop-list-receiving {
      background-color: rgba(59, 130, 246, 0.05);
      border-radius: 8px;
    }

    .cdk-drag {
      transition: transform 250ms ease;
      cursor: grab;
    }

    .cdk-drag:active {
      cursor: grabbing;
    }

    .cdk-drag.cdk-drag-animating {
      transition: transform 300ms cubic-bezier(0, 0, 0.2, 1);
    }

    .cdk-drag-preview {
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 
                  0 10px 10px -5px rgba(0, 0, 0, 0.04);
      border: 2px solid #3b82f6;
      transform: rotate(5deg);
      z-index: 1000;
    }

    .cdk-drag-placeholder {
      opacity: 0.3;
      border: 2px dashed #d1d5db;
      background-color: #f9fafb;
      border-radius: 8px;
    }

    .cdk-drop-list-dragging .cdk-drag {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }

    /* Kanban column hover effects */
    .kanban-column {
      position: relative;
      transition: all 200ms ease;
    }

    .kanban-column:hover {
      transform: translateY(-2px);
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 
                  0 4px 6px -2px rgba(0, 0, 0, 0.05);
    }

    /* Empty state styling */
    .empty-kanban-column {
      border: 2px dashed #e5e7eb;
      background-color: #f9fafb;
      border-radius: 8px;
      transition: all 200ms ease;
    }

    .empty-kanban-column.cdk-drop-list-receiving {
      border-color: #3b82f6;
      background-color: rgba(59, 130, 246, 0.05);
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
    () => this.tasks().filter(t => t.status === TaskStatus.Cancelled || (t.status as any) === 'cancelled').length
  );

  // Computed tasks by status for kanban board
  readonly tasksByStatus = computed(() => {
    const tasks = this.tasks();
    return {
      pending: tasks.filter(t => t.status === TaskStatus.Pending),
      inProgress: tasks.filter(t => t.status === TaskStatus.InProgress),
      completed: tasks.filter(t => t.status === TaskStatus.Completed),
      cancelled: tasks.filter(t => t.status === TaskStatus.Cancelled || (t.status as any) === 'cancelled')
    };
  });

  // Drop container IDs for kanban board
  readonly dropListIds = [
    'pending-list',
    'in-progress-list', 
    'completed-list',
    'cancelled-list'
  ];

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
    this.taskStore.refreshTasks();
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
        status: newStatus
      };
      
      console.log('Updating task with request:', updateRequest);
      this.taskStore.updateTask({ id: task.id, request: updateRequest });
      this.notificationService.notifySuccess('Task moved successfully', { 
        taskName: task.title, 
        newStatus: newStatus 
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
