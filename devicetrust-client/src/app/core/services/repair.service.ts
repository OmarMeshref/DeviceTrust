import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AddPartRequest, CreateRepairRequest, DeviceLookup, RepairDetail } from '../models/repair.models';

@Injectable({ providedIn: 'root' })
export class RepairService {
  constructor(private http: HttpClient) {}

  lookupDevice(publicId: string): Observable<DeviceLookup> {
    return this.http.get<DeviceLookup>(`${environment.apiUrl}/technicians/devices/lookup/${publicId}`);
  }

  createRepair(deviceId: number, dto: CreateRepairRequest): Observable<{ id: number; status: number }> {
    return this.http.post<{ id: number; status: number }>(
      `${environment.apiUrl}/devices/${deviceId}/repairs`, dto);
  }

  getRepair(id: number): Observable<RepairDetail> {
    return this.http.get<RepairDetail>(`${environment.apiUrl}/repairs/${id}`);
  }

  addPart(repairId: number, dto: AddPartRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/repairs/${repairId}/parts`, dto);
  }

  submit(repairId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/repairs/${repairId}/submit`, {});
  }
}