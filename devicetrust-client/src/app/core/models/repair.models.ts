export enum RepairStatus {
  Draft = 1,
  Verified = 2
}

export interface DeviceLookup {
  deviceId: number;
  publicPassportId: string;
  brand: string;
  model: string;
}

export interface CreateRepairRequest {
  problemDescription: string;
  diagnosis: string;
  actionTaken: string;
  repairDate: string;
  warrantyUntil: string | null;
  correctsRecordId: number | null;
}

export interface AddPartRequest {
  partName: string;
  oldPartSerial: string | null;
  newPartSerial: string | null;
  partType: string;
  isOriginal: boolean;
  notes: string | null;
}

export interface RepairPart {
  id: number;
  partName: string;
  oldPartSerial: string | null;
  newPartSerial: string | null;
  partType: string;
  isOriginal: boolean;
  notes: string | null;
}

export interface RepairDetail {
  id: number;
  deviceId: number;
  status: RepairStatus;
  problemDescription: string;
  diagnosis: string;
  actionTaken: string;
  repairDate: string;
  warrantyUntil: string | null;
  createdAt: string;
  verifiedAt: string | null;
  correctsRecordId: number | null;
  parts: RepairPart[];
}

export interface RepairListItem {
  id: number;
  devicePublicPassportId: string;
  deviceBrand: string;
  deviceModel: string;
  status: RepairStatus;
  repairDate: string;
  createdAt: string;
}