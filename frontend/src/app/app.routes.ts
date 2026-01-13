import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => 
            import('./pages/plants/plants').then((m) => m.Plants)
    },
];
