export enum TransferStatus {
  Pending = 1,
  Accepted = 2,
  Rejected = 3,
  Cancelled = 4,
  Expired = 5
}

export interface CreateTransferRequest {
  buyerEmail: string;
}

export interface Transfer {
  id: number;
  deviceId: number;
  devicePublicPassportId: string;
  deviceBrand: string;
  deviceModel: string;
  status: TransferStatus;
  createdAt: string;
  expiresAt: string;
}