import { ChangeDetectionStrategy, Component, ViewEncapsulation, OnInit, inject, computed, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { MatChipInputEvent } from '@angular/material/chips';
import { TaskStore, TaskStatus, TaskPriority, CreateTaskRequest, UpdateTaskRequest } from '../../shared';

@Component({
  selector: 'app-task-new',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatChipsModule,
    MatProgressBarModule,
    MatAutocompleteModule
  ],
  templateUrl: './task-new.component.html',
  styles: `
    .task-form-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 24px;
    }
    
    .form-section {
      margin-bottom: 32px;
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr;
      gap: 16px;
    }
    
    @media (min-width: 768px) {
      .form-row.two-columns {
        grid-template-columns: 1fr 1fr;
      }
      
      .form-row.three-columns {
        grid-template-columns: 1fr 1fr 1fr;
      }
    }
    
    .full-width {
      width: 100%;
    }
    
    .form-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 16px;
      margin-top: 32px;
    }
    
    .actions-left {
      display: flex;
      gap: 12px;
    }
    
    .actions-right {
      display: flex;
      gap: 12px;
    }
    
    .chip-list {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
    }
    
    .error-message {
      color: #d32f2f;
      font-size: 0.875rem;
      margin-top: 4px;
    }
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TaskStore]
})
export class TaskNewComponent implements OnInit {
  // Dependencies
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly taskStore = inject(TaskStore);
  private readonly snackBar = inject(MatSnackBar);

  // Form and state
  readonly form: FormGroup;
  private readonly isEditMode = signal<boolean>(false);
  private readonly taskId = signal<string | null>(null);
  
  // Store selectors
  readonly saving = this.taskStore.saving;
  readonly loading = this.taskStore.loading;
  readonly error = this.taskStore.error;
  
  // Form data
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  readonly tags = signal<string[]>([]);

  // Options for dropdowns
  readonly priorityOptions = [
    { value: TaskPriority.Low, label: 'Low', color: 'text-gray-600' },
    { value: TaskPriority.Medium, label: 'Medium', color: 'text-yellow-600' },
    { value: TaskPriority.High, label: 'High', color: 'text-orange-600' },
    { value: TaskPriority.Critical, label: 'Critical', color: 'text-red-600' }
  ];

  readonly statusOptions = [
    { value: TaskStatus.Pending, label: 'Pending' },
    { value: TaskStatus.InProgress, label: 'In Progress' },
    { value: TaskStatus.Completed, label: 'Completed' },
    { value: TaskStatus.Cancelled, label: 'Cancelled' }
  ];

  readonly projects = [
    { id: '1', name: 'Taskin 2.0' },
    { id: '2', name: 'E-commerce Platform' },
    { id: '3', name: 'Mobile App Redesign' },
    { id: '4', name: 'API Documentation' }
  ];

  readonly assignees = [
    { id: '1', name: 'Juan Luis' },
    { id: '2', name: 'Jane Smith' },
    { id: '3', name: 'Mike Johnson' },
    { id: '4', name: 'Sarah Wilson' }
  ];

  // Computed properties
  readonly pageTitle = computed(() => 
    this.isEditMode() ? 'Edit Task' : 'Create New Task'
  );

  readonly submitButtonText = computed(() => 
    this.isEditMode() ? 'Update Task' : 'Create Task'
  );

  constructor() {
    // Initialize reactive form
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      status: [TaskStatus.Pending, [Validators.required]],
      priority: [TaskPriority.Medium, [Validators.required]],
      projectId: ['', [Validators.required]],
      assigneeId: [''],
      dueDate: [null],
      estimatedPomodoros: [null, [Validators.min(1), Validators.max(50)]]
    });
  }

  ngOnInit(): void {
    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id && id !== 'new') {
        this.isEditMode.set(true);
        this.taskId.set(id);
        this.loadTaskForEditing(id);
      }
    });

    // Handle errors
    this.taskStore.error.subscribe(error => {
      if (error) {
        this.snackBar.open(error, 'Dismiss', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
      }
    });
  }

  private loadTaskForEditing(id: string): void {
    // For now, we'll find the task from the store
    // In a real application, this would call taskStore.loadTask(id)
    this.taskStore.refreshTasks();
    
    setTimeout(() => {
      const tasks = this.taskStore.taskViewModels();
      const task = tasks.find(t => t.id === id);
      if (task) {
        // Populate form with task data
        this.form.patchValue({
          title: task.title,
          description: task.description || '',
          status: task.status,
          priority: task.priority,
          projectId: task.projectId,
          assigneeId: task.assigneeId || '',
          dueDate: task.dueDate ? new Date(task.dueDate) : null,
          estimatedPomodoros: task.estimatedPomodoros || null
        });
        
        // Set tags
        this.tags.set([...task.tags]);
      } else {
        this.router.navigate(['/404']);
      }
    }, 500);
  }

  // Tag management
  addTag(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();
    if (value && !this.tags().includes(value)) {
      this.tags.update(current => [...current, value]);
    }
    event.chipInput!.clear();
  }

  removeTag(tag: string): void {
    this.tags.update(current => current.filter(t => t !== tag));
  }

  // Form validation helpers
  getFieldError(fieldName: string): string | null {
    const field = this.form.get(fieldName);
    if (field && field.invalid && field.touched) {
      if (field.errors?.['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required`;
      }
      if (field.errors?.['minlength']) {
        return `${this.getFieldDisplayName(fieldName)} must be at least ${field.errors['minlength'].requiredLength} characters`;
      }
      if (field.errors?.['maxlength']) {
        return `${this.getFieldDisplayName(fieldName)} cannot exceed ${field.errors['maxlength'].requiredLength} characters`;
      }
      if (field.errors?.['min']) {
        return `${this.getFieldDisplayName(fieldName)} must be at least ${field.errors['min'].min}`;
      }
      if (field.errors?.['max']) {
        return `${this.getFieldDisplayName(fieldName)} cannot exceed ${field.errors['max'].max}`;
      }
    }
    return null;
  }

  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      title: 'Title',
      description: 'Description',
      status: 'Status',
      priority: 'Priority',
      projectId: 'Project',
      assigneeId: 'Assignee',
      dueDate: 'Due Date',
      estimatedPomodoros: 'Estimated Pomodoros'
    };
    return displayNames[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  // Form submission
  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      if (this.isEditMode()) {
        const updateRequest: UpdateTaskRequest = {
          id: this.taskId()!,
          title: formValue.title,
          description: formValue.description || undefined,
          status: formValue.status,
          priority: formValue.priority,
          projectId: formValue.projectId,
          assigneeId: formValue.assigneeId || undefined,
          dueDate: formValue.dueDate || undefined,
          estimatedPomodoros: formValue.estimatedPomodoros || undefined,
          isCompleted: formValue.status === TaskStatus.Completed,
          completedAt: formValue.status === TaskStatus.Completed ? new Date() : undefined
        };

        this.taskStore.updateTask({ id: this.taskId()!, request: updateRequest });
        
        this.snackBar.open('Task updated successfully!', 'Dismiss', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
        
        // Navigate back to task details
        this.router.navigate(['/tasks', this.taskId()]);
      } else {
        const createRequest: CreateTaskRequest = {
          title: formValue.title,
          description: formValue.description || undefined,
          status: formValue.status,
          priority: formValue.priority,
          projectId: formValue.projectId,
          assigneeId: formValue.assigneeId || undefined,
          dueDate: formValue.dueDate || undefined,
          estimatedPomodoros: formValue.estimatedPomodoros || undefined,
          tags: this.tags()
        };

        this.taskStore.createTask(createRequest);
        
        this.snackBar.open('Task created successfully!', 'Dismiss', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
        
        // Navigate back to tasks list
        this.router.navigate(['/tasks']);
      }
    } else {
      // Mark all fields as touched to show validation errors
      this.form.markAllAsTouched();
      this.snackBar.open('Please correct the errors in the form', 'Dismiss', {
        duration: 3000,
        horizontalPosition: 'right',
        verticalPosition: 'top'
      });
    }
  }

  onCancel(): void {
    if (this.isEditMode()) {
      this.router.navigate(['/tasks', this.taskId()]);
    } else {
      this.router.navigate(['/tasks']);
    }
  }

  onReset(): void {
    if (confirm('Are you sure you want to reset all changes?')) {
      this.form.reset();
      this.tags.set([]);
      
      // Reset to default values
      this.form.patchValue({
        status: TaskStatus.Pending,
        priority: TaskPriority.Medium
      });
    }
  }
}
