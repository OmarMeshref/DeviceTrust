import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { DeviceListComponent } from './features/devices/device-list/device-list';
import { DeviceCreateComponent } from './features/devices/device-create/device-create';
import { DeviceDetailComponent } from './features/devices/device-detail/device-detail';
import { PassportViewComponent } from './features/passport/passport-view/passport-view';
import { TransferListComponent } from './features/transfers/transfer-list/transfer-list';
import { TechnicianDashboardComponent } from './features/repairs/technician-dashboard/technician-dashboard';
import { RepairCreateComponent } from './features/repairs/repair-create/repair-create';
import { RepairDetailComponent } from './features/repairs/repair-detail/repair-detail';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard';
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
    path: 'transfers/pending',
    component: TransferListComponent,
    canActivate: [authGuard]
  },
  {
    path: 'passport/:publicId',
    component: PassportViewComponent
  },
  {
    path: 'technician/dashboard',
    component: TechnicianDashboardComponent,
    canActivate: [authGuard],
    data: { role: 'Technician' }
  },
  {
    path: 'technician/devices/:deviceId/repairs/new',
    component: RepairCreateComponent,
    canActivate: [authGuard],
    data: { role: 'Technician' }
  },
  {
    path: 'technician/repairs/:id',
    component: RepairDetailComponent,
    canActivate: [authGuard],
    data: { role: 'Technician' }
  },
  {
    path: 'admin/dashboard',
    component: AdminDashboardComponent,
    canActivate: [authGuard],
    data: { role: 'Admin' }
  }
];