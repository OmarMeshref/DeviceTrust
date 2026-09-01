# DeviceTrust — Business Rules

This document lists the core business rules that shape the database design,
API behavior, and authorization logic. Rules are grouped by domain.

---

## 1. Ownership

- **BR-1.1** — A Device may have only one *current* owner at any time.
  Enforced via a filtered unique index:
  `CREATE UNIQUE INDEX UX_Ownership_OneCurrentPerDevice ON Ownership (DeviceId) WHERE EndDate IS NULL;`
- **BR-1.2** — `Ownership.EndDate = NULL` means this ownership period is
  still active (this is the current owner). A non-null `EndDate` means the
  ownership period has closed.
- **BR-1.3** — Ownership history is never deleted or overwritten. Selling a
  device closes the current `Ownership` row and creates a new one — it does
  not modify the old row's `OwnerId`.

## 2. Ownership Transfer

- **BR-2.1** — A Device may have only one *Pending* transfer request at a
  time. Enforced via a filtered unique index:
  `CREATE UNIQUE INDEX UX_Transfer_OnePendingPerDevice ON OwnershipTransfer (DeviceId) WHERE Status = 0; -- Pending`
- **BR-2.2** — Only the `ToUserId` (recipient/buyer) may Accept or Reject a
  transfer. Only the `FromUserId` (current owner) may Cancel a transfer.
- **BR-2.3** — Accepting a transfer must run inside a single database
  transaction: close the old `Ownership` row (set `EndDate`), create a new
  `Ownership` row for `ToUserId`, and set `OwnershipTransfer.Status = Accepted`
  — all three, or none (rollback on failure). This prevents an inconsistent
  state where ownership changed but the transfer still shows "Pending."
- **BR-2.4** — Every `OwnershipTransfer` has an `ExpiresAt`, set at creation
  (e.g. `RequestedAt + 7 days`). Expiration is enforced lazily: the first
  time a Pending transfer past `ExpiresAt` is read or acted upon, its status
  is flipped to `Expired` before any other logic runs. No background job in
  MVP.

## 3. Repair Records & Verification

- **BR-3.1** — A `RepairRecord` follows the lifecycle `Draft → Submitted → Verified`.
- **BR-3.2** — Once `Status = Verified`, a `RepairRecord` (and its related
  `RepairPart` / `Attachment` rows) cannot be edited or deleted. Enforced at
  the application/service layer (throws before `SaveChanges`); a database-level
  constraint/trigger is a recommended future hardening, not required for MVP.
- **BR-3.3** — Mistakes in a Verified record are never corrected by editing.
  A new `RepairRecord` is created with `CorrectsRecordId` pointing to the
  original. The original record is never altered — history is append-only.
- **BR-3.4** — A correction record follows the exact same lifecycle rules as
  any other `RepairRecord` (Draft → Submitted → Verified, immutable once
  Verified, and may itself be corrected). No special-case logic.
- **BR-3.5** — A `RepairRecord` only counts as coming from a *trusted* source
  (eligible to be marked Verified) if all three hold:
  `TechnicianProfile.RepairCenterId IS NOT NULL`
  `AND RepairCenter.IsApproved == true`
  `AND TechnicianProfile.IsApproved == true`

## 4. Repair Centers & Technicians

- **BR-4.1** — Anyone may self-register an account as `Owner` or `Technician`.
  `Admin` is never a self-selectable role at registration — enforced server-side
  regardless of what the client sends.
- **BR-4.2** — A newly registered Technician starts with
  `TechnicianProfile.RepairCenterId = NULL` and `IsApproved = false`. They are
  not linked to any repair center and cannot submit trusted records until an
  Admin links them.
- **BR-4.3** — Only an Admin may create a `RepairCenter`, approve a
  `RepairCenter` (`IsApproved = true`), link a Technician to a `RepairCenter`,
  approve a Technician (`IsApproved = true`), or unlink a Technician from
  their center.
- **BR-4.4** — Unlinking a Technician from a Repair Center (e.g. employment
  ends) sets `RepairCenterId = NULL` and `IsApproved = false`. It never
  deletes the `TechnicianProfile` row, because historical `RepairRecord.TechnicianId`
  foreign keys must continue to resolve. Past Verified repairs by that
  technician remain valid and unchanged.

## 5. Public Passport & Privacy

- **BR-5.1** — `Device.PublicPassportId` (e.g. `DVT-A83K92`) is distinct from
  `Device.Id` (internal database key). The internal `Id` is never exposed via
  any public-facing endpoint or QR code.
- **BR-5.2** — The public Passport view (`GET /api/passports/{publicId}`, no
  auth required) must never expose: owner full name, email, phone, address,
  or the full serial number. Serial numbers are masked (e.g. `PF123456789` →
  `PF******89`).
- **BR-5.3** — Public and private views of a device use separate DTOs
  (`PublicDevicePassportDto` vs. internal detail DTOs). Privacy is enforced
  by DTO shape, not by conditionally hiding fields on a shared model.

## 6. Audit Logging

- **BR-6.1** — Sensitive state-changing actions (repair verified, transfer
  accepted, technician approved/unlinked, repair center approved, etc.) are
  recorded in `AuditLog`.
- **BR-6.2** — `AuditLog.EntityId` + `EntityType` is a polymorphic reference,
  not a strict foreign key — deliberate tradeoff to support logging actions
  across many unrelated entity types without an ever-growing set of nullable
  FK columns. This tradeoff is accepted only for the audit/log table; all
  core business entities keep real foreign keys.

---

## Rule numbering

Rules are referenced elsewhere (API docs, code comments) by their ID
(e.g. `BR-3.2`) so implementation can be traced back to the specific
business justification.
