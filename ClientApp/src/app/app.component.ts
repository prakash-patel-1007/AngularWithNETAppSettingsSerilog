import { Component } from '@angular/core';
import { ComponentService } from './services/component.service';
import { SettingsService } from './core/services/settings.service';

@Component({
  standalone: false,
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  headerVisible = false;
  showDemoBanner = false;

  constructor(
    private componentService: ComponentService,
    private settingsService: SettingsService,
  ) {
    this.componentService.result$.subscribe(result => this.headerVisible = result);
    this.settingsService.settings$.subscribe(s => {
      this.showDemoBanner = s?.isDemoEnvironment ?? false;
    });
  }
}
