import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TransferService } from '../../../core/services/transfer.service';
import { Transfer } from '../../../core/models/transfer.models';

@Component({
  selector: 'app-transfer-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './transfer-list.html'
})
export class TransferListComponent implements OnInit {
  transfers = signal<Transfer[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  actioningId = signal<number | null>(null); // tracks which row's button is mid-request, to disable just that one

  constructor(private transferService: TransferService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.transferService.getPending().subscribe({
      next: (transfers) => {
        this.transfers.set(transfers);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load transfers.');
        this.isLoading.set(false);
      }
    });
  }

  onAccept(id: number): void {
    this.actioningId.set(id);
    this.transferService.accept(id).subscribe({
      next: () => this.load(),
      error: (err) => {
        this.actioningId.set(null);
        alert(err.error?.error ?? 'Failed to accept transfer.');
      }
    });
  }

  onReject(id: number): void {
    this.actioningId.set(id);
    this.transferService.reject(id).subscribe({
      next: () => this.load(),
      error: (err) => {
        this.actioningId.set(null);
        alert(err.error?.error ?? 'Failed to reject transfer.');
      }
    });
  }
}