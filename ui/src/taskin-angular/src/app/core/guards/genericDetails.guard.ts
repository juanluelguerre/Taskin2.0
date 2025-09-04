import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanDeactivateFn,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { Observable, map, take } from 'rxjs';

export interface IGenericDetailsComponent {
  closeDrawer(): Observable<boolean>;
  canDeactivate: () => boolean;
}

export function canDeactivateDetails(baseRoute: string): CanDeactivateFn<IGenericDetailsComponent> {
  return (
    component: IGenericDetailsComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState: RouterStateSnapshot
  ) => {
    if (component && !component.canDeactivate()) {
      const confirmationService = inject(UiConfirmationService);
      const translocoService = inject(TranslocoService);

      return confirmationService
        .open({
          title: translocoService.translate('messages.unsavedChangesTitle'),
          message: translocoService.translate('messages.unsavedChangesMessage'),
          actions: {
            confirm: {
              label: translocoService.translate('boolean.yes'),
              color: 'primary',
            },
            cancel: {
              label: translocoService.translate('boolean.no'),
            },
          },
          dismissible: true,
        })
        .afterClosed()
        .pipe(
          take(1),
          map(result => {
            if (result !== 'confirmed') {
              return false;
            }

            component.closeDrawer();
            return true;
          }),
          map(data => {
            return data;
          })
        );
    }

    let nextRoute: ActivatedRouteSnapshot = nextState.root;
    while (nextRoute.firstChild) {
      nextRoute = nextRoute.firstChild;
    }

    if (!nextState.url.includes(baseRoute)) {
      return true;
    }

    if (nextRoute.paramMap.get('id')) {
      return true;
    } else {
      return component.closeDrawer();
    }
  };
}

export type CanDeactivateType =
  | Observable<boolean | UrlTree>
  | Promise<boolean | UrlTree>
  | boolean
  | UrlTree;

export interface CanComponentDeactivate {
  canDeactivate: () => CanDeactivateType;
}

export const canDeactivateGuard: CanDeactivateFn<CanComponentDeactivate> = (
  component: CanComponentDeactivate
) => {
  if (component && !component.canDeactivate()) {
    const confirmationService = inject(UiConfirmationService);
    const translocoService = inject(TranslocoService);

    return confirmationService
      .open({
        title: translocoService.translate('messages.unsavedChangesTitle'),
        message: translocoService.translate('messages.unsavedChangesMessage'),
        actions: {
          confirm: {
            label: translocoService.translate('boolean.yes'),
            color: 'primary',
          },
          cancel: {
            label: translocoService.translate('boolean.no'),
          },
        },
        dismissible: true,
      })
      .afterClosed()
      .pipe(
        take(1),
        map(result => {
          if (result !== 'confirmed') {
            return false;
          }

          return true;
        }),
        map(data => {
          return data;
        })
      );
  }

  return true;
};
