import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { LoadingBarStore } from './loading-bar.store';

export const loadingInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const store = inject(LoadingBarStore);

  if (!store.autoMode()) {
    return next(req);
  }

  store.setLoadingStatus(true, req.url);

  return next(req).pipe(
    finalize(() => {
      store.setLoadingStatus(false, req.url);
    })
  );
};
