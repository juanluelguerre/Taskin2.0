import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  Input,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import {
  MatExpansionPanel,
  MatExpansionPanelHeader,
} from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatNavList } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { NavigationService } from '../../core/components/navigation/navigation.service';
import { NavigationItem } from '../../core/components/navigation/navigation.type';
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
    MatExpansionPanel,
    MatExpansionPanelHeader,
  ],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  @Input() isMinimized = false;
  @Input() isHidden = false;

  menuItems = signal<NavigationItem[]>([]);
  openSubmenus = signal<Set<string>>(new Set());

  constructor(private navigationService: NavigationService) {
    this.navigationService.buildNavigation().subscribe((menu) => {
      this.menuItems.set(menu);
    });
  }

  getIcon(iconString: string | undefined): string {
    if (!iconString) return 'circle';
    
    // Map MDI icons to Material icons
    const iconMap: Record<string, string> = {
      'mdi:view-dashboard-variant-outline': 'dashboard',
      'mdi:folder-outline': 'folder',
      'mdi:check-circle-outline': 'task_alt',
      'mdi:timer-outline': 'timer',
      'mdi:account-outline': 'person',
      'mdi:cog-outline': 'settings'
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
