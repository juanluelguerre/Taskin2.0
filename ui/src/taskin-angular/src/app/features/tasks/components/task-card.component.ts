import { Component, ChangeDetectionStrategy, input, output } from '@angular/core'
import { CommonModule, TitleCasePipe, UpperCasePipe } from '@angular/common'
import { CdkDragHandle } from '@angular/cdk/drag-drop'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { MatMenuModule } from '@angular/material/menu'
import { MatCheckboxModule } from '@angular/material/checkbox'
import { MatChipsModule } from '@angular/material/chips'
import { MatTooltipModule } from '@angular/material/tooltip'
import { TaskViewModel } from '../shared/types/task.types'

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [
    CommonModule,
    TitleCasePipe,
    UpperCasePipe,
    CdkDragHandle,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './task-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskCardComponent {
  // Inputs
  task = input.required<TaskViewModel>()
  
  // Outputs
  taskToggled = output<TaskViewModel>()
  taskEdited = output<TaskViewModel>()
  taskDuplicated = output<TaskViewModel>()
  taskViewed = output<TaskViewModel>()
  taskDeleted = output<TaskViewModel>()
}