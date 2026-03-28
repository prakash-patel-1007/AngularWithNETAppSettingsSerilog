import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BehaviorSubject } from 'rxjs';
import { NavMenuComponent } from './nav-menu.component';
import { AuthService } from '../core/services/auth.service';

describe('NavMenuComponent', () => {
  let component: NavMenuComponent;
  let fixture: ComponentFixture<NavMenuComponent>;
  let authServiceStub: Partial<AuthService>;
  const isAuth$ = new BehaviorSubject<boolean>(false);

  beforeEach(waitForAsync(() => {
    authServiceStub = {
      isAuthenticated$: isAuth$.asObservable(),
      logout: jasmine.createSpy('logout'),
      hasPermission: jasmine.createSpy('hasPermission').and.returnValue(false)
    };

    TestBed.configureTestingModule({
      declarations: [NavMenuComponent],
      imports: [RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: authServiceStub }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle expanded state', () => {
    expect(component.isExpanded).toBeFalse();
    component.toggle();
    expect(component.isExpanded).toBeTrue();
    component.toggle();
    expect(component.isExpanded).toBeFalse();
  });

  it('should collapse', () => {
    component.isExpanded = true;
    component.collapse();
    expect(component.isExpanded).toBeFalse();
  });

  it('should call authService.logout on logout', () => {
    component.logout();
    expect(authServiceStub.logout).toHaveBeenCalled();
  });
});
