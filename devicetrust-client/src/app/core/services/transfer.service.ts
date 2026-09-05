import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTransferRequest, Transfer } from '../models/transfer.models';

@Injectable({ providedIn: 'root' })
export class TransferService {
  constructor(private http: HttpClient) {}

  createTransfer(deviceId: number, dto: CreateTransferRequest): Observable<{ id: number; status: number }> {
    return this.http.post<{ id: number; status: number }>(
      `${environment.apiUrl}/devices/${deviceId}/transfers`, dto);
  }

  getPending(): Observable<Transfer[]> {
    return this.http.get<Transfer[]>(`${environment.apiUrl}/transfers/pending`);
  }

  accept(transferId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/transfers/${transferId}/accept`, {});
  }

  reject(transferId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/transfers/${transferId}/reject`, {});
  }

  cancel(transferId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/transfers/${transferId}/cancel`, {});
  }
}