import { Route } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { PomodorosComponent } from './features/pomodoros/pages/pomodoros/pomodoros.component';
import { ProjectsComponent } from './features/projects/pages/projects/projects.component';
import { ProjectDetailsComponent } from './features/projects/pages/project-details/project-details.component';
import { ProjectNewComponent } from './features/projects/pages/project-new/project-new.component';
import { TasksComponent } from './features/tasks/pages/tasks/tasks.component';
import { TaskNewComponent } from './features/tasks/pages/task-new/task-new.component';
import { TaskDetailsComponent } from './features/tasks/pages/task-details/task-details.component';

export const routes: Route[] = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  
  // Project routes
  // Project routes
  { path: 'projects', component: ProjectsComponent },
  { path: 'projects/new', component: ProjectNewComponent },
  { path: 'projects/:id', component: ProjectDetailsComponent },
  { path: 'projects/:id/edit', component: ProjectNewComponent },
  
  // Task routes
  { path: 'tasks', component: TasksComponent },
  { path: 'tasks/new', component: TaskNewComponent },
  { path: 'tasks/:id', component: TaskDetailsComponent },
  { path: 'tasks/:id/edit', component: TaskNewComponent },
  
  { path: 'pomodoros', component: PomodorosComponent },
  { path: '**', redirectTo: '/dashboard', pathMatch: 'full' },
];
