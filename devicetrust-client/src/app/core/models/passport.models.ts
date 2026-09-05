import { DeviceType } from './device.models';

export interface PublicRepairTimelineItem {
  repairDate: string;
  actionTaken: string;
}

export interface PublicDevicePassport {
  publicPassportId: string;
  type: DeviceType;
  brand: string;
  model: string;
  maskedSerialNumber: string;
  registeredAt: string;
  totalRepairCount: number;
  verifiedRepairCount: number;
  repairTimeline: PublicRepairTimelineItem[];
}