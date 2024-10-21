// header.component.ts

import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatListItem, MatNavList } from '@angular/material/list';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MENU_OPTIONS } from '../../menu-options';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  imports: [CommonModule, MatToolbarModule, MatIconModule, MatMenuModule],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() minimizeSidebar = new EventEmitter<void>();
  @Input() isSidebarMinimized: boolean = false;

  // Language options
  languages = ['English', 'Spanish', 'French'];
  selectedLanguage = 'English';

  // User menu options
  userMenu = [
    { name: 'Profile', action: () => this.goToProfile() },
    { name: 'Logout', action: () => this.logout() },
  ];

  // Change application language
  changeLanguage(lang: string) {
    this.selectedLanguage = lang;
    // Implement language change logic here
    console.log(`Language changed to: ${lang}`);
  }

  // Navigate to user profile
  goToProfile() {
    // Implement navigation to profile page
    console.log('Navigating to profile...');
  }

  // Handle user logout
  logout() {
    // Implement logout logic here
    console.log('User logged out.');
  }

  onToggleSidebar() {
    this.toggleSidebar.emit();
  }

  onMinimizeSidebar() {
    this.minimizeSidebar.emit();
  }
}
