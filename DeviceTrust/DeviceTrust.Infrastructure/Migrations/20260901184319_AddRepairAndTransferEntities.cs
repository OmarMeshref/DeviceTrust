using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceTrust.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairAndTransferEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnershipTransfer_Devices_DeviceId",
                table: "OwnershipTransfer");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairRecord_Devices_DeviceId",
                table: "RepairRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepairRecord",
                table: "RepairRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnershipTransfer",
                table: "OwnershipTransfer");

            migrationBuilder.DropIndex(
                name: "IX_OwnershipTransfer_DeviceId",
                table: "OwnershipTransfer");

            migrationBuilder.RenameTable(
                name: "RepairRecord",
                newName: "RepairRecords");

            migrationBuilder.RenameTable(
                name: "OwnershipTransfer",
                newName: "OwnershipTransfers");

            migrationBuilder.RenameIndex(
                name: "IX_RepairRecord_DeviceId",
                table: "RepairRecords",
                newName: "IX_RepairRecords_DeviceId");

            migrationBuilder.AddColumn<string>(
                name: "ActionTaken",
                table: "RepairRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CorrectsRecordId",
                table: "RepairRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RepairRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "RepairRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProblemDescription",
                table: "RepairRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecordType",
                table: "RepairRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepairDate",
                table: "RepairRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RepairRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianProfileId",
                table: "RepairRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "RepairRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyUntil",
                table: "RepairRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OwnershipTransfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "OwnershipTransfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "InitiatingOwnerId",
                table: "OwnershipTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "OwnershipTransfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OwnershipTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetBuyerId",
                table: "OwnershipTransfers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepairRecords",
                table: "RepairRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnershipTransfers",
                table: "OwnershipTransfers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairRecordId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentType = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_RepairRecords_RepairRecordId",
                        column: x => x.RepairRecordId,
                        principalTable: "RepairRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepairRecordId = table.Column<int>(type: "int", nullable: false),
                    PartName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldPartSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewPartSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairParts_RepairRecords_RepairRecordId",
                        column: x => x.RepairRecordId,
                        principalTable: "RepairRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepairCenterId = table.Column<int>(type: "int", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianProfiles_RepairCenters_RepairCenterId",
                        column: x => x.RepairCenterId,
                        principalTable: "RepairCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairRecords_CorrectsRecordId",
                table: "RepairRecords",
                column: "CorrectsRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRecords_TechnicianProfileId",
                table: "RepairRecords",
                column: "TechnicianProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipTransfers_DeviceId",
                table: "OwnershipTransfers",
                column: "DeviceId",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_RepairRecordId",
                table: "Attachments",
                column: "RepairRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairParts_RepairRecordId",
                table: "RepairParts",
                column: "RepairRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianProfiles_RepairCenterId",
                table: "TechnicianProfiles",
                column: "RepairCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianProfiles_UserId",
                table: "TechnicianProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnershipTransfers_Devices_DeviceId",
                table: "OwnershipTransfers",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairRecords_Devices_DeviceId",
                table: "RepairRecords",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairRecords_RepairRecords_CorrectsRecordId",
                table: "RepairRecords",
                column: "CorrectsRecordId",
                principalTable: "RepairRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairRecords_TechnicianProfiles_TechnicianProfileId",
                table: "RepairRecords",
                column: "TechnicianProfileId",
                principalTable: "TechnicianProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnershipTransfers_Devices_DeviceId",
                table: "OwnershipTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairRecords_Devices_DeviceId",
                table: "RepairRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairRecords_RepairRecords_CorrectsRecordId",
                table: "RepairRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairRecords_TechnicianProfiles_TechnicianProfileId",
                table: "RepairRecords");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "RepairParts");

            migrationBuilder.DropTable(
                name: "TechnicianProfiles");

            migrationBuilder.DropTable(
                name: "RepairCenters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepairRecords",
                table: "RepairRecords");

            migrationBuilder.DropIndex(
                name: "IX_RepairRecords_CorrectsRecordId",
                table: "RepairRecords");

            migrationBuilder.DropIndex(
                name: "IX_RepairRecords_TechnicianProfileId",
                table: "RepairRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnershipTransfers",
                table: "OwnershipTransfers");

            migrationBuilder.DropIndex(
                name: "IX_OwnershipTransfers_DeviceId",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "ActionTaken",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "CorrectsRecordId",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "ProblemDescription",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "RecordType",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "RepairDate",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "TechnicianProfileId",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "WarrantyUntil",
                table: "RepairRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "InitiatingOwnerId",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OwnershipTransfers");

            migrationBuilder.DropColumn(
                name: "TargetBuyerId",
                table: "OwnershipTransfers");

            migrationBuilder.RenameTable(
                name: "RepairRecords",
                newName: "RepairRecord");

            migrationBuilder.RenameTable(
                name: "OwnershipTransfers",
                newName: "OwnershipTransfer");

            migrationBuilder.RenameIndex(
                name: "IX_RepairRecords_DeviceId",
                table: "RepairRecord",
                newName: "IX_RepairRecord_DeviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepairRecord",
                table: "RepairRecord",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnershipTransfer",
                table: "OwnershipTransfer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipTransfer_DeviceId",
                table: "OwnershipTransfer",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnershipTransfer_Devices_DeviceId",
                table: "OwnershipTransfer",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairRecord_Devices_DeviceId",
                table: "RepairRecord",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
