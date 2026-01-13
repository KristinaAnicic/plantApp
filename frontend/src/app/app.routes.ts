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
    }
];
