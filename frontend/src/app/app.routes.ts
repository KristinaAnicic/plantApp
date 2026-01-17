import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => 
            import('./pages/plants/plants').then((m) => m.Plants)
    },
    {
        path: 'plant/:id',
        loadComponent: () =>
            import('./pages/plant/plant').then((m) => m.Plant)
    },
    {
        path: 'login',
        loadComponent: () =>
            import('./pages/login/login').then((m) => m.Login),
        canActivate: [guestGuard]
    },
    {
        path: 'register',
        loadComponent: () =>
            import('./pages/register/register').then((m) => m.Register),
        canActivate: [guestGuard]
    },
    }
];
