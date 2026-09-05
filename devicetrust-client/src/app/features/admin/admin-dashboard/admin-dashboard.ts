import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminService } from '../../../core/services/admin.service';
import { RepairCenter, Technician } from '../../../core/models/admin.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-dashboard.html'
})
export class AdminDashboardComponent implements OnInit {
  centers = signal<RepairCenter[]>([]);
  technicians = signal<Technician[]>([]);
  isLoading = signal(true);

  showCenterForm = signal(false);
  centerForm;
  centerSubmitting = signal(false);
  centerError = signal<string | null>(null);

  // Tracks which technician row is currently choosing a center to link to,
  // so only that row shows the center-picker dropdown instead of all rows at once.
  linkingTechnicianId = signal<number | null>(null);
  selectedCenterId = signal<number | null>(null);
  actionError = signal<string | null>(null);

  constructor(private adminService: AdminService, private fb: FormBuilder) {
    this.centerForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadAll();
  }

  private loadAll(): void {
    this.isLoading.set(true);
    this.adminService.getRepairCenters().subscribe(centers => {
      this.centers.set(centers);
      this.adminService.getTechnicians().subscribe(technicians => {
        this.technicians.set(technicians);
        this.isLoading.set(false);
      });
    });
  }

  onCreateCenter(): void {
    if (this.centerForm.invalid) return;
    this.centerSubmitting.set(true);
    this.centerError.set(null);

    const raw = this.centerForm.getRawValue();
    this.adminService.createRepairCenter({ name: raw.name!, address: raw.address! }).subscribe({
      next: () => {
        this.centerSubmitting.set(false);
        this.showCenterForm.set(false);
        this.centerForm.reset();
        this.loadAll();
      },
      error: (err) => {
        this.centerSubmitting.set(false);
        this.centerError.set(err.error?.error ?? 'Failed to create repair center.');
      }
    });
  }

  onApproveCenter(id: number): void {
    this.adminService.approveRepairCenter(id).subscribe({
      next: () => this.loadAll(),
      error: (err) => this.actionError.set(err.error?.error ?? 'Failed to approve center.')
    });
  }

  onStartLink(technicianId: number): void {
    this.linkingTechnicianId.set(technicianId);
    this.selectedCenterId.set(null);
  }

  onConfirmLink(technicianId: number): void {
    const centerId = this.selectedCenterId();
    if (!centerId) return;

    this.adminService.linkTechnician(technicianId, centerId).subscribe({
      next: () => {
        this.linkingTechnicianId.set(null);
        this.loadAll();
      },
      error: (err) => this.actionError.set(err.error?.error ?? 'Failed to link technician.')
    });
  }

  onUnlinkTechnician(technicianId: number): void {
    this.adminService.unlinkTechnician(technicianId).subscribe({
      next: () => this.loadAll(),
      error: (err) => this.actionError.set(err.error?.error ?? 'Failed to unlink technician.')
    });
  }

  onApproveTechnician(technicianId: number): void {
    this.adminService.approveTechnician(technicianId).subscribe({
      next: () => this.loadAll(),
      error: (err) => this.actionError.set(err.error?.error ?? 'Failed to approve technician.')
    });
  }
}