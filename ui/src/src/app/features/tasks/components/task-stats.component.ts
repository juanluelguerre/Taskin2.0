import { Component, ChangeDetectionStrategy, input } from '@angular/core'

import { MatIconModule } from '@angular/material/icon'
import { TranslocoModule } from '@jsverse/transloco'
import { TaskStatsViewModel } from '../shared/types/task.types'

@Component({
  selector: 'app-task-stats',
  standalone: true,
  imports: [
    MatIconModule,
    TranslocoModule
],
  templateUrl: './task-stats.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskStatsComponent {
  // Inputs
  stats = input.required<TaskStatsViewModel>()
}