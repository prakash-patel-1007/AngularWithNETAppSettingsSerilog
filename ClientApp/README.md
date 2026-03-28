# AngularWithNET — Client App

Angular 19 SPA for the AngularWithNET sample application.

## Sample Functions
- Angular SPA
- .NET 10 API backend
- JWT Authentication with refresh token rotation
- Permissions
- Serilog Logging
- Getting appSettings.json settings in Angular
- Auto Routing to Login if not authenticated
- Todo CRUD with completion tracking

## Development server

Run `npm install` to install all modules required for dev server.

Run `npm start` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files. API calls are proxied to the backend via `proxy.conf.json`.

Run the .NET API in a separate terminal with `dotnet watch run`.

Use `admin` as User Id and `admin` as Password for all permissions.

Use `user` as User Id and `user` as Password for one permission.
