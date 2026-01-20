import { Routes } from '@angular/router';
import { guestGuard } from './utils/guest-guard';

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
    {
        path: 'my-plants',
        loadComponent: () =>
            import('./pages/user-plants/user-plants').then((m) => m.UserPlants)
    },
    {
        path: 'place/:id',
        loadComponent: () =>
            import('./pages/place-plants/place-plants').then((m) => m.PlacePlants)
    },
    {
        path: 'my-plants/:id',
        loadComponent: () =>
            import('./pages/user-plant/user-plant').then((m) => m.UserPlant)
    }
];
