import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { DeviceDetail } from '../../../core/models/device.models';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-detail.html'
})
export class DeviceDetailComponent implements OnInit {
  device = signal<DeviceDetail | null>(null);
  isLoading = signal(true);
  notFound = signal(false);

  constructor(private route: ActivatedRoute, private deviceService: DeviceService) {}

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
}