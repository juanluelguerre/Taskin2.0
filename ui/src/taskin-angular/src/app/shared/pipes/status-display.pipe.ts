import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusDisplay',
  standalone: true
})
export class StatusDisplayPipe implements PipeTransform {
  transform(status: string, type: 'project' | 'task' = 'project'): string {
    if (!status) return status;
    
    const normalizedStatus = status.toLowerCase();
    
    if (type === 'project') {
      switch (normalizedStatus) {
        case 'onhold': return 'On Hold';
        default: return this.capitalize(status);
      }
    } else { // task
      switch (normalizedStatus) {
        case 'inprogress': return 'In Progress';
        case 'pending': return 'Pending';
        case 'completed': return 'Completed';
        case 'cancelled': return 'Cancelled';
        default: return this.capitalize(status);
      }
    }
  }

  private capitalize(text: string): string {
    return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
  }
}