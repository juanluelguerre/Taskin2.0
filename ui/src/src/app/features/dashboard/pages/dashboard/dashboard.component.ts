import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { DashboardService, DashboardStats } from '../../services/dashboard.service';

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
export class DashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  userName = 'JuanLu';

  stats = signal<DashboardStats>({
    activeProjects: 0,
    pendingTasks: 0,
    completedToday: 0,
    pomodorosToday: 0,
    weeklyProgress: 0,
    focusHours: 0
  });

  recentActivities = signal<RecentActivity[]>([]);

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats.set(data);
      },
      error: (err) => {
        console.error('Failed to load dashboard stats:', err);
      }
    });
  }
}
