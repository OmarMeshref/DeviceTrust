export enum DeviceType {
  Laptop = 1,
  DesktopPc = 2,
  Smartphone = 3,
  Tablet = 4
}

export interface DeviceListItem {
  id: number;
  publicPassportId: string;
  type: DeviceType;
  brand: string;
  model: string;
  registeredAt: string;
}

export interface DeviceDetail extends DeviceListItem {
  serialNumber: string;
  purchaseDate: string | null;
  repairCount: number;
}

export interface CreateDeviceRequest {
  type: DeviceType;
  brand: string;
  model: string;
  serialNumber: string;
  purchaseDate: string | null;
}

export interface OwnerSummary {
  totalDevices: number;
  pendingTransfersOut: number;
  pendingTransfersIn: number;
  totalRepairsAcrossDevices: number;
}