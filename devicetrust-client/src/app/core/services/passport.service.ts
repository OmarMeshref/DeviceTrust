import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PublicDevicePassport } from '../models/passport.models';

@Injectable({ providedIn: 'root' })
export class PassportService {
  constructor(private http: HttpClient) {}

  getByPublicId(publicId: string): Observable<PublicDevicePassport> {
    return this.http.get<PublicDevicePassport>(`${environment.apiUrl}/passports/${publicId}`);
  }
}