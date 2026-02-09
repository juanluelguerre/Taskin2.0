import { computed } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';

export type LoadingMode = 'determinate' | 'indeterminate';

type LoadingBarState = {
  visible: boolean;
  mode: LoadingMode;
  progress: number;
  autoMode: boolean;
};

const initialState: LoadingBarState = {
  visible: false,
  mode: 'indeterminate',
  progress: 0,
  autoMode: true,
};

// Track concurrent HTTP requests outside store state
const urlMap = new Map<string, boolean>();

export const LoadingBarStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    isLoading: computed(() => store.visible()),
  })),
  withMethods((store) => ({
    show: () => {
      patchState(store, { visible: true });
    },

    hide: () => {
      patchState(store, { visible: false });
    },

    setAutoMode: (autoMode: boolean) => {
      patchState(store, { autoMode });
    },

    setMode: (mode: LoadingMode) => {
      patchState(store, { mode });
    },

    setProgress: (progress: number) => {
      const clampedProgress = Math.max(0, Math.min(100, progress));
      patchState(store, { progress: clampedProgress });
    },

    setLoadingStatus: (loading: boolean, url: string) => {
      if (loading) {
        urlMap.set(url, true);
      } else {
        urlMap.delete(url);
      }

      const visible = urlMap.size > 0;
      patchState(store, { visible });
    },
  }))
);
