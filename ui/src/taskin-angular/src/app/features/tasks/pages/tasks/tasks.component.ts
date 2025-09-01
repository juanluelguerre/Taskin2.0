import { ChangeDetectionStrategy, Component, ViewEncapsulation, inject, OnInit, effect, computed } from '@angular/core'
import { CommonModule } from '@angular/common'
import { Router } from '@angular/router'
import { MatButtonModule } from '@angular/material/button'
import { MatIconModule } from '@angular/material/icon'
import { MatProgressBarModule } from '@angular/material/progress-bar'
import { MatSnackBar } from '@angular/material/snack-bar'
import { MatTooltipModule } from '@angular/material/tooltip'
import { MatButtonToggleModule } from '@angular/material/button-toggle'
import { 
  TaskStore,
  TaskViewModel,
  TaskFilters,
  TaskStatsViewModel,
  TaskStatus
} from '../../shared'
import { TaskCardComponent, TaskStatsComponent, TaskFiltersComponent } from '../../components'

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatButtonToggleModule,
    TaskCardComponent,
    TaskStatsComponent,
    TaskFiltersComponent
  ],
  templateUrl: './tasks.component.html',
  styles: `
    .loading-bar {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      z-index: 10;
    }
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TaskStore]
})
export class TasksComponent implements OnInit {
  // Dependencies
  private readonly taskStore = inject(TaskStore)
  private readonly router = inject(Router)
  private readonly snackBar = inject(MatSnackBar)
  
  // Store selectors - exposed as signals
  readonly tasks = this.taskStore.taskViewModels
  readonly stats = this.taskStore.taskStatistics
  readonly loading = this.taskStore.loading
  readonly saving = this.taskStore.saving
  readonly error = this.taskStore.error
  readonly filters = this.taskStore.filters
  readonly searchTerm = this.taskStore.searchTerm
  readonly viewMode = this.taskStore.viewMode
  readonly totalPages = this.taskStore.totalPages
  readonly currentPage = this.taskStore.currentPage
  readonly hasNextPage = this.taskStore.hasNextPage
  readonly hasPreviousPage = this.taskStore.hasPreviousPage
  
  // Mock data for dropdowns (in real app, these would come from services)
  readonly projects = [
    { id: '1', name: 'Taskin 2.0' },
    { id: '2', name: 'E-commerce Platform' },
    { id: '3', name: 'Mobile App Redesign' },
    { id: '4', name: 'API Documentation' }
  ]
  
  readonly assignees = [
    { id: '1', name: 'John Doe' },
    { id: '2', name: 'Jane Smith' },
    { id: '3', name: 'Mike Johnson' },
    { id: '4', name: 'Sarah Wilson' }
  ]

  // Computed properties for template expressions
  readonly hasActiveFilters = computed(() => {
    const filters = this.filters()
    return Object.keys(filters).some(key => filters[key as keyof typeof filters])
  })

  readonly showingFromIndex = computed(() => 
    (this.currentPage() - 1) * 25 + 1
  )

  readonly showingToIndex = computed(() => 
    Math.min(this.currentPage() * 25, this.tasks().length)
  )

  readonly cancelledTasksCount = computed(() => 
    this.tasks().filter(t => t.status === 'cancelled').length
  )

  // Enum references for template use
  readonly TaskStatus = TaskStatus

  ngOnInit(): void {
    // Load initial data
    this.taskStore.refreshTasks()
    
    // React to errors using effect
    effect(() => {
      const error = this.error()
      if (error) {
        this.snackBar.open(error, 'Dismiss', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        })
      }
    })
  }
  
  // Event handlers for task actions
  onTaskToggled(task: TaskViewModel): void {
    this.taskStore.toggleTaskCompletion(task.id)
  }
  
  onTaskEdited(task: TaskViewModel): void {
    this.router.navigate(['/tasks', task.id, 'edit'])
  }
  
  onTaskDuplicated(task: TaskViewModel): void {
    this.taskStore.duplicateTask(task.id)
    this.snackBar.open('Task duplicated successfully', 'Dismiss', {
      duration: 3000,
      horizontalPosition: 'right',
      verticalPosition: 'top'
    })
  }
  
  onTaskViewed(task: TaskViewModel): void {
    this.router.navigate(['/tasks', task.id])
  }
  
  onTaskDeleted(task: TaskViewModel): void {
    if (confirm(`Are you sure you want to delete "${task.title}"?`)) {
      this.taskStore.deleteTask(task.id)
      this.snackBar.open('Task deleted successfully', 'Dismiss', {
        duration: 3000,
        horizontalPosition: 'right',
        verticalPosition: 'top'
      })
    }
  }
  
  // Event handlers for filters
  onFiltersChanged(filters: TaskFilters): void {
    this.taskStore.setFilters(filters)
  }
  
  onSearchChanged(searchTerm: string): void {
    this.taskStore.searchTasks(searchTerm)
  }
  
  onFiltersCleared(): void {
    this.taskStore.clearFilters()
  }
  
  // Navigation and view actions
  onNewTaskClick(): void {
    this.router.navigate(['/tasks/new'])
  }
  
  onViewModeChanged(viewMode: 'list' | 'grid' | 'kanban'): void {
    this.taskStore.setViewMode(viewMode)
  }
  
  // Pagination
  onPageChanged(page: number): void {
    this.taskStore.setPage(page)
  }
  
  onNextPage(): void {
    if (this.hasNextPage()) {
      this.taskStore.setPage(this.currentPage() + 1)
    }
  }
  
  onPreviousPage(): void {
    if (this.hasPreviousPage()) {
      this.taskStore.setPage(this.currentPage() - 1)
    }
  }
  
  // Utility methods
  onRefresh(): void {
    this.taskStore.refreshTasks()
  }
  
  onDismissError(): void {
    this.taskStore.clearError()
  }
}
