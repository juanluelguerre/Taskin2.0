import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation,
} from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidenav/sidebar.component';
import { RouterModule } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  imports: [RouterModule, HeaderComponent, SidebarComponent, FooterComponent],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutComponent {
  isSidebarOpen = true;
  isSidebarMinimized = false;

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
    if (!this.isSidebarOpen) {
      this.isSidebarMinimized = false;
    }
  }

  minimizeSidebar() {
    this.isSidebarMinimized = !this.isSidebarMinimized;
  }
}
