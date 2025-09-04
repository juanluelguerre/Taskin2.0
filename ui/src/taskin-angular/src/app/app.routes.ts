import { Route } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { PomodorosComponent } from './features/pomodoros/pages/pomodoros/pomodoros.component';
import { pomodoroExitGuard } from './features/pomodoros/shared/guards/pomodoro-exit.guard';
import { pomodoroNavigationGuard } from './core/guards/pomodoro-navigation.guard';
import { ProjectsComponent } from './features/projects/pages/projects/projects.component';
import { ProjectDetailsComponent } from './features/projects/pages/project-details/project-details.component';
import { ProjectNewComponent } from './features/projects/pages/project-new/project-new.component';
import { TasksComponent } from './features/tasks/pages/tasks/tasks.component';
import { TaskNewComponent } from './features/tasks/pages/task-new/task-new.component';
import { TaskDetailsComponent } from './features/tasks/pages/task-details/task-details.component';

export const routes: Route[] = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent, canActivate: [pomodoroNavigationGuard] },
  
  // Project routes
  { path: 'projects', component: ProjectsComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'projects/new', component: ProjectNewComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'projects/:id', component: ProjectDetailsComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'projects/:id/edit', component: ProjectNewComponent, canActivate: [pomodoroNavigationGuard] },
  
  // Task routes
  { path: 'tasks', component: TasksComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'tasks/new', component: TaskNewComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'tasks/:id', component: TaskDetailsComponent, canActivate: [pomodoroNavigationGuard] },
  { path: 'tasks/:id/edit', component: TaskNewComponent, canActivate: [pomodoroNavigationGuard] },
  
  // Pomodoros route - only has the exit guard, not the navigation guard
  { path: 'pomodoros', component: PomodorosComponent, canDeactivate: [pomodoroExitGuard] },
  { path: '**', redirectTo: '/dashboard', pathMatch: 'full' },
];
