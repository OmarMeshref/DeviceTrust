import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './landing.html'
})
export class LandingComponent {
  lookupForm;
  lookupError = signal<string | null>(null);

  constructor(private fb: FormBuilder, private router: Router) {
    this.lookupForm = this.fb.group({
      publicId: ['', Validators.required]
    });
  }

  onLookup(): void {
    if (this.lookupForm.invalid) return;
    const publicId = this.lookupForm.getRawValue().publicId!.trim().toUpperCase();
    this.router.navigate(['/passport', publicId]);
  }
}