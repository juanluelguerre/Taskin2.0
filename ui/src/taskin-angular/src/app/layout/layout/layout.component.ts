import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation,
  HostListener,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidenav/sidebar.component';
import { RouterModule } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  imports: [CommonModule, RouterModule, HeaderComponent, SidebarComponent, FooterComponent],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutComponent {
  isSidebarMinimized = signal(false);
  isSidebarHidden = signal(true);

  constructor() {
    this.checkScreenSize();
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  private checkScreenSize() {
    const isDesktop = window.innerWidth >= 768;
    if (isDesktop) {
      this.isSidebarHidden.set(false);
    } else {
      this.isSidebarHidden.set(true);
    }
  }

  toggleSidebar() {
    this.isSidebarHidden.set(!this.isSidebarHidden());
  }

  minimizeSidebar() {
    this.isSidebarMinimized.set(!this.isSidebarMinimized());
  }
}
