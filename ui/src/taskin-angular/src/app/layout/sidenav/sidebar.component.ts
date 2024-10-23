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

  constructor(private navigationService: NavigationService) {
    this.navigationService.buildNavigation().subscribe((menu) => {
      this.menuItems.set(menu);

      console.log(this.menuItems);
    });
  }
}
