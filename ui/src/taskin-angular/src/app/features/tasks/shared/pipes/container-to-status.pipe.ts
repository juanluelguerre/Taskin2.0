import { Pipe, PipeTransform } from '@angular/core';
import { TaskStatus } from '../types/task.types';

@Pipe({
  name: 'containerToStatus',
  standalone: true
})
export class ContainerToStatusPipe implements PipeTransform {
  transform(containerId: string): TaskStatus | null {
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