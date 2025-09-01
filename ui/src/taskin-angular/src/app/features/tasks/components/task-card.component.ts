import { Component, ChangeDetectionStrategy, input, output } from '@angular/core'
import { CommonModule, TitleCasePipe, UpperCasePipe } from '@angular/common'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { MatMenuModule } from '@angular/material/menu'
import { MatCheckboxModule } from '@angular/material/checkbox'
import { MatChipsModule } from '@angular/material/chips'
import { TaskViewModel } from '../shared/types/task.types'

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [
    CommonModule,
    TitleCasePipe,
    UpperCasePipe,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatChipsModule
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