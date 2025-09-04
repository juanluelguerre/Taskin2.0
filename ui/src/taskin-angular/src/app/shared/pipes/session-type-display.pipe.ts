import { Pipe, PipeTransform } from '@angular/core';
import { PomodoroType } from '../../features/pomodoros/shared/types/pomodoro.types';

@Pipe({
  name: 'sessionTypeDisplay',
  standalone: true
})
export class SessionTypeDisplayPipe implements PipeTransform {
  transform(type: PomodoroType): string {
    switch (type) {
      case PomodoroType.Work:
        return 'Work Session';
      case PomodoroType.ShortBreak:
        return 'Short Break';
      case PomodoroType.LongBreak:
        return 'Long Break';
      default:
        return 'Unknown';
    }
  }
}