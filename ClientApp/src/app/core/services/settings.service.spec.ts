import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { SettingsService, AppSettingsDto } from './settings.service';

describe('SettingsService', () => {
  let service: SettingsService;
  let httpMock: HttpTestingController;

  const mockSettings: AppSettingsDto = {
    appSettings1: 'val1',
    appSettings2: 'val2',
    appSettings3: 'val3',
    appSettings4: 'val4',
    appSettings5: 'val5',
    isDemoEnvironment: true
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: 'BASE_URL', useValue: '/' },
        SettingsService
      ]
    });

    service = TestBed.inject(SettingsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should load settings from API', () => {
    service.loadSettings().subscribe();

    const req = httpMock.expectOne('/api/settings');
    expect(req.request.method).toBe('GET');
    req.flush(mockSettings);

    expect(service.settings).toEqual(mockSettings);
    expect(service.isDemoMode).toBeTrue();
  });

  it('should handle load error gracefully', () => {
    service.loadSettings().subscribe();

    httpMock.expectOne('/api/settings').error(new ProgressEvent('error'));

    expect(service.settings).toBeNull();
    expect(service.loadError).toBe('Failed to load application settings.');
  });

  it('isDemoMode defaults to false when not loaded', () => {
    expect(service.isDemoMode).toBeFalse();
  });
});
