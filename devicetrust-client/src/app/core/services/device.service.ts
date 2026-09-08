import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateDeviceRequest, DeviceDetail, DeviceListItem, OwnerSummary } from '../models/device.models';

@Injectable({ providedIn: 'root' })
export class DeviceService {
  constructor(private http: HttpClient) {}

  getMyDevices(): Observable<DeviceListItem[]> {
    return this.http.get<DeviceListItem[]>(`${environment.apiUrl}/devices`);
  }

  getDevice(id: number): Observable<DeviceDetail> {
    return this.http.get<DeviceDetail>(`${environment.apiUrl}/devices/${id}`);
  }

  createDevice(dto: CreateDeviceRequest): Observable<{ id: number; publicPassportId: string }> {
    return this.http.post<{ id: number; publicPassportId: string }>(`${environment.apiUrl}/devices`, dto);
  }

  getSummary(): Observable<OwnerSummary> {
    return this.http.get<OwnerSummary>(`${environment.apiUrl}/devices/summary`);
  }
}