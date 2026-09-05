import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RepairService } from '../../../core/services/repair.service';
import { DeviceLookup } from '../../../core/models/repair.models';

@Component({
  selector: 'app-technician-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './technician-dashboard.html'
})
export class TechnicianDashboardComponent {
  form;
  result = signal<DeviceLookup | null>(null);
  errorMessage = signal<string | null>(null);
  isSearching = signal(false);

  constructor(private fb: FormBuilder, private repairService: RepairService, private router: Router) {
    this.form = this.fb.group({
      publicId: ['', Validators.required]
    });
  }

  onSearch(): void {
    if (this.form.invalid) return;

    this.isSearching.set(true);
    this.errorMessage.set(null);
    this.result.set(null);

    const publicId = this.form.getRawValue().publicId!.trim().toUpperCase();

    this.repairService.lookupDevice(publicId).subscribe({
      next: (device) => {
        this.isSearching.set(false);
        this.result.set(device);
      },
      error: (err) => {
        this.isSearching.set(false);
        this.errorMessage.set(err.status === 404 ? 'No device found with that Passport ID.' : 'Lookup failed.');
      }
    });
  }

  onCreateRepair(): void {
    const device = this.result();
    if (!device) return;
    this.router.navigate(['/technician/devices', device.deviceId, 'repairs', 'new']);
  }
}