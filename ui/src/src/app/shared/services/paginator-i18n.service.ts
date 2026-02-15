import { Injectable, inject, DestroyRef } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslocoService } from '@jsverse/transloco';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class PaginatorI18nService {
  private readonly translate = inject(TranslocoService);
  private readonly destroyRef = inject(DestroyRef);

  paginatorIntl = new MatPaginatorIntl();

  constructor() {
    this.translate.langChanges$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.getPaginatorIntl());
  }

  getPaginatorIntl() {
    this.paginatorIntl.itemsPerPageLabel = this.translate.translate('paginator.items_per_page_label');
    this.paginatorIntl.previousPageLabel = this.translate.translate('paginator.previous_page_label');
    this.paginatorIntl.nextPageLabel = this.translate.translate('paginator.next_page_label');
    this.paginatorIntl.firstPageLabel = this.translate.translate('paginator.first_page_label');
    this.paginatorIntl.lastPageLabel = this.translate.translate('paginator.last_page_label');
    this.paginatorIntl.getRangeLabel = this.getRangeLabel.bind(this);

    this.paginatorIntl.changes.next();

    return this.paginatorIntl;
  }

  private getRangeLabel(page: number, pageSize: number, length: number): string {
    if (length === 0 || pageSize === 0) {
      return this.translate.translate('paginator.range_page_label_1', { length });
    }
    length = Math.max(length, 0);

    const startIndex = page * pageSize;
    const endIndex =
      startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;

    return this.translate.translate('paginator.range_page_label_2', {
      startIndex: startIndex + 1,
      endIndex,
      length,
    });
  }
}
