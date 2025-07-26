import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

interface DashboardStats {
  activeProjects: number;
  pendingTasks: number;
  completedToday: number;
  pomodorosToday: number;
  weeklyProgress: number;
  focusHours: number;
}

interface RecentActivity {
  icon: string;
  title: string;
  time: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './dashboard.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent {
  userName = 'JuanLu';

  stats = signal<DashboardStats>({
    activeProjects: 8,
    pendingTasks: 24,
    completedToday: 12,
    pomodorosToday: 6,
    weeklyProgress: 75,
    focusHours: 28
  });

  recentActivities = signal<RecentActivity[]>([
    {
      icon: 'check_circle',
      title: 'Completed "API Integration" task',
      time: '2 minutes ago'
    },
    {
      icon: 'folder',
      title: 'Created new project "Mobile App"',
      time: '15 minutes ago'
    },
    {
      icon: 'timer',
      title: 'Finished 25-minute focus session',
      time: '30 minutes ago'
    },
    {
      icon: 'task_alt',
      title: 'Added 3 new tasks to "Website Redesign"',
      time: '1 hour ago'
    },
    {
      icon: 'schedule',
      title: 'Updated project deadline',
      time: '2 hours ago'
    }
  ]);
}
