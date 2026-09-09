import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileService } from '../../../core/services/profile.service';
import { UserProfile } from '../../../core/models/profile.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.html'
})
export class ProfileComponent implements OnInit {
  profile = signal<UserProfile | null>(null);
  isLoading = signal(true);

  constructor(private profileService: ProfileService) {}

  ngOnInit(): void {
    this.profileService.getProfile().subscribe({
      next: (p) => { this.profile.set(p); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }
}