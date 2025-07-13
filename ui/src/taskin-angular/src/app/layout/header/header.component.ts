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
  userName = 'Juanlu';

  @Output() toggleSidebarEvent = new EventEmitter<void>();
  @Output() minimizeSidebarEvent = new EventEmitter<void>();

  toggleSidebar() {
    this.toggleSidebarEvent.emit();
  }

  minimizeSidebar() {
    this.minimizeSidebarEvent.emit();
  }

  changeLanguage(lang: string) {
    // Logic to change language
    console.log(`Language changed to ${lang}`);
  }

  logout() {
    // Logic to logout
    console.log('User logged out');
  }
}
