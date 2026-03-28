import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

let isRefreshing = false;

export interface ApiError extends Error {
  correlationId?: string;
  status?: number;
  detail?: string;
}

function extractCorrelationId(error: HttpErrorResponse): string | undefined {
  return error.error?.correlationId
    || error.error?.extensions?.correlationId
    || error.headers?.get('X-Correlation-Id')
    || undefined;
}

function toApiError(error: HttpErrorResponse): ApiError {
  const apiError: ApiError = new Error(
    error.error?.title || error.error?.detail || error.message
  ) as ApiError;
  apiError.correlationId = extractCorrelationId(error);
  apiError.status = error.status;
  apiError.detail = error.error?.detail;
  return apiError;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (req.url.includes('/api/auth/login') || req.url.includes('/api/auth/refresh')) {
    return next(req);
  }

  const token = authService.getAccessToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isRefreshing && authService.getRefreshToken()) {
        isRefreshing = true;

        return authService.refresh().pipe(
          switchMap(response => {
            isRefreshing = false;
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${response.accessToken}` }
            });
            return next(retryReq);
          }),
          catchError(refreshError => {
            isRefreshing = false;
            authService.sessionExpired();
            return throwError(() => toApiError(refreshError));
          })
        );
      }

      return throwError(() => toApiError(error));
    })
  );
};
