import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthInterceptor } from './utils/auth-interceptor';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AppTranslateModule } from './translate/translate-module';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes, 
      withInMemoryScrolling({scrollPositionRestoration: 'enabled'}), 
      withComponentInputBinding()
    ),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    importProvidersFrom(
      MatSnackBarModule,
      AppTranslateModule 
    )
  ]
};
