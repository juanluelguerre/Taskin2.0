import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ViewEncapsulation,
  effect,
  inject,
  signal,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateProjectCommand, UpdateProjectCommand } from '../../services/project.service';
import { ProjectStore } from '../../stores/project.store';

@Component({
  selector: 'app-project-new',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatProgressSpinnerModule,
  ],
  providers: [ProjectStore],
  templateUrl: './project-new.component.html',
  styleUrl: './project-new.component.scss',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectNewComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly projectStore = inject(ProjectStore);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  // State signals
  isEditMode = signal(false);
  projectId = signal<string | null>(null);

  // Expose store selectors
  readonly loading = this.projectStore.loading;
  readonly saving = this.projectStore.saving;
  readonly error = this.projectStore.error;
  readonly project = this.projectStore.selectedProject;

  // Form
  projectForm: FormGroup;

  // Status options
  statusOptions = [
    { value: 0, label: 'Active' },
    { value: 1, label: 'Completed' },
    { value: 2, label: 'On Hold' },
  ];

  // Color options
  colorOptions = [
    { value: '#3B82F6', label: 'Blue' },
    { value: '#10B981', label: 'Green' },
    { value: '#8B5CF6', label: 'Purple' },
    { value: '#F59E0B', label: 'Amber' },
    { value: '#EF4444', label: 'Red' },
    { value: '#14B8A6', label: 'Teal' },
    { value: '#6366F1', label: 'Indigo' },
    { value: '#84CC16', label: 'Lime' },
  ];

  constructor() {
    this.projectForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      dueDate: [null],
      imageUrl: [''],
      backgroundColor: ['#3B82F6'],
      status: [0],
    });
  }

  ngOnInit() {
    // Check if we're in edit mode
    const projectId = this.route.snapshot.params['id'];
    if (projectId) {
      this.isEditMode.set(true);
      this.projectId.set(projectId);
      this.projectStore.loadProject(projectId);

      // Watch for project changes to populate form
      effect(() => {
        const project = this.project();
        if (project) {
          this.populateForm(project);
        }
      });
    }
  }

  populateForm(project: any) {
    const statusValue = this.getStatusValue(project.status);

    this.projectForm.patchValue({
      name: project.name,
      description: project.description,
      dueDate: project.dueDate ? new Date(project.dueDate) : null,
      imageUrl: project.imageUrl,
      backgroundColor: project.backgroundColor || '#3B82F6',
      status: statusValue,
    });
  }

  getStatusValue(status: string): number {
    switch (status.toLowerCase()) {
      case 'active':
        return 0;
      case 'completed':
        return 1;
      case 'onhold':
        return 2;
      default:
        return 0;
    }
  }

  onSubmit() {
    if (this.projectForm.valid) {
      const formValue = this.projectForm.value;

      if (this.isEditMode()) {
        this.updateProject(formValue);
      } else {
        this.createProject(formValue);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  createProject(formValue: any) {
    const command: CreateProjectCommand = {
      name: formValue.name,
      description: formValue.description || undefined,
      dueDate: formValue.dueDate ? formValue.dueDate.toISOString() : undefined,
      imageUrl: formValue.imageUrl || undefined,
      backgroundColor: formValue.backgroundColor || undefined,
      status: formValue.status,
    };

    this.projectStore.createProject(command);
  }

  updateProject(formValue: any) {
    const projectId = this.projectId();
    if (!projectId) return;

    const command: UpdateProjectCommand = {
      id: projectId,
      name: formValue.name,
      description: formValue.description || undefined,
      dueDate: formValue.dueDate ? formValue.dueDate.toISOString() : undefined,
      imageUrl: formValue.imageUrl || undefined,
      backgroundColor: formValue.backgroundColor || undefined,
      status: formValue.status,
    };

    this.projectStore.updateProject({ id: projectId, command });
  }

  onCancel() {
    if (this.isEditMode()) {
      this.router.navigate(['/projects', this.projectId()]);
    } else {
      this.router.navigate(['/projects']);
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.projectForm.controls).forEach(key => {
      const control = this.projectForm.get(key);
      control?.markAsTouched();
    });
  }

  // Helper methods for template
  hasError(controlName: string, errorType: string): boolean {
    const control = this.projectForm.get(controlName);
    return (control?.hasError(errorType) && control?.touched) || false;
  }

  getErrorMessage(controlName: string): string {
    const control = this.projectForm.get(controlName);
    if (control?.hasError('required')) {
      return `${this.getFieldLabel(controlName)} is required`;
    }
    if (control?.hasError('minlength')) {
      const minLength = control.getError('minlength').requiredLength;
      return `${this.getFieldLabel(controlName)} must be at least ${minLength} characters`;
    }
    return '';
  }

  private getFieldLabel(controlName: string): string {
    const labels: { [key: string]: string } = {
      name: 'Project name',
      description: 'Description',
      dueDate: 'Due date',
      imageUrl: 'Image URL',
      backgroundColor: 'Background color',
      status: 'Status',
    };
    return labels[controlName] || controlName;
  }

  onImageError(event: Event): void {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.style.display = 'none';
    }
  }
}
