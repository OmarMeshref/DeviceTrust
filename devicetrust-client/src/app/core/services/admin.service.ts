import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateRepairCenterRequest, RepairCenter, Technician } from '../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private http: HttpClient) {}

  getRepairCenters(): Observable<RepairCenter[]> {
    return this.http.get<RepairCenter[]>(`${environment.apiUrl}/repaircenters`);
  }

  createRepairCenter(dto: CreateRepairCenterRequest): Observable<{ id: number; name: string }> {
    return this.http.post<{ id: number; name: string }>(`${environment.apiUrl}/repaircenters`, dto);
  }

  approveRepairCenter(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/repaircenters/${id}/approve`, {});
  }

  getTechnicians(): Observable<Technician[]> {
    return this.http.get<Technician[]>(`${environment.apiUrl}/technicians`);
  }

  linkTechnician(technicianId: number, repairCenterId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${environment.apiUrl}/technicians/${technicianId}/link`, { repairCenterId });
  }

  unlinkTechnician(technicianId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/technicians/${technicianId}/unlink`, {});
  }

  approveTechnician(technicianId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/technicians/${technicianId}/approve`, {});
  }
}