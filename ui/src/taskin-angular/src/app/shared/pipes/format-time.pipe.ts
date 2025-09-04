import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatTime',
  standalone: true
})
export class FormatTimePipe implements PipeTransform {
  transform(timeInSeconds: number | null | undefined, format: 'short' | 'long' = 'short'): string {
    if (timeInSeconds === null || timeInSeconds === undefined || timeInSeconds < 0) {
      return format === 'short' ? '00:00' : '0 minutes';
    }

    const hours = Math.floor(timeInSeconds / 3600);
    const minutes = Math.floor((timeInSeconds % 3600) / 60);
    const seconds = timeInSeconds % 60;

    if (format === 'short') {
      if (hours > 0) {
        return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
      }
      return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    } else {
      // Long format
      const parts: string[] = [];
      if (hours > 0) {
        parts.push(`${hours} ${hours === 1 ? 'hour' : 'hours'}`);
      }
      if (minutes > 0) {
        parts.push(`${minutes} ${minutes === 1 ? 'minute' : 'minutes'}`);
      }
      if (seconds > 0 && hours === 0) {
        parts.push(`${seconds} ${seconds === 1 ? 'second' : 'seconds'}`);
      }
      
      return parts.length > 0 ? parts.join(' ') : '0 minutes';
    }
  }
}