export interface RepairCenter {
  id: number;
  name: string;
  address: string;
  isApproved: boolean;
  createdAt: string;
  technicianCount: number;
}

export interface CreateRepairCenterRequest {
  name: string;
  address: string;
}

export interface Technician {
  id: number;
  userId: string;
  email: string;
  fullName: string;
  repairCenterId: number | null;
  repairCenterName: string | null;
  isApproved: boolean;
}