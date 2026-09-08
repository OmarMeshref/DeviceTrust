import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html'
})
export class LoginComponent {
  form;

  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.form.getRawValue() as { email: string; password: string }).subscribe({
      next: () => {
        const role = this.authService.getRole();
        if (role === 'Owner') this.router.navigate(['/owner/home']);
        else if (role === 'Technician') this.router.navigate(['/technician/dashboard']);
        else if (role === 'Admin') this.router.navigate(['/admin/dashboard']);
        else this.router.navigate(['/']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.error ?? 'Login failed. Check your credentials.');
      }
    });
  }
}