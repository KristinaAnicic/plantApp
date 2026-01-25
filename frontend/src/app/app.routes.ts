import { Routes } from '@angular/router';
import { guestGuard } from './utils/guest-guard';
import { UpsertPlantDto } from './models/plant.interface'
import { plantEditResolver } from './utils/plant-edit-resolver';
import { plantTradeEditResolver } from './utils/plant-trade-edit-resolver';

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
    },
    {
        path: 'plant-form',
        loadComponent: () =>
            import('./pages/add-edit-plant/add-edit-plant').then((m) => m.AddEditPlant),
        resolve: { editPlant: () => null }
    },
    {
        path: 'plant-form/:id',
        loadComponent: () =>
            import('./pages/add-edit-plant/add-edit-plant').then((m) => m.AddEditPlant),
        resolve: { editPlant: plantEditResolver }
    },
    {
        path: 'trade',
        loadComponent: () =>
            import('./pages/plant-exchange-list/plant-exchange-list').then((m) => m.PlantExchangeList)
    },
    {
        path: 'trade/:id',
        loadComponent: () =>
            import('./pages/plant-exchange/plant-exchange').then((m) => m.PlantExchange)
    },
    {
        path: 'trade-form',
        loadComponent: () =>
            import('./pages/add-edit-exchange/add-edit-exchange').then((m) => m.AddEditExchange)
    },
    {
        path: 'trade-form/:id',
        loadComponent: () =>
            import('./pages/add-edit-exchange/add-edit-exchange').then((m) => m.AddEditExchange),
        resolve: { editTrade: plantTradeEditResolver }
    }
];
