import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService, CreateProjectCommand, UpdateProjectCommand, ProjectDetailsDto } from '../../services/project.service';

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
    MatProgressSpinnerModule
  ],
  templateUrl: './project-new.component.html',
  styles: `
    /* Fix Material form field label issues */
    .mat-mdc-form-field {
      width: 100%;
      display: block;
      margin-bottom: 0;
    }
    
    .mat-mdc-form-field .mat-mdc-floating-label {
      top: 50% !important;
      transform: translateY(-50%) !important;
    }
    
    .mat-mdc-form-field.mat-mdc-form-field-has-icon-suffix .mat-mdc-text-field-wrapper {
      padding-left: 0;
      padding-right: 0;
    }
    
    /* Proper form field spacing */
    .form-field-container {
      margin-bottom: 32px;
      display: block;
      width: 100%;
    }
    
    .form-field-container:last-child {
      margin-bottom: 0;
    }
    
    /* Ensure labels have proper space */
    .mat-mdc-text-field-wrapper {
      height: 56px;
    }
    
    /* Fix select field height */
    .mat-mdc-select .mat-mdc-select-trigger {
      height: 56px;
      display: flex;
      align-items: center;
    }
    
    /* Fix textarea height */
    .mat-mdc-form-field textarea.mat-mdc-input-element {
      min-height: 100px;
      resize: vertical;
    }
    
    /* Card content padding */
    .mat-mdc-card-content {
      padding: 24px;
    }
    
    .mat-mdc-card-header {
      padding: 24px 24px 16px 24px;
    }
    
    /* Form content wrapper */
    .form-content-wrapper {
      padding: 8px 0;
    }
    
    /* Color preview styling */
    .color-preview {
      min-width: 20px;
      min-height: 20px;
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }
    
    /* Button spacing */
    .button-container {
      padding-top: 8px;
    }
    
    /* Datepicker icon alignment */
    .mat-datepicker-toggle {
      padding: 0;
    }
    
    /* Fix floating labels */
    .mat-mdc-form-field-subscript-wrapper {
      margin-top: 8px;
    }
  `,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectNewComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly projectService = inject(ProjectService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  // State signals
  loading = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);
  projectId = signal<string | null>(null);

  // Form
  projectForm: FormGroup;

  // Status options
  statusOptions = [
    { value: 0, label: 'Active' },
    { value: 1, label: 'Completed' },
    { value: 2, label: 'On Hold' }
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
    { value: '#84CC16', label: 'Lime' }
  ];

  constructor() {
    this.projectForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      dueDate: [null],
      imageUrl: [''],
      backgroundColor: ['#3B82F6'],
      status: [0]
    });
  }

  ngOnInit() {
    // Check if we're in edit mode
    const projectId = this.route.snapshot.params['id'];
    if (projectId) {
      this.isEditMode.set(true);
      this.projectId.set(projectId);
      this.loadProject(projectId);
    }
  }

  loadProject(id: string) {
    this.loading.set(true);
    this.error.set(null);

    this.projectService.getProject(id).subscribe({
      next: (project: ProjectDetailsDto) => {
        this.populateForm(project);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Error loading project. Please try again.');
        this.loading.set(false);
        console.error('Error loading project:', err);
      }
    });
  }

  populateForm(project: ProjectDetailsDto) {
    const statusValue = this.getStatusValue(project.status);
    
    this.projectForm.patchValue({
      name: project.name,
      description: project.description,
      dueDate: project.dueDate ? new Date(project.dueDate) : null,
      imageUrl: project.imageUrl,
      backgroundColor: project.backgroundColor || '#3B82F6',
      status: statusValue
    });
  }

  getStatusValue(status: string): number {
    switch (status.toLowerCase()) {
      case 'active': return 0;
      case 'completed': return 1;
      case 'onhold': return 2;
      default: return 0;
    }
  }

  onSubmit() {
    if (this.projectForm.valid) {
      this.loading.set(true);
      this.error.set(null);

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
      status: formValue.status
    };

    this.projectService.createProject(command).subscribe({
      next: (response) => {
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        this.error.set('Error creating project. Please try again.');
        this.loading.set(false);
        console.error('Error creating project:', err);
      }
    });
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
      status: formValue.status
    };

    this.projectService.updateProject(projectId, command).subscribe({
      next: (response) => {
        this.router.navigate(['/projects', projectId]);
      },
      error: (err) => {
        this.error.set('Error updating project. Please try again.');
        this.loading.set(false);
        console.error('Error updating project:', err);
      }
    });
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
    return control?.hasError(errorType) && control?.touched || false;
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
      status: 'Status'
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
