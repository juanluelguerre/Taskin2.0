import { Route } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { PomodorosComponent } from './features/pomodoros/pages/pomodoros/pomodoros.component';
import { ProjectsComponent } from './features/projects/pages/projects/projects.component';
import { TasksComponent } from './features/tasks/pages/tasks/tasks.component';

export const routes: Route[] = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'projects', component: ProjectsComponent },
  { path: 'tasks', component: TasksComponent },
  { path: 'pomodoros', component: PomodorosComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard', pathMatch: 'full' },
];
