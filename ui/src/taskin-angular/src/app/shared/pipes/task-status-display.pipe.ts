import { Pipe, PipeTransform } from '@angular/core';
import { TaskStatus } from '../../features/tasks/shared/types/task.types';

@Pipe({
  name: 'taskStatusDisplay',
  standalone: true
})
export class TaskStatusDisplayPipe implements PipeTransform {
  transform(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Pending:
        return 'Pending';
      case TaskStatus.InProgress:
        return 'In Progress';
      case TaskStatus.Completed:
        return 'Completed';
      case TaskStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }
}