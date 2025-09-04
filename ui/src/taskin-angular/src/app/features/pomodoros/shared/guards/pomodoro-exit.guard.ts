import { CanDeactivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { PomodoroStore } from '../stores/pomodoro.store';

export interface CanComponentDeactivate {
  canDeactivate?(): boolean;
}

export const pomodoroExitGuard: CanDeactivateFn<CanComponentDeactivate> = (component) => {
  const store = inject(PomodoroStore);
  const timer = store.timer();
  
  // If timer is running or paused, show confirmation
  if (timer.isRunning || timer.isPaused) {
    return window.confirm(
      'You have an active pomodoro session. Are you sure you want to leave? Your timer will be reset.'
    );
  }
  
  return true;
};