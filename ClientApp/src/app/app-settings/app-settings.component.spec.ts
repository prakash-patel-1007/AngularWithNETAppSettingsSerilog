import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AppSettingsComponent } from './app-settings.component';
import { AppSettingsService } from '../services/app-settings.service';

describe('AppSettingsComponent', () => {
  let component: AppSettingsComponent;
  let fixture: ComponentFixture<AppSettingsComponent>;

  beforeEach(waitForAsync(() => {
    const appSettingsStub = {
      spaAppSettings1: 'v1',
      spaAppSettings2: 'v2',
      spaAppSettings3: 'v3',
      spaAppSettings4: 'v4',
      spaAppSettings5: 'v5'
    };

    TestBed.configureTestingModule({
      declarations: [AppSettingsComponent],
      providers: [
        { provide: AppSettingsService, useValue: appSettingsStub }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
