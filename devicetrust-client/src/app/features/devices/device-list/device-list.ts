import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { DeviceListItem } from '../../../core/models/device.models';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-list.html'
})
export class DeviceListComponent implements OnInit {
  devices = signal<DeviceListItem[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor(private deviceService: DeviceService) {}

  ngOnInit(): void {
    this.deviceService.getMyDevices().subscribe({
      next: (devices) => {
        this.devices.set(devices);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load devices.');
        this.isLoading.set(false);
      }
    });
  }
}