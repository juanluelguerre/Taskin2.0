import { CdkDrag, CdkDragStart } from '@angular/cdk/drag-drop';
import {
  Component,
  EventEmitter,
  Output,
  TemplateRef,
  ViewEncapsulation,
  inject,
} from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Dialog, DialogRef, DialogModule } from '@angular/cdk/dialog';
import { Subscription } from 'rxjs';

import { AppSettings, SettingsService } from '@core';
import { DisableControlDirective } from '@shared';

@Component({
  selector: 'app-customizer',
  templateUrl: './customizer.component.html',
  styleUrl: './customizer.component.scss',
  encapsulation: ViewEncapsulation.None,
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CdkDrag,
    MatButtonModule,
    MatDividerModule,
    MatIconModule,
    MatRadioModule,
    MatSlideToggleModule,
    MatTooltipModule,
    DialogModule,
    DisableControlDirective,
  ],
})
export class CustomizerComponent {
  @Output() optionsChange = new EventEmitter<AppSettings>();

  private readonly settings = inject(SettingsService);
  private readonly dialog = inject(Dialog);
  private readonly fb = inject(FormBuilder);

  form = this.fb.nonNullable.group<AppSettings>(this.settings.options);

  private formSubscription = Subscription.EMPTY;

  get isHeaderPosAbove() {
    return this.form.get('headerPos')?.value === 'above';
  }

  get isNavPosTop() {
    return this.form.get('navPos')?.value === 'top';
  }

  get isShowHeader() {
    return this.form.get('showHeader')?.value === true;
  }

  private dragging = false;

  private dialogRef?: DialogRef;

  onDragStart(event: CdkDragStart) {
    this.dragging = true;
  }

  openPanel(templateRef: TemplateRef<any>) {
    if (this.dragging) {
      this.dragging = false;
      return;
    }

    this.dialogRef = this.dialog.open(templateRef, {
      panelClass: 'customizer-drawer-panel',
      hasBackdrop: true,
      width: '320px',
    });

    this.dialogRef.opened.subscribe(() => {
      this.formSubscription = this.form.valueChanges.subscribe(value => {
        this.sendOptions(this.form.getRawValue());
      });
    });

    this.dialogRef.closed.subscribe(() => {
      this.formSubscription.unsubscribe();
    });
  }

  closePanel() {
    this.dialogRef?.close();
  }

  sendOptions(options: AppSettings) {
    this.optionsChange.emit(options);
  }
}
