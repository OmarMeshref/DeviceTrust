import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DeviceService } from '../../../core/services/device.service';
import { TransferService } from '../../../core/services/transfer.service';
import { DeviceDetail } from '../../../core/models/device.models';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './device-detail.html'
})
export class DeviceDetailComponent implements OnInit {
  device = signal<DeviceDetail | null>(null);
  isLoading = signal(true);
  notFound = signal(false);

  showTransferForm = signal(false);
  transferSubmitting = signal(false);
  transferError = signal<string | null>(null);
  transferSuccess = signal<string | null>(null);

  transferForm;

  constructor(
    private route: ActivatedRoute,
    private deviceService: DeviceService,
    private transferService: TransferService,
    private fb: FormBuilder
  ) {
    this.transferForm = this.fb.group({
      buyerEmail: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.deviceService.getDevice(id).subscribe({
      next: (device) => {
        this.device.set(device);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (err.status === 404) this.notFound.set(true);
      }
    });
  }

  onStartTransfer(): void {
    if (this.transferForm.invalid) return;

    const deviceId = this.device()!.id;
    this.transferSubmitting.set(true);
    this.transferError.set(null);

    this.transferService.createTransfer(deviceId, {
      buyerEmail: this.transferForm.getRawValue().buyerEmail!
    }).subscribe({
      next: () => {
        this.transferSubmitting.set(false);
        this.transferSuccess.set('Transfer request sent. Waiting for the buyer to accept.');
        this.showTransferForm.set(false);
        this.transferForm.reset();
      },
      error: (err) => {
        this.transferSubmitting.set(false);
        this.transferError.set(err.error?.error ?? 'Failed to create transfer.');
      }
    });
  }
}