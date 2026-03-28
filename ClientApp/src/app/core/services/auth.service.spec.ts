import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService, LoginResponse } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockResponse: LoginResponse = {
    accessToken: 'test-access-token',
    refreshToken: 'test-refresh-token',
    username: 'admin',
    permissions: ['ViewCounter', 'ViewForecast']
  };

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: 'BASE_URL', useValue: '/' },
        { provide: Router, useValue: routerSpy },
        AuthService
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store session', () => {
    let result: LoginResponse | undefined;
    service.login('admin', 'admin').subscribe(r => result = r);

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ username: 'admin', password: 'admin' });
    req.flush(mockResponse);

    expect(result).toEqual(mockResponse);
    expect(service.getAccessToken()).toBe('test-access-token');
    expect(service.getRefreshToken()).toBe('test-refresh-token');
    expect(service.getUsername()).toBe('admin');
  });

  it('should emit authenticated after login', () => {
    let isAuthenticated = false;
    service.isAuthenticated$.subscribe(v => isAuthenticated = v);

    service.login('admin', 'admin').subscribe();
    httpMock.expectOne('/api/auth/login').flush(mockResponse);

    expect(isAuthenticated).toBeTrue();
  });

  it('should refresh token', () => {
    sessionStorage.setItem('refresh_token', 'old-refresh');

    service.refresh().subscribe();

    const req = httpMock.expectOne('/api/auth/refresh');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'old-refresh' });
    req.flush(mockResponse);

    expect(service.getAccessToken()).toBe('test-access-token');
  });

  it('should clear session on logout', () => {
    sessionStorage.setItem('access_token', 'token');
    sessionStorage.setItem('refresh_token', 'refresh');

    service.logout();

    httpMock.expectOne('/api/auth/logout');

    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should clear session and navigate on sessionExpired', () => {
    sessionStorage.setItem('access_token', 'token');

    service.sessionExpired();

    expect(service.getAccessToken()).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login'], { queryParams: { expired: '1' } });
  });

  it('should return permissions correctly', () => {
    sessionStorage.setItem('permissions', 'ViewCounter,ViewForecast');

    expect(service.getPermissions()).toEqual(['ViewCounter', 'ViewForecast']);
    expect(service.hasPermission('ViewCounter')).toBeTrue();
    expect(service.hasPermission('Admin')).toBeFalse();
  });

  it('should return empty permissions when none stored', () => {
    expect(service.getPermissions()).toEqual([]);
    expect(service.hasPermission('ViewCounter')).toBeFalse();
  });
});
