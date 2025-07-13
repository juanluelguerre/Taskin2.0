// header.component.ts

import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslocoModule } from '@ngneat/transloco';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  imports: [
    CommonModule,
    TranslocoModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
  ],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  userName = 'JuanLu';

  @Output() toggleSidebarEvent = new EventEmitter<void>();
  @Output() minimizeSidebarEvent = new EventEmitter<void>();

  toggleSidebar() {
    this.toggleSidebarEvent.emit();
  }

  minimizeSidebar() {
    this.minimizeSidebarEvent.emit();
  }

  getUserInitials(): string {
    return this.userName
      .split(' ')
      .map(name => name[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  changeLanguage(lang: string) {
    // TODO: Implement language change logic with TranslocoService
    console.log(`Language changed to ${lang}`);
  }

  logout() {
    // TODO: Implement logout logic
    console.log('User logged out');
  }
}
