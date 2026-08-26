import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'credenciales', pathMatch: 'full' },
    {
        path: 'credenciales',
        loadComponent: () =>
            import('./pages/listado/listado').then(m => m.Listado)
    },
    {
        path: 'credenciales/alta',
        loadComponent: () =>
            import('./pages/alta/alta').then(m => m.Alta)
    },
    { path: '**', redirectTo: 'credenciales' }
];