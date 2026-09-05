import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RepairService } from '../../../core/services/repair.service';

@Component({
  selector: 'app-repair-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './repair-create.html'
})
export class RepairCreateComponent {
  form;
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);
  private deviceId: number;

  constructor(
    private route: ActivatedRoute,
    private repairService: RepairService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.deviceId = Number(this.route.snapshot.paramMap.get('deviceId'));

    this.form = this.fb.group({
      problemDescription: ['', Validators.required],
      diagnosis: ['', Validators.required],
      actionTaken: ['', Validators.required],
      repairDate: [new Date().toISOString().slice(0, 10), Validators.required],
      warrantyUntil: ['']
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();

    this.repairService.createRepair(this.deviceId, {
      problemDescription: raw.problemDescription!,
      diagnosis: raw.diagnosis!,
      actionTaken: raw.actionTaken!,
      repairDate: raw.repairDate!,
      warrantyUntil: raw.warrantyUntil || null,
      correctsRecordId: null // corrections handled separately, not from this basic form
    }).subscribe({
      next: (result) => {
        this.router.navigate(['/technician/repairs', result.id]);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.error ?? 'Failed to create repair record.');
      }
    });
  }
}