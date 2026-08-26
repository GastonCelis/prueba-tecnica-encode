import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'credenciales', pathMatch: 'full' },
    {
        path: 'credenciales',
        loadComponent: () =>
            import('./pages/listado/listado').then(m => m.Listado)
    },
    { path: '**', redirectTo: 'credenciales' }
];