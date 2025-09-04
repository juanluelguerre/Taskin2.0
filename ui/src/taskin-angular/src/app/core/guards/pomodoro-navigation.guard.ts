import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { PomodoroStore } from '../../features/pomodoros/shared/stores/pomodoro.store';

export const pomodoroNavigationGuard: CanActivateFn = (route, state) => {
  const pomodoroStore = inject(PomodoroStore);
  const timer = pomodoroStore.timer();
  
  // If we're navigating TO the pomodoros page, always allow
  if (state.url === '/pomodoros') {
    return true;
  }
  
  // If timer is running, prevent navigation to other pages
  if (timer.isRunning) {
    const confirmed = window.confirm(
      '¡Tienes un pomodoro activo en marcha! ¿Estás seguro de que quieres navegar a otra página? El timer se mantendrá corriendo en segundo plano.'
    );
    
    if (!confirmed) {
      return false;
    }
  }
  
  return true;
};