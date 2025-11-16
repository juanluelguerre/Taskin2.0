import { Injectable, inject } from '@angular/core'
import { MatDialog, MatDialogRef } from '@angular/material/dialog'
import { UiConfirmationConfig, UiConfirmationResult } from './confirmation.types'
import { UiConfirmationDialogComponent } from './confirmation-dialog.component'

@Injectable({ providedIn: 'root' })
export class UiConfirmationService {
  private _matDialog = inject(MatDialog)

  private _defaultConfig: UiConfirmationConfig = {
    title: 'Confirm action',
    message: 'Are you sure you want to confirm this action?',
    icon: {
      show: true,
      name: 'warning',
      color: 'warn',
    },
    actions: {
      confirm: {
        show: true,
        label: 'Confirm',
        color: 'warn',
      },
      cancel: {
        show: true,
        label: 'Cancel',
      },
    },
    dismissible: false,
  }

  open(config: UiConfirmationConfig = {}): MatDialogRef<UiConfirmationDialogComponent, UiConfirmationResult> {
    const merged: UiConfirmationConfig = {
      ...this._defaultConfig,
      ...config,
      icon: { ...this._defaultConfig.icon, ...config.icon },
      actions: {
        confirm: { ...this._defaultConfig.actions?.confirm, ...config.actions?.confirm },
        cancel: { ...this._defaultConfig.actions?.cancel, ...config.actions?.cancel },
      },
    }

    return this._matDialog.open(UiConfirmationDialogComponent, {
      autoFocus: false,
      disableClose: !merged.dismissible,
      data: merged,
      panelClass: 'ui-confirmation-dialog-panel',
    })
  }
}

