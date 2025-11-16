import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-pomodoro-details',
  standalone: true,
  imports: [],
  templateUrl: './pomodoro-details.component.html',
  styles: ``,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PomodoroDetailsComponent {

}
