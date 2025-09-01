import { Component, ViewEncapsulation, inject } from '@angular/core'
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { CommonModule, NgClass } from '@angular/common'
import { UiConfirmationConfig } from './confirmation.types'

@Component({
    selector: 'ui-confirmation-dialog',
    standalone: true,
    imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, NgClass],
    templateUrl: './confirmation-dialog.component.html',
    styleUrls: ['./confirmation-dialog.component.scss'],
    encapsulation: ViewEncapsulation.None,
})
export class UiConfirmationDialogComponent {
    data: UiConfirmationConfig = inject(MAT_DIALOG_DATA)
}

