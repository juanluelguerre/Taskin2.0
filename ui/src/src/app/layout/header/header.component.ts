
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  inject,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  imports: [TranslocoModule, MatToolbarModule, MatIconModule, MatMenuModule],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  private readonly translocoService = inject(TranslocoService);

  @Input() isDesktop = false;
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
    this.translocoService.setActiveLang(lang);
  }

  logout() {
    // TODO: Implement logout logic
    console.log('User logged out');
  }
}
