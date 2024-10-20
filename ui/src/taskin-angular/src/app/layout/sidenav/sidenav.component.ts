import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListItem, MatNavList } from '@angular/material/list';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MENU_OPTIONS } from '../../menu-options';
import { ToolbarComponent } from '../toolbar/toolbar.component';
import { ContentComponent } from '../content/content.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.css'],
  imports: [
    CommonModule,
    MatIconModule,
    MatToolbarModule,
    MatNavList,
    MatSidenavModule,
    MatListItem,
    ToolbarComponent,
    ContentComponent,
    FooterComponent,
  ],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidenavComponent {
  @Input() isLargeScreen: boolean = false;
  @Output() drawerToggle = new EventEmitter<void>();
  @ViewChild('drawer') drawer!: MatSidenav;

  isSidenavOpen: boolean = false;
  menuOptions = MENU_OPTIONS;
}
