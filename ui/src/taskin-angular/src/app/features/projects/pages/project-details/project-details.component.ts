import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  ViewEncapsulation,
  inject,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router } from '@angular/router';
import { UiConfirmationService } from '@shared/components/dialogs/confirmation/confirmation.service';
import { StatusColorPipe, StatusDisplayPipe, PriorityColorPipe } from '@shared/pipes';
import { ProjectStore } from '../../stores/project.store';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatCardModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    DatePipe,
    TitleCasePipe,
    StatusColorPipe,
    StatusDisplayPipe,
    PriorityColorPipe,
  ],
  providers: [ProjectStore],
  templateUrl: './project-details.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDetailsComponent implements OnInit {
  private readonly projectStore = inject(ProjectStore);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly confirmationService = inject(UiConfirmationService);

  // Expose store selectors
  readonly project = this.projectStore.selectedProject;
  readonly loading = this.projectStore.loading;
  readonly error = this.projectStore.error;
  readonly deleting = this.projectStore.deleting;

  ngOnInit() {
    const projectId = this.route.snapshot.params['id'];
    if (projectId) {
      this.projectStore.loadProject(projectId);
    }
  }

  onEdit() {
    const project = this.project();
    if (project) {
      this.router.navigate(['/projects', project.id, 'edit']);
    }
  }

  onDelete() {
    const project = this.project();
    if (!project) return;

    const confirmationRef = this.confirmationService.open({
      title: 'Delete Project',
      message: `Are you sure you want to delete the project "${project.name}"? This action cannot be undone and will also delete all associated tasks.`,
      icon: {
        show: true,
        name: 'delete',
        color: 'warn',
      },
      actions: {
        confirm: {
          show: true,
          label: 'Delete',
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
      if (result === 'confirmed' && project) {
        this.projectStore.deleteProject(project.id);
        // Navigate back to projects list after successful deletion
        this.router.navigate(['/projects']);
      }
    });
  }

  onBackToProjects() {
    this.router.navigate(['/projects']);
  }

  onViewTask(taskId: string) {
    this.router.navigate(['/tasks', taskId]);
  }

}
