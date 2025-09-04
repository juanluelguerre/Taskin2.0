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
  signal,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { AssigneeService, Assignee } from '@core/services/assignee.service';
import { NotificationService } from '@core/services/notification.service';
import { ProjectService, ProjectListDto } from '../../../projects/shared/services/project.service';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { TaskCardComponent, TaskFiltersComponent, TaskStatsComponent } from '../../components';
import { TaskFilters, TaskStatus, TaskStore, TaskViewModel } from '../../shared';
import { ContainerToStatusPipe } from '../../shared/pipes/container-to-status.pipe';

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
  private readonly store = inject(TaskStore);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);
  private readonly projectService = inject(ProjectService);
  private readonly assigneeService = inject(AssigneeService);
  private readonly containerToStatusPipe = new ContainerToStatusPipe();

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

  readonly projects = signal<ProjectListDto[]>([]);
  readonly assignees = signal<Assignee[]>([]);

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

  readonly dropListIds = ['pending-list', 'in-progress-list', 'completed-list'];
  readonly TaskStatus = TaskStatus;

  constructor() {
    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.general', { error });
      }
    });
  }

  ngOnInit(): void {
    this.store.loadTasks();
    this.loadProjects();
    this.loadAssignees();
  }

  private loadProjects(): void {
    this.projectService.getProjectsForDropdown().subscribe({
      next: projects => this.projects.set(projects),
      error: error => {
        console.error('Error loading projects:', error);
        this.notificationService.notifyError('Failed to load projects');
      },
    });
  }

  private loadAssignees(): void {
    this.assigneeService.getAssigneesForDropdown().subscribe({
      next: assignees => this.assignees.set(assignees),
      error: error => {
        console.error('Error loading assignees:', error);
        this.notificationService.notifyError('Failed to load assignees');
      },
    });
  }

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

  onNewTaskClick(): void {
    this.router.navigate(['/tasks/new']);
  }

  onViewModeChanged(viewMode: 'list' | 'grid' | 'kanban'): void {
    this.store.setViewMode(viewMode);
  }

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

  onRefresh(): void {
    this.store.loadTasks();
  }

  onDismissError(): void {
    this.store.clearError();
  }

  onTaskDropped(event: CdkDragDrop<TaskViewModel[]>): void {
    if (event.previousContainer === event.container) {
      console.log('Same container, no status change needed');
      return;
    }

    const task = event.previousContainer.data[event.previousIndex];
    console.log('Task to update:', task);

    const newStatus = this.containerToStatusPipe.transform(event.container.id);
    console.log('New status:', newStatus);

    if (newStatus && task.status !== newStatus) {
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
}
