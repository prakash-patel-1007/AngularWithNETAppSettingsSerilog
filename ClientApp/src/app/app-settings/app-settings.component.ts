import { Component, OnInit } from '@angular/core';
import { AppSettingsService } from '../services/app-settings.service';

@Component({
  standalone: false,
  selector: 'app-app-settings',
  templateUrl: './app-settings.component.html',
  styleUrls: ['./app-settings.component.css']
})
export class AppSettingsComponent implements OnInit {
  public settingsData: SettingsData[]; 
  constructor(private appSettingService: AppSettingsService) { }

  ngOnInit() {
    this.settingsData = [{
      setting: 'Data 1',
      value: this.appSettingService.spaAppSettings1
    },
    {
      setting: 'Data 2',
      value: this.appSettingService.spaAppSettings2
    },
    {
      setting: 'Data 3',
      value: this.appSettingService.spaAppSettings3
    },
    {
      setting: 'Data 4',
      value: this.appSettingService.spaAppSettings4
    },
    {
      setting: 'Data 5',
      value: this.appSettingService.spaAppSettings5
    }];

  }

}

interface SettingsData {
  setting: string;
  value: string;
}
