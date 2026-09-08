# DeviceTrust — Business Model Canvas

Reference: [Business Model Canvas (Wikipedia)](https://en.wikipedia.org/wiki/Business_model_canvas)

This canvas treats DeviceTrust as a real product exercise, separate from the technical build. It's not implemented functionality (e.g. no payment or subscription system exists in the MVP) — it's the product-thinking layer behind why the system is designed the way it is.

## Customer Segments
- Private individuals buying or selling used laptops, phones, tablets, and desktops (the app's **Owner** and **Buyer** roles)
- Independent repair shops and technicians who want to build a verifiable track record of their work
- (Secondary, future) refurbishers and resellers who could use verified history as a sales differentiator

## Value Propositions
- **For buyers:** verifiable repair and ownership history before purchasing a used device — removes the "trust me, it's never been repaired" risk inherent to private used-device sales
- **For sellers/owners:** a device with documented, verified history is worth more and sells with less friction and negotiation
- **For repair centers/technicians:** a public, permanent record of quality work becomes a reputation asset, not just a paper receipt that gets lost

## Channels
- The web app itself, where an Owner registers a device and generates its passport/QR
- The physical QR sticker on the device — the actual distribution mechanism for reaching a buyer who has no prior relationship with DeviceTrust and may never visit the site otherwise
- Word of mouth among repair shops — once one shop's verified repairs build visible buyer trust, competing shops have an incentive to join to stay credible

## Customer Relationships
- Self-service for Owners and Buyers — no direct support relationship needed for the MVP
- Admin-mediated approval relationship with Repair Centers — a light "gatekeeper" trust relationship, where DeviceTrust vouches for who it approves to write trusted records

## Revenue Streams *(hypothetical — not implemented in the MVP)*
- Freemium model: free device registration and public passport lookup; a paid tier for repair centers, e.g. a per-verified-repair fee or a monthly subscription to submit records
- Potential future monetization of inspection/certification reports for resale-grade devices

## Key Resources
- The passport/ownership/repair database itself — the core asset; its integrity *is* the product
- The verification and trust-enforcement logic (business rules enforced server-side, immutable verified records, audit logging) — the actual defensible asset, not the UI
- The network of approved repair centers — value increases as more centers join

## Key Activities
- Maintaining data integrity and the verification workflow (immutable verified records once submitted, full audit trail)
- Approving and vetting repair centers — quality control over who is allowed to write trusted history into the system
- Keeping the public passport genuinely privacy-safe (masked serial numbers, no owner personal information exposed) — this is core to the value proposition, not an incidental feature

## Key Partners
- Repair centers and technicians — they generate the actual content (repair records) that makes the product valuable in the first place
- (Future) resale marketplaces or classifieds platforms that could integrate with or link to DeviceTrust passports

## Cost Structure
- Hosting and infrastructure — currently $0 for the MVP (local SQL Server, no cloud spend, per the project's zero-infrastructure-cost constraint)
- Development time (solo developer)
- At scale (hypothetically): cloud hosting, file storage for repair attachments/images, and moderation overhead for repair-center approvals

---

## Honest self-assessment

The weakest block here is **Revenue Streams** — DeviceTrust's real value only materializes once there's a critical mass of both registered devices *and* approved repair centers on the platform, which is a genuine two-sided network-effect problem and a non-trivial go-to-market challenge. This canvas doesn't solve that; it names it.

The strongest blocks are **Key Resources** and **Value Propositions** — the trust and verification model *is* the product. This is consistent with why the project's technical build prioritized getting the business rules, database design, and data integrity right before writing any UI: the UI is not what makes a buyer trust a repair record, the enforced immutability and verification workflow underneath it is.
