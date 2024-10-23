import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize, Observable, take } from 'rxjs';
import { LoadingService } from './loading.service';

export const loadingInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const loadingService = inject(LoadingService);
  let handleRequestsAutomatically = false;

  loadingService.auto$.pipe(take(1)).subscribe((value) => {
    handleRequestsAutomatically = value;
  });

  if (!handleRequestsAutomatically) {
    return next(req);
  }

  loadingService.setStatus(true, req.url);

  return next(req).pipe(
    finalize(() => {
      loadingService.setStatus(false, req.url);
    })
  );
};
