import { coerceBooleanProperty } from '@angular/cdk/coercion';
import {
  Component,
  DestroyRef,
  inject,
  Input,
  OnChanges,
  SimpleChanges,
  ViewEncapsulation,
} from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoadingService } from './loading.service';
import { LoadingMode } from './loading-bar.types';

@Component({
  selector: 'loading-bar',
  templateUrl: './loading-bar.component.html',
  styleUrls: ['./loading-bar.component.scss'],
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [MatProgressBarModule],
})
export class LoadingBarComponent implements OnChanges {
  private _destroyRef = inject(DestroyRef);

  @Input() autoMode: boolean = true;
  mode: LoadingMode = LoadingMode.Indeterminate;
  progress: number = 0;
  show: boolean = false;

  constructor(private _loadingService: LoadingService) {
    this._loadingService.mode$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((value) => {
        this.mode = value;
      });

    this._loadingService.progress$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((value) => {
        this.progress = value;
      });

    this._loadingService.show$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe((value) => {
        this.show = value;
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ('autoMode' in changes) {
      this._loadingService.setAutoMode(
        coerceBooleanProperty(changes['autoMode'].currentValue)
      );
    }
  }
}
