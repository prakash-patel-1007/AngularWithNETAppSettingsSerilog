import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LoginComponent } from './login/login.component';
import { AppSettingsComponent } from './app-settings/app-settings.component';
import { PermissionDirective } from './services/permission.directive';
import { TodoListComponent } from './features/todos/todo-list.component';

import { ComponentService } from './services/component.service';
import { AppSettingsService } from './services/app-settings.service';
import { SettingsService } from './core/services/settings.service';
import { authGuard } from './core/guards/auth.guard';
import { authInterceptor } from './core/interceptors/auth.interceptor';

const appInitializerFn = (settingsService: SettingsService) => {
  return () => settingsService.loadSettings();
};

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    AppSettingsComponent,
    PermissionDirective,
    TodoListComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent, canActivate: [authGuard], pathMatch: 'full' },
      { path: 'counter', canActivate: [authGuard], component: CounterComponent },
      { path: 'fetch-data', canActivate: [authGuard], component: FetchDataComponent },
      { path: 'app-settings', canActivate: [authGuard], component: AppSettingsComponent },
      { path: 'todos', canActivate: [authGuard], component: TodoListComponent },
      { path: 'login', component: LoginComponent },
    ])
  ],
  providers: [
    { provide: 'BASE_URL', useFactory: getBaseUrl },
    { provide: 'ORIGIN_URL', useFactory: getBaseUrl },
    provideHttpClient(withInterceptors([authInterceptor])),
    ComponentService,
    AppSettingsService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      multi: true,
      deps: [SettingsService]
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
