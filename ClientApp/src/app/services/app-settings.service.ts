import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable()
export class AppSettingsService {
  private configuration: IServerConfiguration;
  private _baseUri: string;
  constructor(
      private http: HttpClient,
      @Inject('BASE_URL') baseUrl: string,
      ) {
          this._baseUri = baseUrl;
       }

  loadConfig() {
    return firstValueFrom(this.http.get<IServerConfiguration>(this._baseUri + 'security/getSettings'))
      .then(result => {
        this.configuration = result;
      }, error => console.error(error));
  }

  get spaAppSettings1() {
    return this.configuration.appSettings1;
  }
  get spaAppSettings2() {
    return this.configuration.appSettings2;
  }
  get spaAppSettings3() {
    return this.configuration.appSettings3;
  }
  get spaAppSettings4() {
    return this.configuration.appSettings4;
  }
  get spaAppSettings5() {
    return this.configuration.appSettings5;
  }

}

export interface IServerConfiguration {
  appSettings1: string;
  appSettings2: string;
  appSettings3: string;
  appSettings4: string;
  appSettings5: string;
}