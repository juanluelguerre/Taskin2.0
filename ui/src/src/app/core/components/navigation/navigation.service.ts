import { Injectable } from '@angular/core';
import { defaultNavigation } from './navigation.data';
import { map, Observable, of, ReplaySubject, take } from 'rxjs';
import { NavigationItem } from './navigation.type';
import { TranslocoService } from '@jsverse/transloco';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private _navigation: ReplaySubject<NavigationItem[]> = new ReplaySubject<NavigationItem[]>(1);

  constructor(private translocoService: TranslocoService) {}

  buildNavigation(): Observable<NavigationItem[]> {
    const baseNavigation = defaultNavigation;

    //return of(baseNavigation);

    return this.translateNavigation$(baseNavigation).pipe(
      take(1),
      map(translatedNavigation => {
        this._navigation.next(translatedNavigation);
        return translatedNavigation;
      })
    );
  }

  private translateNavigation$(baseNavigation: NavigationItem[]): Observable<NavigationItem[]> {
    return this.translocoService.selectTranslation().pipe(
      take(1),
      map(() => this.translateNavigation(baseNavigation))
    );
  }

  private translateNavigation(baseNavigation: NavigationItem[]): NavigationItem[] {
    const mainNavigation: NavigationItem[] = [];

    baseNavigation.forEach(navItem => {
      if (navItem.type === 'group' || navItem.type === 'aside' || navItem.type === 'collapsable') {
        // manage group
        const groupChildrenNavigation = this.translateNavigation(navItem.children ?? []);
        if (groupChildrenNavigation.length > 0) {
          const groupNavigation = structuredClone(navItem);
          groupNavigation.children = groupChildrenNavigation;
          groupNavigation.title = this.translocoService.translate(navItem.title ?? '');
          groupNavigation.tooltip = this.translocoService.translate(
            navItem.tooltip ?? navItem.title ?? ''
          );
          mainNavigation.push(groupNavigation);
        }

        return;
      }

      const newNavItem = structuredClone(navItem);
      newNavItem.title = this.translocoService.translate(navItem.title ?? '');
      newNavItem.tooltip = this.translocoService.translate(navItem.tooltip ?? navItem.title ?? '');

      mainNavigation.push(newNavItem);
    });

    return mainNavigation;
  }
}
