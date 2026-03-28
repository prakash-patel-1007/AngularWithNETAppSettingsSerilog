import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  username: string;
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly ACCESS_TOKEN = 'access_token';
  private readonly REFRESH_TOKEN = 'refresh_token';
  private readonly USERNAME = 'username';
  private readonly PERMISSIONS = 'permissions';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject('BASE_URL') private baseUrl: string
  ) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.baseUrl + 'api/auth/login', { username, password })
      .pipe(tap(response => this.storeSession(response)));
  }

  refresh(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http.post<LoginResponse>(this.baseUrl + 'api/auth/refresh', { refreshToken })
      .pipe(tap(response => this.storeSession(response)));
  }

  logout(): void {
    const token = this.getAccessToken();
    if (token) {
      this.http.post(this.baseUrl + 'api/auth/logout', {}).subscribe({ error: () => {} });
    }
    this.clearSession();
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem(this.ACCESS_TOKEN);
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem(this.REFRESH_TOKEN);
  }

  getUsername(): string | null {
    return sessionStorage.getItem(this.USERNAME);
  }

  getPermissions(): string[] {
    const raw = sessionStorage.getItem(this.PERMISSIONS);
    return raw ? raw.split(',') : [];
  }

  hasPermission(permission: string): boolean {
    return this.getPermissions().includes(permission);
  }

  clearSession(): void {
    sessionStorage.removeItem(this.ACCESS_TOKEN);
    sessionStorage.removeItem(this.REFRESH_TOKEN);
    sessionStorage.removeItem(this.USERNAME);
    sessionStorage.removeItem(this.PERMISSIONS);
    this.isAuthenticatedSubject.next(false);
  }

  sessionExpired(): void {
    this.clearSession();
    this.router.navigate(['/login'], { queryParams: { expired: '1' } });
  }

  private storeSession(response: LoginResponse): void {
    sessionStorage.setItem(this.ACCESS_TOKEN, response.accessToken);
    sessionStorage.setItem(this.REFRESH_TOKEN, response.refreshToken);
    sessionStorage.setItem(this.USERNAME, response.username);
    sessionStorage.setItem(this.PERMISSIONS, response.permissions.join(','));
    this.isAuthenticatedSubject.next(true);
  }

  private hasToken(): boolean {
    return !!sessionStorage.getItem(this.ACCESS_TOKEN);
  }
}
