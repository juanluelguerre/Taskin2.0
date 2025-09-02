import { Pipe, PipeTransform } from '@angular/core';
import { TaskPriority } from '../../features/tasks/shared/types/task.types';

@Pipe({
  name: 'taskPriorityDisplay',
  standalone: true
})
export class TaskPriorityDisplayPipe implements PipeTransform {
  transform(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Low:
        return 'Low';
      case TaskPriority.Medium:
        return 'Medium';
      case TaskPriority.High:
        return 'High';
      case TaskPriority.Critical:
        return 'Critical';
      default:
        return 'Unknown';
    }
  }
}