import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../core/services/auth.service';
import { ComponentService } from '../services/component.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(waitForAsync(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { queryParams: {} } } },
        ComponentService
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a form with username and password', () => {
    expect(component.form.contains('username')).toBeTrue();
    expect(component.form.contains('password')).toBeTrue();
  });

  it('should not call login when form is invalid', () => {
    component.login();
    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('should call login on valid submit', () => {
    authServiceSpy.login.and.returnValue(of({
      accessToken: 'token', refreshToken: 'refresh', username: 'admin', permissions: []
    }));

    component.form.setValue({ username: 'admin', password: 'admin' });
    component.login();

    expect(authServiceSpy.login).toHaveBeenCalledWith('admin', 'admin');
  });

  it('should display error on login failure', () => {
    authServiceSpy.login.and.returnValue(throwError(() => new Error('fail')));

    component.form.setValue({ username: 'admin', password: 'wrong' });
    component.login();

    expect(component.errorMessage).toBe('Invalid username or password.');
    expect(component.isLoading).toBeFalse();
  });

  it('should show expired message from query params', () => {
    const route = TestBed.inject(ActivatedRoute);
    (route.snapshot.queryParams as any)['expired'] = '1';

    component.ngOnInit();

    expect(component.errorMessage).toBe('Session expired — please sign in again.');
  });
});
