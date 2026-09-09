import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TransferService } from '../../../core/services/transfer.service';
import { Transfer, TransferStatus } from '../../../core/models/transfer.models';

@Component({
  selector: 'app-transfer-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './transfer-list.html'
})
export class TransferListComponent implements OnInit {
  activeTab = signal<'pending' | 'history'>('pending');
  transfers = signal<Transfer[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  actioningId = signal<number | null>(null);
  TransferStatus = TransferStatus;

  constructor(private transferService: TransferService) {}

  ngOnInit(): void {
    this.load();
  }

  switchTab(tab: 'pending' | 'history'): void {
    this.activeTab.set(tab);
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    const source = this.activeTab() === 'pending'
      ? this.transferService.getPending()
      : this.transferService.getHistory();

    source.subscribe({
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