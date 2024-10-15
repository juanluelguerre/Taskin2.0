import {
  ChangeDetectionStrategy,
  Component,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { MtxSidenavModule } from '@ng-matero/extensions/sidenav';
import { MtxToolbarModule } from '@ng-matero/extensions/toolbar';
import { MtxMenuModule } from '@ng-matero/extensions/menu';
import { MtxButtonModule } from '@ng-matero/extensions/button';
import { MtxIconModule } from '@ng-matero/extensions/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sidebar',
  imports: [
    CommonModule,
    MtxSidenavModule,
    MtxToolbarModule,
    MtxMenuModule,
    MtxButtonModule,
    MtxIconModule,
  ],
  ],
  templateUrl: './sidebar.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  isCollapsed = signal<boolean>(false);

  toggleCollapse() {
    this.isCollapsed.set(!this.isCollapsed);
  }
}
