import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { OwnerSummary, DeviceListItem } from '../../../core/models/device.models';

@Component({
  selector: 'app-owner-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './owner-home.html'
})
export class OwnerHomeComponent implements OnInit {
  summary = signal<OwnerSummary | null>(null);
  recentDevices = signal<DeviceListItem[]>([]);
  isLoading = signal(true);

  constructor(private deviceService: DeviceService) {}

  ngOnInit(): void {
    this.deviceService.getSummary().subscribe({
      next: (s) => {
        this.summary.set(s);
        this.loadRecentDevices();
      },
      error: () => this.isLoading.set(false)
    });
  }

  private loadRecentDevices(): void {
    this.deviceService.getMyDevices().subscribe({
      next: (devices) => {
        // Most-recently-registered first, capped at 3 — a preview, not the full list.
        this.recentDevices.set(devices.slice(0, 3));
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}