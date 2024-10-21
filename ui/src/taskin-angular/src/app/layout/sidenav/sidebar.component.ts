import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatNavList } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

interface sidebarMenu {
  link: string;
  icon: string;
  menu: string;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
  imports: [
    CommonModule,
    RouterModule,
    FooterComponent,
    MatSidenavModule,
    MatIconModule,
    MatNavList,
  ],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  @Input() isOpen: boolean = true;
  @Input() isMinimized: boolean = false;
  @Output() toggleMinimize = new EventEmitter<void>();

  onToggleMinimize() {
    this.toggleMinimize.emit();
  }
}
