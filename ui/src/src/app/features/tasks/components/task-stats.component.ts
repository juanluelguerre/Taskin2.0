import { Component, ChangeDetectionStrategy, input } from '@angular/core'
import { CommonModule } from '@angular/common'
import { MatIconModule } from '@angular/material/icon'
import { TaskStatsViewModel } from '../shared/types/task.types'

@Component({
  selector: 'app-task-stats',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule
  ],
  templateUrl: './task-stats.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskStatsComponent {
  // Inputs
  stats = input.required<TaskStatsViewModel>()
}