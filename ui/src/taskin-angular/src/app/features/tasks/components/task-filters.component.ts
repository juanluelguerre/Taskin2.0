import { Component, ChangeDetectionStrategy, input, output, signal } from '@angular/core'
import { TitleCasePipe } from '@angular/common'
import { CommonModule } from '@angular/common'
import { FormControl, ReactiveFormsModule } from '@angular/forms'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatSelectModule } from '@angular/material/select'
import { MatButtonModule } from '@angular/material/button'
import { MatButtonToggleModule } from '@angular/material/button-toggle'
import { MatIconModule } from '@angular/material/icon'
import { MatChipsModule } from '@angular/material/chips'
import { TaskStatus, TaskPriority, TaskFilters } from '../shared/types/task.types'

@Component({
  selector: 'app-task-filters',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './task-filters.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskFiltersComponent {
  // Inputs
  filters = input.required<TaskFilters>()
  searchTerm = input<string>('')
  projects = input<Array<{id: string, name: string}>>([])
  assignees = input<Array<{id: string, name: string}>>([])
  
  // Outputs
  filtersChanged = output<TaskFilters>()
  searchChanged = output<string>()
  filtersCleared = output<void>()
  
  // Form controls
  searchControl = new FormControl('')
  
  // Internal state
  private readonly internalFilters = signal<TaskFilters>({})
  
  // Constants
  readonly priorityOptions = [
    { value: TaskPriority.Low, label: 'Low' },
    { value: TaskPriority.Medium, label: 'Medium' },
    { value: TaskPriority.High, label: 'High' },
    { value: TaskPriority.Critical, label: 'Critical' }
  ]
  
  // Computed properties
  hasActiveFilters = () => {
    const f = this.filters()
    return !!(f.status !== undefined || f.priority !== undefined || f.projectId || f.assigneeId || f.isOverdue || (f.tags && f.tags.length > 0))
  }
  
  activeFilterChips = () => {
    const chips: Array<{key: string, label: string}> = []
    const f = this.filters()
    
    if (f.status !== undefined) {
      chips.push({ key: 'status', label: `Status: ${this.getStatusDisplay(f.status)}` })
    }
    
    if (f.priority !== undefined) {
      chips.push({ key: 'priority', label: `Priority: ${this.getPriorityDisplay(f.priority)}` })
    }
    
    if (f.projectId) {
      const project = this.projects().find(p => p.id === f.projectId)
      chips.push({ key: 'projectId', label: `Project: ${project?.name || 'Unknown'}` })
    }
    
    if (f.assigneeId) {
      const assignee = this.assignees().find(a => a.id === f.assigneeId)
      const label = f.assigneeId === 'me' ? 'Assigned to me' : 
                   f.assigneeId === 'unassigned' ? 'Unassigned' :
                   `Assignee: ${assignee?.name || 'Unknown'}`
      chips.push({ key: 'assigneeId', label })
    }
    
    if (f.isOverdue) {
      chips.push({ key: 'isOverdue', label: 'Overdue tasks' })
    }
    
    if (f.tags && f.tags.length > 0) {
      f.tags.forEach((tag, index) => {
        chips.push({ key: `tags-${index}`, label: `Tag: ${tag}` })
      })
    }
    
    return chips
  }
  
  // Event handlers
  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement
    this.searchChanged.emit(target.value)
  }
  
  onStatusChange(status: string | number): void {
    const newStatus = status === 'all' ? undefined : Number(status) as TaskStatus
    this.updateFilters({ status: newStatus })
  }
  
  onPriorityChange(priority: string | number): void {
    const newPriority = priority === 'all' ? undefined : Number(priority) as TaskPriority
    this.updateFilters({ priority: newPriority })
  }
  
  onProjectChange(projectId: string): void {
    const newProjectId = projectId === 'all' ? undefined : projectId
    this.updateFilters({ projectId: newProjectId })
  }
  
  onAssigneeChange(assigneeId: string): void {
    const newAssigneeId = assigneeId === 'all' ? undefined : assigneeId
    this.updateFilters({ assigneeId: newAssigneeId })
  }
  
  toggleOverdueFilter(): void {
    const currentValue = this.filters().isOverdue
    this.updateFilters({ isOverdue: currentValue ? undefined : true })
  }
  
  removeFilter(filterKey: string): void {
    const updates: Partial<TaskFilters> = {}
    
    if (filterKey.startsWith('tags-')) {
      const tagIndex = parseInt(filterKey.split('-')[1])
      const currentTags = this.filters().tags || []
      const newTags = currentTags.filter((_, index) => index !== tagIndex)
      updates.tags = newTags.length > 0 ? newTags : undefined
    } else {
      (updates as any)[filterKey] = undefined
    }
    
    this.updateFilters(updates)
  }
  
  clearAllFilters(): void {
    this.searchControl.setValue('')
    this.filtersCleared.emit()
  }
  
  private updateFilters(updates: Partial<TaskFilters>): void {
    const newFilters = { ...this.filters(), ...updates }
    this.filtersChanged.emit(newFilters)
  }
  
  private getStatusDisplay(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Pending:
        return 'Pending';
      case TaskStatus.InProgress:
        return 'In Progress';
      case TaskStatus.Completed:
        return 'Completed';
      case TaskStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }
  
  private getPriorityDisplay(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Low:
        return 'Low';
      case TaskPriority.Medium:
        return 'Medium';
      case TaskPriority.High:
        return 'High';
      case TaskPriority.Critical:
        return 'Critical';
      default:
        return 'Unknown';
    }
  }
}