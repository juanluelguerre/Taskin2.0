import { ChangeDetectionStrategy, Component, ViewEncapsulation, OnInit, inject, computed, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { TaskStore, TaskViewModel, TaskStatus } from '../../shared';

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
    MatCardModule
  ],
  templateUrl: './task-details.component.html',
  styles: `
    .task-details-container {
      max-width: 900px;
      margin: 0 auto;
      padding: 24px;
    }
    
    .task-header {
      margin-bottom: 32px;
    }
    
    .task-content {
      display: grid;
      grid-template-columns: 1fr;
      gap: 24px;
    }
    
    @media (min-width: 768px) {
      .task-content {
        grid-template-columns: 2fr 1fr;
      }
    }
    
    .info-section {
      display: grid;
      gap: 16px;
    }
    
    .info-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 0;
      border-bottom: 1px solid #e5e7eb;
    }
    
    .info-item:last-child {
      border-bottom: none;
    }
    
    .info-label {
      min-width: 120px;
      font-weight: 500;
      color: #6b7280;
    }
    
    .info-value {
      flex: 1;
    }
    
    .progress-section {
      margin: 24px 0;
    }
    
    .progress-bar-container {
      margin: 12px 0;
    }
    
    .pomodoro-stats {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 12px;
      margin-top: 16px;
    }
    
    .stat-card {
      text-align: center;
      padding: 16px;
      background: #f8fafc;
      border-radius: 8px;
    }
    
    .stat-number {
      font-size: 1.5rem;
      font-weight: bold;
      color: #1f2937;
    }
    
    .stat-label {
      font-size: 0.875rem;
      color: #6b7280;
      margin-top: 4px;
    }
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TaskStore]
})
export class TaskDetailsComponent implements OnInit {
  // Dependencies
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly taskStore = inject(TaskStore);
  private readonly snackBar = inject(MatSnackBar);

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
        this.loadTask(id);
      }
    });

    // Handle errors
    this.taskStore.error.subscribe(error => {
      if (error) {
        this.snackBar.open(error, 'Dismiss', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
      }
    });
  }

  private loadTask(id: string): void {
    // For now, we'll find the task from the store
    // In a real application, this would call taskStore.loadTask(id)
    this.taskStore.refreshTasks();
    
    // Simulate finding the task in the store
    setTimeout(() => {
      const tasks = this.taskStore.taskViewModels();
      const foundTask = tasks.find(t => t.id === id);
      if (foundTask) {
        // Set the selected task in the store
        // Note: We'd need to add a setSelectedTask method to the store
        console.log('Found task:', foundTask);
      } else {
        this.router.navigate(['/404']);
      }
    }, 500);
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
      this.snackBar.open('Task duplicated successfully', 'Dismiss', {
        duration: 3000,
        horizontalPosition: 'right',
        verticalPosition: 'top'
      });
    }
  }

  onDeleteTask(): void {
    const currentTask = this.task();
    if (currentTask) {
      if (confirm(`Are you sure you want to delete "${currentTask.title}"?`)) {
        this.taskStore.deleteTask(currentTask.id);
        this.snackBar.open('Task deleted successfully', 'Dismiss', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
        this.router.navigate(['/tasks']);
      }
    }
  }

  onStartPomodoro(): void {
    const currentTask = this.task();
    if (currentTask) {
      // This would integrate with a pomodoro timer component
      this.snackBar.open('Starting Pomodoro timer...', 'Dismiss', {
        duration: 2000
      });
    }
  }

  onBackToList(): void {
    this.router.navigate(['/tasks']);
  }
}
