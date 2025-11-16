export enum LoadingMode {
  Determinate = 'determinate',
  Indeterminate = 'indeterminate',
}

export interface LoadingState {
  isAuto: boolean;
  mode: LoadingMode;
  progress: number;
  isVisible: boolean;
}
