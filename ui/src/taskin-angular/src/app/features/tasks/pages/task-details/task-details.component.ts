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
  ],
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.scss',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TaskStore],
})
export class TaskDetailsComponent implements OnInit {
  // Dependencies
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly taskStore = inject(TaskStore);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);

  // Local state
  private readonly taskId = signal<string | null>(null);

  // Store selectors
  readonly task = this.taskStore.selectedTask;
  readonly loading = this.taskStore.loading;
  readonly saving = this.taskStore.saving;
  readonly error = this.taskStore.error;

  // Computed properties
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

  // Enum reference for template
  readonly TaskStatus = TaskStatus;

  ngOnInit(): void {
    // Get task ID from route
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.taskId.set(id);
        this.taskStore.loadTask(id);
      }
    });

    // Handle errors using effect
    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.loadFailed');
      }
    });
  }

  // Action methods
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
      // This would integrate with a pomodoro timer component
      this.notificationService.notifyInfo('tasks.messages.pomodoroStarted');
      // TODO: Navigate to pomodoro timer or start timer here
    }
  }

  onBackToList(): void {
    this.router.navigate(['/tasks']);
  }
}
