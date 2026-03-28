import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';

export interface AppSettingsDto {
  appSettings1: string;
  appSettings2: string;
  appSettings3: string;
  appSettings4: string;
  appSettings5: string;
  isDemoEnvironment: boolean;
}

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private settingsSubject = new BehaviorSubject<AppSettingsDto | null>(null);
  settings$ = this.settingsSubject.asObservable();
  loadError = '';

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {}

  loadSettings(): Observable<AppSettingsDto | null> {
    return this.http.get<AppSettingsDto>(this.baseUrl + 'api/settings').pipe(
      tap(settings => this.settingsSubject.next(settings)),
      catchError(() => {
        this.loadError = 'Failed to load application settings.';
        return of(null);
      })
    );
  }

  get settings(): AppSettingsDto | null {
    return this.settingsSubject.value;
  }

  get isDemoMode(): boolean {
    return this.settingsSubject.value?.isDemoEnvironment ?? false;
  }
}
