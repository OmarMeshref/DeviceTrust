import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { DeviceType } from '../../../core/models/device.models';

@Component({
  selector: 'app-device-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './device-create.html'
})
export class DeviceCreateComponent {
  deviceTypes = [
    { value: DeviceType.Laptop, label: 'Laptop' },
    { value: DeviceType.DesktopPc, label: 'Desktop PC' },
    { value: DeviceType.Smartphone, label: 'Smartphone' },
    { value: DeviceType.Tablet, label: 'Tablet' }
  ];

  form;
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  constructor(private fb: FormBuilder, private deviceService: DeviceService, private router: Router) {
    this.form = this.fb.group({
      type: [DeviceType.Laptop, Validators.required],
      brand: ['', Validators.required],
      model: ['', Validators.required],
      serialNumber: ['', Validators.required],
      purchaseDate: ['']
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();

    this.deviceService.createDevice({
      type: Number(raw.type),
      brand: raw.brand!,
      model: raw.model!,
      serialNumber: raw.serialNumber!,
      purchaseDate: raw.purchaseDate || null
    }).subscribe({
      next: (result) => {
        this.router.navigate(['/owner/devices', result.id]);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.error ?? 'Failed to register device.');
      }
    });
  }
}