import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './landing.html'
})
export class LandingComponent {
  lookupForm;
  lookupError = signal<string | null>(null);

  constructor(private fb: FormBuilder, private router: Router, public authService: AuthService) {
    this.lookupForm = this.fb.group({
      publicId: ['', Validators.required]
    });
  }

  onLookup(): void {
    if (this.lookupForm.invalid) return;
    const publicId = this.lookupForm.getRawValue().publicId!.trim().toUpperCase();
    this.router.navigate(['/passport', publicId]);
  }

  onPrimaryAction(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    const role = this.authService.getRole();
    if (role === 'Owner') this.router.navigate(['/owner/devices/new']);
    else if (role === 'Technician') this.router.navigate(['/technician/dashboard']);
    else if (role === 'Admin') this.router.navigate(['/admin/dashboard']);
  }
}