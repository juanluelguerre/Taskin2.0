import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoadingMode, LoadingState } from './loading-bar.types';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private static readonly INITIAL_STATE: LoadingState = {
    isAuto: true,
    mode: LoadingMode.Indeterminate,
    progress: 0,
    isVisible: false,
  };

  private static readonly MIN_PROGRESS = 0;
  private static readonly MAX_PROGRESS = 100;

  private readonly state$ = new BehaviorSubject<LoadingState>(
    LoadingService.INITIAL_STATE
  );

  private readonly urlMap = new Map<string, boolean>();

  constructor() {}

  get auto$(): Observable<boolean> {
    return new Observable((subscriber) => {
      this.state$.subscribe((state) => subscriber.next(state.isAuto));
    });
  }

  get mode$(): Observable<LoadingMode> {
    return new Observable((subscriber) => {
      this.state$.subscribe((state) => subscriber.next(state.mode));
    });
  }

  get progress$(): Observable<number> {
    return new Observable((subscriber) => {
      this.state$.subscribe((state) => subscriber.next(state.progress));
    });
  }

  get show$(): Observable<boolean> {
    return new Observable((subscriber) => {
      this.state$.subscribe((state) => subscriber.next(state.isVisible));
    });
  }

  show(): void {
    this.updateState({ isVisible: true });
  }

  hide(): void {
    this.updateState({ isVisible: false });
  }

  setAutoMode(isAuto: boolean): void {
    this.updateState({ isAuto });
  }

  setMode(mode: LoadingMode): void {
    this.updateState({ mode });
  }

  setProgress(progress: number): void {
    if (!this.isValidProgress(progress)) {
      console.error(
        `Progress value must be between ${LoadingService.MIN_PROGRESS} and ${LoadingService.MAX_PROGRESS}`
      );
      return;
    }
    this.updateState({ progress });
  }

  setStatus(status: boolean, url: string): void {
    if (!this.isValidUrl(url)) {
      console.error('Invalid URL provided');
      return;
    }

    this.handleUrlStatus(status, url);
    this.updateVisibilityBasedOnUrls();
  }

  private updateState(partialState: Partial<LoadingState>): void {
    this.state$.next({
      ...this.state$.value,
      ...partialState,
    });
  }

  private isValidProgress(progress: number): boolean {
    return (
      progress >= LoadingService.MIN_PROGRESS &&
      progress <= LoadingService.MAX_PROGRESS
    );
  }

  private isValidUrl(url: string): boolean {
    return Boolean(url?.trim());
  }

  private handleUrlStatus(status: boolean, url: string): void {
    if (status) {
      this.urlMap.set(url, true);
    } else {
      this.urlMap.delete(url);
    }
  }

  private updateVisibilityBasedOnUrls(): void {
    const isVisible = this.urlMap.size > 0;
    this.updateState({ isVisible });
  }
}
