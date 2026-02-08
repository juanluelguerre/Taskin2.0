import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';

import { PageHeaderComponent } from '@shared';
import { TablesDataService } from '../data.service';
import { TablesKitchenSinkEditComponent } from './edit/edit.component';

export interface ColumnDef {
  header: any;
  field: string;
  sortable?: boolean;
  disabled?: boolean;
  minWidth?: number;
  width?: string;
  hide?: boolean;
  pinned?: string;
  type?: string;
  showExpand?: boolean;
  buttons?: any[];
}

@Component({
  selector: 'app-table-kitchen-sink',
  templateUrl: './kitchen-sink.component.html',
  styleUrl: './kitchen-sink.component.scss',
  providers: [TablesDataService],
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatRadioModule,
    MatSortModule,
    MatTableModule,
    PageHeaderComponent,
  ],
})
export class TablesKitchenSinkComponent implements OnInit {
  private readonly translate = inject(TranslateService);
  private readonly dataSrv = inject(TablesDataService);
  private readonly dialog = inject(MatDialog);

  columns: ColumnDef[] = [
    {
      header: 'Position',
      field: 'position',
      sortable: true,
      minWidth: 100,
      width: '100px',
    },
    {
      header: 'Name',
      field: 'name',
      sortable: true,
      disabled: true,
      minWidth: 100,
      width: '100px',
    },
    {
      header: 'Weight',
      field: 'weight',
      minWidth: 100,
    },
    {
      header: 'Symbol',
      field: 'symbol',
      minWidth: 100,
    },
    {
      header: 'Gender',
      field: 'gender',
      minWidth: 100,
    },
    {
      header: 'Mobile',
      field: 'mobile',
      hide: true,
      minWidth: 120,
    },
    {
      header: 'Tele',
      field: 'tele',
      minWidth: 120,
      width: '120px',
    },
    {
      header: 'Birthday',
      field: 'birthday',
      minWidth: 180,
    },
    {
      header: 'City',
      field: 'city',
      minWidth: 120,
    },
    {
      header: 'Address',
      field: 'address',
      minWidth: 180,
      width: '200px',
    },
    {
      header: 'Company',
      field: 'company',
      minWidth: 120,
    },
    {
      header: 'Website',
      field: 'website',
      minWidth: 180,
    },
    {
      header: 'Email',
      field: 'email',
      minWidth: 180,
    },
  ];

  get displayedColumns(): string[] {
    return this.columns.filter(c => !c.hide).map(c => c.field).concat(['operation']);
  }

  list: any[] = [];
  isLoading = true;

  multiSelectable = true;
  rowSelectable = true;
  hideRowSelectionCheckbox = false;
  showToolbar = true;
  columnHideable = true;
  columnSortable = true;
  columnPinnable = true;
  rowHover = false;
  rowStriped = false;
  showPaginator = true;
  expandable = false;
  columnResizable = false;

  ngOnInit() {
    this.list = this.dataSrv.getData();
    this.isLoading = false;
  }

  edit(value: any) {
    const dialogRef = this.dialog.open(TablesKitchenSinkEditComponent, {
      width: '600px',
      data: { record: value },
    });

    dialogRef.afterClosed().subscribe(() => console.log('The dialog was closed'));
  }

  delete(value: any) {
    window.alert(`You have deleted ${value.position}!`);
  }

  changeSelect(e: any) {
    console.log(e);
  }

  changeSort(e: any) {
    console.log(e);
  }

  enableRowExpandable() {
    this.columns[0].showExpand = this.expandable;
  }

  updateCell() {
    this.list = this.list.map(item => {
      item.weight = Math.round(Math.random() * 1000) / 100;
      return item;
    });
  }

  updateList() {
    this.list = this.list.splice(-1).concat(this.list);
  }
}
