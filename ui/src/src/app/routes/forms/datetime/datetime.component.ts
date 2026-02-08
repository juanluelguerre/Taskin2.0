import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { DateAdapter, provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

import { PageHeaderComponent } from '@shared';

@Component({
  selector: 'app-forms-datetime',
  templateUrl: './datetime.component.html',
  styleUrl: './datetime.component.scss',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    PageHeaderComponent,
  ],
})
export class FormsDatetimeComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly dateAdapter = inject(DateAdapter);
  private readonly translate = inject(TranslateService);

  type = 'native';

  group: FormGroup;
  today: Date;
  tomorrow: Date;
  min: Date;
  max: Date;
  start: Date;
  filter: (date: Date | null) => boolean;

  private translateSubscription = Subscription.EMPTY;

  constructor() {
    this.today = new Date();
    this.tomorrow = new Date();
    this.tomorrow.setDate(this.tomorrow.getDate() + 1);
    this.min = new Date(2018, 10, 3);
    this.max = new Date(2018, 10, 4);
    this.start = new Date(1930, 9, 28);
    this.filter = (date: Date | null) => {
      if (date === null) {
        return true;
      }
      return date.getFullYear() % 2 === 0 && date.getMonth() % 2 === 0 && date.getDate() % 2 === 0;
    };

    this.group = this.fb.group({
      dateTime: [new Date('2017-11-09T12:10:00.000Z'), Validators.required],
      dateTimeManual: [new Date('2017-11-09T12:10:00.000Z'), Validators.required],
      dateTimeYear: [new Date('2017-11-09T12:10:00.000Z'), Validators.required],
      date: [null, Validators.required],
      time: [null, Validators.required],
      timeAMPM: [null, Validators.required],
      timeAMPMManual: [null, Validators.required],
      month: [null, Validators.required],
      year: [null, Validators.required],
      mintest: [this.today, Validators.required],
      filtertest: [this.today, Validators.required],
      touch: [null, Validators.required],
    });
  }

  ngOnInit() {
    this.translateSubscription = this.translate.onLangChange.subscribe((res: { lang: string }) => {
      this.dateAdapter.setLocale(res.lang);
    });
  }

  ngOnDestroy() {
    this.translateSubscription.unsubscribe();
  }
}
