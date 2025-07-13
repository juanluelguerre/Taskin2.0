import { ChangeDetectionStrategy, Component, ViewEncapsulation, signal } from '@angular/core';
import { CommonModule, TitleCasePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';

interface Task {
  id: string;
  title: string;
  description: string;
  status: 'pending' | 'in-progress' | 'completed';
  priority: 'low' | 'medium' | 'high';
  project: string;
  assignee?: string;
  dueDate?: string;
  completed: boolean;
}

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatMenuModule,
    MatCheckboxModule,
    TitleCasePipe
  ],
  templateUrl: './tasks.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TasksComponent {
  selectedFilter = signal('all');
  selectedProject = signal('all');

  tasks = signal<Task[]>([
    {
      id: '1',
      title: 'Implement user authentication',
      description: 'Add login and registration functionality with JWT tokens',
      status: 'in-progress',
      priority: 'high',
      project: 'E-commerce Platform',
      assignee: 'John Doe',
      dueDate: 'Dec 20, 2024',
      completed: false
    },
    {
      id: '2',
      title: 'Design product catalog page',
      description: 'Create responsive design for product listing and filtering',
      status: 'pending',
      priority: 'medium',
      project: 'E-commerce Platform',
      assignee: 'Jane Smith',
      dueDate: 'Dec 18, 2024',
      completed: false
    },
    {
      id: '3',
      title: 'Set up CI/CD pipeline',
      description: 'Configure automated testing and deployment workflow',
      status: 'completed',
      priority: 'high',
      project: 'E-commerce Platform',
      assignee: 'Mike Johnson',
      dueDate: 'Dec 10, 2024',
      completed: true
    },
    {
      id: '4',
      title: 'Create mobile wireframes',
      description: 'Design wireframes for new mobile app interface',
      status: 'in-progress',
      priority: 'medium',
      project: 'Mobile App Redesign',
      assignee: 'Sarah Wilson',
      dueDate: 'Dec 25, 2024',
      completed: false
    },
    {
      id: '5',
      title: 'API endpoint documentation',
      description: 'Document all REST API endpoints with examples',
      status: 'completed',
      priority: 'low',
      project: 'API Documentation',
      assignee: 'Alex Chen',
      dueDate: 'Dec 5, 2024',
      completed: true
    },
    {
      id: '6',
      title: 'Performance optimization',
      description: 'Optimize app performance and reduce load times',
      status: 'pending',
      priority: 'high',
      project: 'Mobile App Redesign',
      assignee: 'Tom Brown',
      dueDate: 'Dec 30, 2024',
      completed: false
    },
    {
      id: '7',
      title: 'User interface testing',
      description: 'Conduct comprehensive UI testing across devices',
      status: 'pending',
      priority: 'medium',
      project: 'Mobile App Redesign',
      dueDate: 'Jan 5, 2025',
      completed: false
    },
    {
      id: '8',
      title: 'Database migration script',
      description: 'Create script to migrate data to new database schema',
      status: 'in-progress',
      priority: 'high',
      project: 'E-commerce Platform',
      assignee: 'David Kim',
      dueDate: 'Dec 22, 2024',
      completed: false
    }
  ]);

  toggleTaskCompletion(task: Task): void {
    const updatedTasks = this.tasks().map(t => {
      if (t.id === task.id) {
        return {
          ...t,
          completed: !t.completed,
          status: !t.completed ? 'completed' : 'pending'
        } as Task;
      }
      return t;
    });
    this.tasks.set(updatedTasks);
  }
}
