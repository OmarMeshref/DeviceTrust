// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { DeviceListComponent } from './features/devices/device-list/device-list';
import { DeviceCreateComponent } from './features/devices/device-create/device-create';
import { DeviceDetailComponent } from './features/devices/device-detail/device-detail';
import { PassportViewComponent } from './features/passport/passport-view/passport-view';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';


export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
  {
    path: 'owner/devices',
    component: DeviceListComponent,
    canActivate: [authGuard],
    data: { role: 'Owner' }
  },
  {
    path: 'owner/devices/new',
    component: DeviceCreateComponent,
    canActivate: [authGuard],
    data: { role: 'Owner' }
  },
  {
    path: 'owner/devices/:id',
    component: DeviceDetailComponent,
    canActivate: [authGuard],
    data: { role: 'Owner' }
  },
  { 
    path: 'passport/:publicId', 
    component: PassportViewComponent 
  }
];