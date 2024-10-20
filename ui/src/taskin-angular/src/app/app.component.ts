import {
  ChangeDetectionStrategy,
  Component,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatNavList } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { SidenavComponent } from './layout/sidenav/sidenav.component';
import { ToolbarComponent } from './layout/toolbar/toolbar.component';
import { ContentComponent } from './layout/content/content.component';
import { CommonModule } from '@angular/common';
import { FooterComponent } from './layout/footer/footer.component';
import { debounceTime, fromEvent } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [
    CommonModule,
    MatIconModule,
    MatToolbarModule,
    MatNavList,
    MatSidenavModule,
    ToolbarComponent,
    ContentComponent,
    SidenavComponent,
    FooterComponent,
  ],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  @ViewChild('drawer') drawer!: MatSidenav;
  isLargeScreen: boolean = window.innerWidth >= 768;

  ngOnInit() {
    // Use RxJS to debounce the resize event
    fromEvent(window, 'resize')
      .pipe(debounceTime(200)) // Adjust the debounce time as needed
      .subscribe(() => {
        this.isLargeScreen = window.innerWidth >= 768;
      });
  }
}
