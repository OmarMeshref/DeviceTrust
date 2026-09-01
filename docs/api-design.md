# DeviceTrust — API Design

Status: locked for MVP. Base route prefix: `/api`.

## Conventions

- Auth: JWT bearer token, `[Authorize]` on protected endpoints, `[Authorize(Roles = "...")]` where role-restricted.
- Ownership/resource-level authorization is enforced in the **Service layer**, not the Controller. Controller extracts `userId` from claims and passes it down. Service query filters by `resourceId + callerId` together (never fetch-then-check in C#).
- Unauthorized access to a resource that exists but isn't yours → **404**, never 403 (avoids ID enumeration / leaking existence).
- All private endpoints return role-specific DTOs. Public endpoints never return the same DTO class as a private endpoint, even with fields nulled out — always a separate DTO class.
- Multi-step writes that must succeed/fail together (create device + initial ownership, accept transfer) are wrapped in a DB transaction at the Service layer.

---

## Auth

| Method | Route | Auth | Notes |
|---|---|---|---|
| POST | `/auth/register` | Public | Role field accepted from client but validated server-side — only `Owner` or `Technician` allowed. `Admin` can never be created via this endpoint. Technician created with `RepairCenterId = null`, `IsApproved = false`. |
| POST | `/auth/login` | Public | Returns JWT + role claim. |

---

## Devices

| Method | Route | Auth | Notes |
|---|---|---|---|
| GET | `/devices` | Owner | Returns caller's devices only. `DeviceListItemDto[]`. |
| GET | `/devices/{id}` | Owner | Service checks `deviceId + callerId` together in query. 404 if not caller's device. `OwnerDeviceDetailDto`. |
| POST | `/devices` | Owner | Creates `Device` + initial `Ownership` row in one transaction. Generates `PublicPassportId` (retry-until-unique). `CreateDeviceRequestDto` in. |
| GET | `/passports/{publicId}` | Public | No auth. `PublicDevicePassportDto` — masked serial, no owner PII, repair timeline (Verified records only). |

---

## Ownership Transfers

| Method | Route | Auth | Notes |
|---|---|---|---|
| POST | `/devices/{id}/transfers` | Owner | Caller must be current owner (checked in query). Creates transfer, `Status = Pending`. Blocked if a pending transfer already exists for this device (unique filtered index backs this up at DB level too). |
| GET | `/transfers/pending` | Any authenticated | Returns transfers where caller is the **target buyer**. |
| POST | `/transfers/{id}/accept` | Target buyer only | Transaction: close old `Ownership` (`EndDate = now`), create new `Ownership` row, `Status = Accepted`. Lazy-check expiration first — if expired, reject the accept and set `Status = Expired` instead. |
| POST | `/transfers/{id}/reject` | Target buyer only | `Status = Rejected`. |
| POST | `/transfers/{id}/cancel` | Initiating owner only | `Status = Cancelled`. Only valid while `Pending`. |

---

## Repair Centers / Technicians (Admin-controlled)

| Method | Route | Auth | Notes |
|---|---|---|---|
| POST | `/repaircenters` | Admin | Create repair center, `IsApproved = false` by default. |
| POST | `/repaircenters/{id}/approve` | Admin | Sets `IsApproved = true`. |
| GET | `/repaircenters` | Admin | List/manage. |
| POST | `/technicians/{id}/link` | Admin | Body: `repairCenterId`. Sets FK. |
| POST | `/technicians/{id}/unlink` | Admin | Sets `RepairCenterId = null`, `IsApproved = false`. Never deletes the row (preserves FK history on past `RepairRecord`s). |
| POST | `/technicians/{id}/approve` | Admin | Sets `IsApproved = true`. Only meaningful once linked. |

---

## Repair Records / Parts / Attachments

| Method | Route | Auth | Notes |
|---|---|---|---|
| GET | `/devices/{id}/repairs` | Owner (their device) or Technician (who has a record on it) | Verified + own Draft/Submitted records depending on caller. |
| POST | `/devices/{id}/repairs` | Technician | Trust rule enforced in service: `technician.RepairCenterId != null && repairCenter.IsApproved && technician.IsApproved`. Creates `Status = Draft`. |
| PATCH | `/repairs/{id}` | Technician (owns the record) | Allowed only while `Status = Draft`. Service throws/blocks if `Verified`. |
| POST | `/repairs/{id}/submit` | Technician (owns the record) | Re-checks trust rule at submit time (not just at draft-creation time), then sets `Status = Verified` directly — **no separate verify step in MVP** (auto-verify on submit). Writes `AuditLog` entry. |
| POST | `/repairs/{id}/parts` | Technician (owns the record, Draft only) | Adds `RepairPart` row(s). |
| POST | `/repairs/{id}/attachments` | Technician (owns the record, Draft only) | File upload → local disk, path + metadata stored in `Attachment`. |
| GET | `/repairs/{id}` | Owner (device owner) or Technician (record owner) | Detail view. |

Correction workflow (post-Verified error fix): **not** a PATCH — creates a **new** `RepairRecord` with `CorrectsRecordId` pointing at the original. Original row is never touched. Same `POST /devices/{id}/repairs` endpoint, just with `CorrectsRecordId` set in the request body.

---

## Audit Log

| Method | Route | Auth | Notes |
|---|---|---|---|
| GET | `/admin/audit-logs` | Admin | Filterable by `EntityType`, `EntityId`. |

No POST endpoint — `AuditLog` rows are written internally by services (e.g. inside `TransferService.AcceptAsync()`, `RepairService.SubmitAsync()`), never created directly via a client call.

---

## Deferred (explicitly NOT in MVP)

- Manual/peer-review repair verification step (Phase 2 — currently auto-verify on submit)
- Public repair-center directory endpoint
- Notification endpoints
- Inspection as separate entity/endpoints (merged into RepairRecord.RecordType)
