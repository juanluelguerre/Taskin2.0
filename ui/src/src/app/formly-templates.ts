import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { FieldTypeConfig } from '@ngx-formly/core';
import { FieldType } from '@ngx-formly/material/form-field';

/**
 * This is just an example.
 */
@Component({
  selector: 'formly-field-combobox',
  template: `<mat-select
    [formControl]="formControl"
    [multiple]="props.multiple"
    [placeholder]="props.placeholder!"
    [required]="props.required!"
    [compareWith]="props.compareWith"
  >
    @for (option of props.options; track option) {
      <mat-option [value]="bindValue ? option[bindValue] : option">
        {{ bindLabel ? option[bindLabel] : option }}
      </mat-option>
    }
  </mat-select>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [ReactiveFormsModule, MatSelectModule, MatOptionModule],
})
export class FormlyFieldComboboxComponent extends FieldType<FieldTypeConfig> {
  get bindLabel(): string {
    return typeof this.props.labelProp === 'string' ? this.props.labelProp : '';
  }

  get bindValue(): string | undefined {
    return typeof this.props.valueProp === 'string' ? this.props.valueProp : undefined;
  }
}
