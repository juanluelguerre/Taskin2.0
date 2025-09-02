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
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '@core/services/notification.service';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { TaskStatus, TaskStore } from '../../shared';
import { TaskStatusDisplayPipe } from '@shared/pipes/task-status-display.pipe';
import { TaskPriorityDisplayPipe } from '@shared/pipes/task-priority-display.pipe';

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatMenuModule,
    MatDividerModule,
    MatCardModule,
    TaskStatusDisplayPipe,
    TaskPriorityDisplayPipe,
  ],
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.scss',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TaskStore],
})
export class TaskDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly taskStore = inject(TaskStore);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);
  private readonly taskId = signal<string | null>(null);

  readonly task = this.taskStore.selectedTask;
  readonly loading = this.taskStore.loading;
  readonly saving = this.taskStore.saving;
  readonly error = this.taskStore.error;

  readonly isOverdue = computed(() => {
    const currentTask = this.task();
    if (!currentTask?.dueDate) return false;
    return new Date(currentTask.dueDate) < new Date() && !currentTask.isCompleted;
  });

  readonly daysUntilDue = computed(() => {
    const currentTask = this.task();
    if (!currentTask?.dueDate) return undefined;
    const now = new Date();
    const dueDate = new Date(currentTask.dueDate);
    const diffTime = dueDate.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  });

  readonly completionPercentage = computed(() => {
    const currentTask = this.task();
    if (!currentTask?.estimatedPomodoros) return 0;
    const completed = currentTask.completedPomodoros || 0;
    return Math.round((completed / currentTask.estimatedPomodoros) * 100);
  });

  readonly dueDateDisplayText = computed(() => {
    const currentTask = this.task();
    if (!currentTask?.dueDate) return null;

    const days = this.daysUntilDue();
    if (days === undefined) return null;

    if (this.isOverdue()) return 'Overdue';
    if (days === 0) return 'Due today';
    if (days === 1) return 'Due tomorrow';
    if (days > 0) return `${days} days left`;
    return `${Math.abs(days)} days overdue`;
  });

  readonly statusColor = computed(() => {
    const currentTask = this.task();
    if (!currentTask) return 'text-gray-600 bg-gray-100';
    return this.getStatusColor(currentTask.status);
  });

  readonly priorityColor = computed(() => {
    const currentTask = this.task();
    if (!currentTask) return 'text-gray-600 bg-gray-100';
    return this.getPriorityColor(currentTask.priority);
  });

  readonly TaskStatus = TaskStatus;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.taskId.set(id);
        this.taskStore.loadTask(id);
      }
    });

    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.loadFailed');
      }
    });
  }

  onToggleCompletion(): void {
    const currentTask = this.task();
    if (currentTask) {
      this.taskStore.toggleTaskCompletion(currentTask.id);
    }
  }

  onEditTask(): void {
    const currentTask = this.task();
    if (currentTask) {
      this.router.navigate(['/tasks', currentTask.id, 'edit']);
    }
  }

  onDuplicateTask(): void {
    const currentTask = this.task();
    if (currentTask) {
      this.taskStore.duplicateTask(currentTask.id);
      this.notificationService.notifySuccess('tasks.messages.duplicated', {
        name: currentTask.title,
      });
    }
  }

  onDeleteTask(): void {
    const currentTask = this.task();
    if (!currentTask) return;

    const confirmationRef = this.confirmationService.open({
      title: 'Delete Task',
      message: `Are you sure you want to delete "${currentTask.title}"? This action cannot be undone.`,
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
      if (result === 'confirmed' && currentTask) {
        this.taskStore.deleteTask(currentTask.id);
        this.notificationService.notifySuccess('tasks.messages.deleted', {
          name: currentTask.title,
        });
        this.router.navigate(['/tasks']);
      }
    });
  }

  onStartPomodoro(): void {
    const currentTask = this.task();
    if (currentTask) {
      this.notificationService.notifyInfo('tasks.messages.pomodoroStarted');
    }
  }

  onBackToList(): void {
    this.router.navigate(['/tasks']);
  }

  private getStatusColor(status: any): string {
    switch (status) {
      case TaskStatus.Pending:
        return 'text-gray-600 bg-gray-100';
      case TaskStatus.InProgress:
        return 'text-blue-600 bg-blue-100';
      case TaskStatus.Completed:
        return 'text-green-600 bg-green-100';
      case TaskStatus.Cancelled:
        return 'text-red-600 bg-red-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  }

  private getPriorityColor(priority: any): string {
    switch (priority) {
      case 'Low':
        return 'text-gray-600 bg-gray-100';
      case 'Medium':
        return 'text-yellow-600 bg-yellow-100';
      case 'High':
        return 'text-orange-600 bg-orange-100';
      case 'Critical':
        return 'text-red-600 bg-red-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  }

}
