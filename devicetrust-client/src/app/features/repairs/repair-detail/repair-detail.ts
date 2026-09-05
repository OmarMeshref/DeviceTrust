import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RepairService } from '../../../core/services/repair.service';
import { RepairDetail, RepairStatus } from '../../../core/models/repair.models';

@Component({
  selector: 'app-repair-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './repair-detail.html'
})
export class RepairDetailComponent implements OnInit {
  repair = signal<RepairDetail | null>(null);
  isLoading = signal(true);

  showPartForm = signal(false);
  partSubmitting = signal(false);
  partError = signal<string | null>(null);

  submitInProgress = signal(false);
  submitError = signal<string | null>(null);
  submitSuccess = signal(false);

  partForm;
  RepairStatus = RepairStatus;

  constructor(private route: ActivatedRoute, private repairService: RepairService, private fb: FormBuilder) {
    this.partForm = this.fb.group({
      partName: ['', Validators.required],
      partType: ['', Validators.required],
      oldPartSerial: [''],
      newPartSerial: [''],
      isOriginal: [true],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadRepair();
  }

  private loadRepair(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.repairService.getRepair(id).subscribe({
      next: (r) => {
        this.repair.set(r);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onAddPart(): void {
    if (this.partForm.invalid) return;

    const repairId = this.repair()!.id;
    this.partSubmitting.set(true);
    this.partError.set(null);

    const raw = this.partForm.getRawValue();

    this.repairService.addPart(repairId, {
      partName: raw.partName!,
      partType: raw.partType!,
      oldPartSerial: raw.oldPartSerial || null,
      newPartSerial: raw.newPartSerial || null,
      isOriginal: raw.isOriginal!,
      notes: raw.notes || null
    }).subscribe({
      next: () => {
        this.partSubmitting.set(false);
        this.showPartForm.set(false);
        this.partForm.reset({ isOriginal: true });
        this.loadRepair(); // refresh to show the newly added part
      },
      error: (err) => {
        this.partSubmitting.set(false);
        this.partError.set(err.error?.error ?? 'Failed to add part.');
      }
    });
  }

  onSubmitRepair(): void {
    const repairId = this.repair()!.id;
    this.submitInProgress.set(true);
    this.submitError.set(null);

    this.repairService.submit(repairId).subscribe({
      next: () => {
        this.submitInProgress.set(false);
        this.submitSuccess.set(true);
        this.loadRepair(); // refresh to show Verified status
      },
      error: (err) => {
        this.submitInProgress.set(false);
        this.submitError.set(err.error?.error ?? 'Failed to submit repair.');
      }
    });
  }
}