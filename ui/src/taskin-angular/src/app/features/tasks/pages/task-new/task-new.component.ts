import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ViewEncapsulation,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { AssigneeService, Assignee } from '@core/services/assignee.service';
import { NotificationService } from '@core/services/notification.service';
import { ProjectService, ProjectListDto } from '../../../projects/shared/services/project.service';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import {
  CreateTaskRequest,
  TaskPriority,
  TaskStatus,
  TaskStore,
  UpdateTaskRequest,
} from '../../shared';

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
    MatProgressSpinnerModule,
    MatAutocompleteModule,
  ],
  templateUrl: './task-new.component.html',
  styleUrl: './task-new.component.scss',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskNewComponent implements OnInit {
  // Dependencies
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly taskStore = inject(TaskStore);
  private readonly projectService = inject(ProjectService);
  private readonly assigneeService = inject(AssigneeService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationService = inject(UiConfirmationService);

  // Form and state
  readonly form: FormGroup;
  readonly isEditMode = signal<boolean>(false);
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
    { value: TaskPriority.Critical, label: 'Critical', color: 'text-red-600' },
  ];

  readonly statusOptions = [
    { value: TaskStatus.Pending, label: 'Pending' },
    { value: TaskStatus.InProgress, label: 'In Progress' },
    { value: TaskStatus.Completed, label: 'Completed' },
    { value: TaskStatus.Cancelled, label: 'Cancelled' },
  ];

  readonly projects = signal<ProjectListDto[]>([]);
  readonly assignees = signal<Assignee[]>([]);

  // Computed properties
  readonly pageTitle = computed(() => (this.isEditMode() ? 'Edit Task' : 'Create New Task'));

  readonly submitButtonText = computed(() => (this.isEditMode() ? 'Update Task' : 'Create Task'));

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
      estimatedPomodoros: [null, [Validators.min(1), Validators.max(50)]],
    });
  }

  ngOnInit(): void {
    // Load projects and assignees
    this.loadProjects();
    this.loadAssignees();
    
    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      const id = params['id'];
      
      if (id && id !== 'new') {
        this.isEditMode.set(true);
        this.taskId.set(id);
        
        // First clear any existing selection
        this.taskStore.clearSelection();
        
        // Load the task
        this.taskStore.loadTask(id);
      } else {
        this.isEditMode.set(false);
        this.taskId.set(null);
        this.taskStore.clearSelection();
      }
    });

    // Watch for task changes to populate form
    effect(() => {
      const task = this.taskStore.selectedTask();
      const isEdit = this.isEditMode();
      
      if (task && isEdit) {
        this.populateFormFromTask(task);
      }
    });

    // Handle errors using effect
    effect(() => {
      const error = this.error();
      if (error) {
        this.notificationService.notifyError('tasks.errors.general', { error });
      }
    });
  }

  private loadProjects(): void {
    this.projectService.getProjectsForDropdown().subscribe({
      next: (projects) => this.projects.set(projects),
      error: (error) => {
        console.error('Error loading projects:', error);
        this.notificationService.notifyError('Failed to load projects');
      }
    });
  }

  private loadAssignees(): void {
    this.assigneeService.getAssigneesForDropdown().subscribe({
      next: (assignees) => this.assignees.set(assignees),
      error: (error) => {
        console.error('Error loading assignees:', error);
        this.notificationService.notifyError('Failed to load assignees');
      }
    });
  }

  private populateFormFromTask(task: any): void {
    this.form.patchValue({
      title: task.title,
      description: task.description || '',
      status: task.status,
      priority: task.priority,
      projectId: task.projectId,
      assigneeId: task.assigneeId || '',
      dueDate: task.dueDate ? new Date(task.dueDate) : null,
      estimatedPomodoros: task.estimatedPomodoros || null,
    });

    // Set tags - ensure tags is an array
    const taskTags = task.tags && Array.isArray(task.tags) ? task.tags : [];
    this.tags.set([...taskTags]);
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
        return `${this.getFieldDisplayName(fieldName)} must be at least ${
          field.errors['minlength'].requiredLength
        } characters`;
      }
      if (field.errors?.['maxlength']) {
        return `${this.getFieldDisplayName(fieldName)} cannot exceed ${
          field.errors['maxlength'].requiredLength
        } characters`;
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
      estimatedPomodoros: 'Estimated Pomodoros',
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
          completedAt: formValue.status === TaskStatus.Completed ? new Date() : undefined,
        };

        this.taskStore.updateTask({ id: this.taskId()!, request: updateRequest });
        this.notificationService.notifySuccess('tasks.messages.updated', { name: formValue.title });
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
          tags: this.tags(),
        };

        this.taskStore.createTask(createRequest);
        this.notificationService.notifySuccess('tasks.messages.created', { name: formValue.title });
        this.router.navigate(['/tasks']);
      }
    } else {
      // Mark all fields as touched to show validation errors
      this.form.markAllAsTouched();
      this.notificationService.notifyWarning('forms.errors.validation');
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
    const confirmationRef = this.confirmationService.open({
      title: 'Reset Form',
      message: 'Are you sure you want to reset all changes? This action cannot be undone.',
      icon: {
        show: true,
        name: 'refresh',
        color: 'warn',
      },
      actions: {
        confirm: {
          show: true,
          label: 'Reset',
          color: 'warn',
        },
        cancel: {
          show: true,
          label: 'Cancel',
        },
      },
      dismissible: true,
    });

    confirmationRef.afterClosed().subscribe(result => {
      if (result === 'confirmed') {
        this.form.reset();
        this.tags.set([]);

        // Reset to default values
        this.form.patchValue({
          status: TaskStatus.Pending,
          priority: TaskPriority.Medium,
        });

        this.notificationService.notifyInfo('forms.messages.reset');
      }
    });
  }
}
