import { Injectable, inject } from '@angular/core';
import {
  MatSnackBar,
  MatSnackBarConfig,
  MatSnackBarHorizontalPosition,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';
import { TranslocoService } from '@jsverse/transloco';

/**
 * NotificationService
 *
 * Thin wrapper around Angular Material's MatSnackBar with i18n support
 * (Transloco) and immutable per-call configuration. Intended for showing
 * short, transient feedback messages across the app.
 *
 * Usage examples:
 *
 * const notify = inject(NotificationService);
 * notify.notifySuccess('messages.saved');
 * notify.notifyError(); // falls back to 'messages.error'
 * notify.notifyWarning('projects.limitReached', { max: 10 });
 *
 * Styling expectations:
 * - Provide CSS for panel classes: 'snackbar-success', 'snackbar-warning',
 *   'snackbar-warn', and optionally 'snackbar-info'.
 */

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  // Default snackbar placement and lifetime
  private readonly _horizontalPosition: MatSnackBarHorizontalPosition = 'center';
  private readonly _verticalPosition: MatSnackBarVerticalPosition = 'bottom';
  private readonly _durationMs = 3000;

  private readonly _snackBar = inject(MatSnackBar);
  private readonly _translateService = inject(TranslocoService);

  // Base, immutable default config used for every call
  private readonly _defaultConfig: MatSnackBarConfig = {
    horizontalPosition: this._horizontalPosition,
    verticalPosition: this._verticalPosition,
    duration: this._durationMs,
  };

  /**
   * Show an error notification.
   * If `message` is omitted, it falls back to i18n key `messages.error`.
   */
  notifyError(message?: string, params?: Record<string, unknown>): void {
    const text = message
      ? this._translateService.translate(message, params)
      : this._translateService.translate('messages.error');

    this._open(text, 'snackbar-warn');
  }

  /** Show a success notification. */
  notifySuccess(message: string, params?: Record<string, unknown>): void {
    const text = this._translateService.translate(message, params);
    this._open(text, 'snackbar-success');
  }

  /** Show a warning notification. */
  notifyWarning(message: string, params?: Record<string, unknown>): void {
    const text = this._translateService.translate(message, params);
    this._open(text, 'snackbar-warning');
  }

  /** Show an informational notification. */
  notifyInfo(message: string, params?: Record<string, unknown>): void {
    const text = this._translateService.translate(message, params);
    this._open(text, 'snackbar-info');
  }

  /** Dismiss the active notification (if any). */
  dismiss(): void {
    this._snackBar.dismiss();
  }

  /**
   * Internal helper to open the snackbar with immutable defaults.
   * Accepts optional MatSnackBarConfig overrides per call without mutating
   * shared state.
   */
  private _open(message: string, panelClass: string | string[], config?: MatSnackBarConfig): void {
    const merged: MatSnackBarConfig = {
      ...this._defaultConfig,
      ...config,
      panelClass,
    };
    this._snackBar.open(message, '', merged);
  }
}
