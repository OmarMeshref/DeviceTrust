import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RepairService } from '../../../core/services/repair.service';
import { RepairListItem, RepairStatus } from '../../../core/models/repair.models';

@Component({
  selector: 'app-repair-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './repair-list.html'
})
export class RepairListComponent implements OnInit {
  repairs = signal<RepairListItem[]>([]);
  isLoading = signal(true);
  RepairStatus = RepairStatus;

  constructor(private repairService: RepairService) {}

  ngOnInit(): void {
    this.repairService.getMyRepairs().subscribe({
      next: (r) => { this.repairs.set(r); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }
}