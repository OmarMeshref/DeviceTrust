// src/app/features/devices/device-detail/device-detail.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import * as QRCode from 'qrcode';
import { DeviceService } from '../../../core/services/device.service';
import { TransferService } from '../../../core/services/transfer.service';
import { DeviceDetail } from '../../../core/models/device.models';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './device-detail.html'
})
export class DeviceDetailComponent implements OnInit {
  device = signal<DeviceDetail | null>(null);
  isLoading = signal(true);
  notFound = signal(false);
  qrDataUrl = signal<string | null>(null);

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
        this.generateQr(device.publicPassportId);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (err.status === 404) this.notFound.set(true);
      }
    });
  }

  private generateQr(publicId: string): void {
    // Points at the Angular app's own passport route — this is the URL a
    // phone camera will open directly when scanning the printed sticker.
    const passportUrl = `${window.location.origin}/passport/${publicId}`;

    QRCode.toDataURL(passportUrl, { width: 220, margin: 2 })
      .then((dataUrl) => this.qrDataUrl.set(dataUrl))
      .catch(() => this.qrDataUrl.set(null));
  }

  downloadQr(): void {
    const url = this.qrDataUrl();
    const publicId = this.device()?.publicPassportId;
    if (!url || !publicId) return;

    const link = document.createElement('a');
    link.href = url;
    link.download = `devicetrust-${publicId}.png`;
    link.click();
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