
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Input,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { NavigationService } from '../../core/components/navigation/navigation.service';
import { NavigationItem } from '../../core/components/navigation/navigation.type';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
  imports: [RouterModule, MatSidenavModule, MatIconModule, TranslocoDirective],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  private readonly navigationService = inject(NavigationService);

  @Input() isMinimized = false;
  @Input() isHidden = false;

  menuItems = signal<NavigationItem[]>(this.navigationService.buildNavigation());
  openSubmenus = signal<Set<string>>(new Set());

  getIcon(iconString: string | undefined): string {
    if (!iconString) return 'circle';

    // Map MDI icons to Material icons
    const iconMap: Record<string, string> = {
      'mdi:view-dashboard-variant-outline': 'dashboard',
      'mdi:folder-outline': 'folder_open',
      'mdi:check-circle-outline': 'task_alt',
      'mdi:timer-outline': 'timer',
      'mdi:account-outline': 'person',
      'mdi:cog-outline': 'settings',
    };

    return iconMap[iconString] || iconString.replace('mdi:', '').replace(/-/g, '_');
  }

  toggleSubmenu(itemId: string): void {
    const current = this.openSubmenus();
    const newSet = new Set(current);

    if (newSet.has(itemId)) {
      newSet.delete(itemId);
    } else {
      newSet.add(itemId);
    }

    this.openSubmenus.set(newSet);
  }

  isSubmenuOpen(itemId: string): boolean {
    return this.openSubmenus().has(itemId);
  }
}
