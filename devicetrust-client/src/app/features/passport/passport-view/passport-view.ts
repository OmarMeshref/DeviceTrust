import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PassportService } from '../../../core/services/passport.service';
import { PublicDevicePassport } from '../../../core/models/passport.models';
import { DeviceType } from '../../../core/models/device.models';

@Component({
  selector: 'app-passport-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './passport-view.html'
})
export class PassportViewComponent implements OnInit {
  passport = signal<PublicDevicePassport | null>(null);
  isLoading = signal(true);
  notFound = signal(false);

  // Human-readable labels for the enum, since the public page shouldn't show raw numbers.
  private readonly typeLabels: Record<DeviceType, string> = {
    [DeviceType.Laptop]: 'Laptop',
    [DeviceType.DesktopPc]: 'Desktop PC',
    [DeviceType.Smartphone]: 'Smartphone',
    [DeviceType.Tablet]: 'Tablet'
  };

  constructor(private route: ActivatedRoute, private passportService: PassportService) {}

  ngOnInit(): void {
    const publicId = this.route.snapshot.paramMap.get('publicId')!;

    this.passportService.getByPublicId(publicId).subscribe({
      next: (passport) => {
        this.passport.set(passport);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (err.status === 404) this.notFound.set(true);
      }
    });
  }

  typeLabel(type: DeviceType): string {
    return this.typeLabels[type] ?? 'Device';
  }

  // Trust signal shown directly in the UI: are all logged repairs verified,
  // or is there a gap worth a buyer's attention?
  trustStatus(p: PublicDevicePassport): 'clean' | 'partial' | 'none' {
    if (p.totalRepairCount === 0) return 'none';
    if (p.verifiedRepairCount === p.totalRepairCount) return 'clean';
    return 'partial';
  }
}