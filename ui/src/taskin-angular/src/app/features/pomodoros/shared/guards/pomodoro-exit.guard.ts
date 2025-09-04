import { CanDeactivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { map, take } from 'rxjs';
import { PomodoroStore } from '../stores/pomodoro.store';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';

export interface CanComponentDeactivate {
  canDeactivate(): boolean;
}

export const pomodoroExitGuard: CanDeactivateFn<CanComponentDeactivate> = component => {
  const confirmationService = inject(UiConfirmationService);

  if (component && component.canDeactivate()) {
    return confirmationService
      .open({
        title: 'Sesión Activa',
        message:
          'Tienes una sesión de pomodoro activa. ¿Estás seguro de que quieres salir? Tu timer será reiniciado.',
        icon: {
          show: true,
          name: 'timer',
          color: 'warning',
        },
        actions: {
          confirm: {
            show: true,
            label: 'Salir y reiniciar',
            color: 'warn',
          },
          cancel: {
            show: true,
            label: 'Continuar aquí',
          },
        },
        dismissible: true,
      })
      .afterClosed()
      .pipe(
        take(1),
        map(result => {
          return result === 'confirmed';
        })
      );
  }

  return true;
};
