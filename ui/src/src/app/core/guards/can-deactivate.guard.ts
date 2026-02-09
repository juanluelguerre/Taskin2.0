import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { map, take } from 'rxjs';

export interface CanComponentDeactivate {
  canDeactivate: () => boolean;
}

export const canDeactivateGuard: CanDeactivateFn<CanComponentDeactivate> = (component) => {
  if (component && !component.canDeactivate()) {
    const confirmationService = inject(UiConfirmationService);
    const t = inject(TranslocoService);

    return confirmationService
      .open({
        title: t.translate('guards.unsavedChangesTitle'),
        message: t.translate('guards.unsavedChangesMessage'),
        icon: { show: true, name: 'warning', color: 'warn' },
        actions: {
          confirm: { show: true, label: t.translate('common.yes'), color: 'warn' },
          cancel: { show: true, label: t.translate('common.cancel') },
        },
        dismissible: true,
      })
      .afterClosed()
      .pipe(
        take(1),
        map((result) => result === 'confirmed'),
      );
  }
  return true;
};
