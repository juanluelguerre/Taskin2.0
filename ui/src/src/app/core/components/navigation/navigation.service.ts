import { Injectable } from '@angular/core';
import { defaultNavigation } from './navigation.data';
import { NavigationItem } from './navigation.type';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  buildNavigation(): NavigationItem[] {
    return defaultNavigation;
  }
}
