import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TaskPriorityDisplayPipe } from '@shared/pipes/task-priority-display.pipe';
import { TaskStatusDisplayPipe } from '@shared/pipes/task-status-display.pipe';
import { TaskViewModel } from '../shared/types/task.types';
import { TaskStore } from '../shared/stores/task.store';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [
    CommonModule,
    CdkDragHandle,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatChipsModule,
    MatTooltipModule,
    TaskStatusDisplayPipe,
    TaskPriorityDisplayPipe,
  ],
  templateUrl: './task-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskCardComponent {
  // Inputs
  task = input.required<TaskViewModel>();
  viewMode = input<'list' | 'grid' | 'kanban'>('list');

  // Outputs
  taskToggled = output<TaskViewModel>();
  taskEdited = output<TaskViewModel>();
  taskDuplicated = output<TaskViewModel>();
  taskViewed = output<TaskViewModel>();
  taskDeleted = output<TaskViewModel>();

  getStatusIcon(status: number): string {
    switch (status) {
      case 0: // Pending
        return 'pending';
      case 1: // InProgress
        return 'work';
      case 2: // Completed
        return 'check_circle';
      case 3: // Cancelled
        return 'cancel';
      default:
        return 'pending';
    }
  }
}
