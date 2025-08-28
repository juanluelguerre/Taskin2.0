import { Route } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { PomodorosComponent } from './features/pomodoros/pages/pomodoros/pomodoros.component';
import { ProjectsComponent } from './features/projects/pages/projects/projects.component';
import { ProjectDetailsComponent } from './features/projects/pages/project-details/project-details.component';
import { ProjectNewComponent } from './features/projects/pages/project-new/project-new.component';
import { TasksComponent } from './features/tasks/pages/tasks/tasks.component';

export const routes: Route[] = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  
  // Project routes
  { path: 'projects', component: ProjectsComponent },
  { path: 'projects/new', component: ProjectNewComponent },
  { path: 'projects/:id', component: ProjectDetailsComponent },
  { path: 'projects/:id/edit', component: ProjectNewComponent },
  
  { path: 'tasks', component: TasksComponent },
  { path: 'pomodoros', component: PomodorosComponent },
  { path: '**', redirectTo: '/dashboard', pathMatch: 'full' },
];
