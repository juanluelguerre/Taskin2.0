import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal } from '@angular/core';
import { CommonModule, TitleCasePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatMenuModule } from '@angular/material/menu';

interface TeamMember {
  name: string;
  initials: string;
}

interface Project {
  id: string;
  name: string;
  description: string;
  status: 'active' | 'completed' | 'on-hold';
  progress: number;
  totalTasks: number;
  completedTasks: number;
  team: TeamMember[];
  dueDate: string;
}

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonToggleModule,
    MatMenuModule,
    TitleCasePipe
  ],
  templateUrl: './projects.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectsComponent {
  selectedFilter = signal('all');

  projects = signal<Project[]>([
    {
      id: '1',
      name: 'E-commerce Platform',
      description: 'Building a modern e-commerce solution with React and Node.js',
      status: 'active',
      progress: 75,
      totalTasks: 32,
      completedTasks: 24,
      team: [
        { name: 'John Doe', initials: 'JD' },
        { name: 'Jane Smith', initials: 'JS' },
        { name: 'Mike Johnson', initials: 'MJ' }
      ],
      dueDate: 'Dec 15, 2024'
    },
    {
      id: '2',
      name: 'Mobile App Redesign',
      description: 'Complete UI/UX overhaul of our mobile application',
      status: 'active',
      progress: 45,
      totalTasks: 28,
      completedTasks: 13,
      team: [
        { name: 'Sarah Wilson', initials: 'SW' },
        { name: 'Tom Brown', initials: 'TB' }
      ],
      dueDate: 'Jan 20, 2025'
    },
    {
      id: '3',
      name: 'API Documentation',
      description: 'Comprehensive API documentation for developers',
      status: 'completed',
      progress: 100,
      totalTasks: 15,
      completedTasks: 15,
      team: [
        { name: 'Alex Chen', initials: 'AC' }
      ],
      dueDate: 'Nov 30, 2024'
    },
    {
      id: '4',
      name: 'Marketing Website',
      description: 'New landing page with improved SEO and performance',
      status: 'on-hold',
      progress: 30,
      totalTasks: 20,
      completedTasks: 6,
      team: [
        { name: 'Emma Davis', initials: 'ED' },
        { name: 'Chris Lee', initials: 'CL' }
      ],
      dueDate: 'Feb 10, 2025'
    },
    {
      id: '5',
      name: 'Data Analytics Dashboard',
      description: 'Real-time analytics dashboard for business intelligence',
      status: 'active',
      progress: 60,
      totalTasks: 25,
      completedTasks: 15,
      team: [
        { name: 'David Kim', initials: 'DK' },
        { name: 'Lisa Park', initials: 'LP' },
        { name: 'Ryan Garcia', initials: 'RG' }
      ],
      dueDate: 'Dec 30, 2024'
    },
    {
      id: '6',
      name: 'Security Audit',
      description: 'Comprehensive security review and vulnerability assessment',
      status: 'active',
      progress: 20,
      totalTasks: 18,
      completedTasks: 4,
      team: [
        { name: 'Security Team', initials: 'ST' }
      ],
      dueDate: 'Jan 15, 2025'
    }
  ]);
}
