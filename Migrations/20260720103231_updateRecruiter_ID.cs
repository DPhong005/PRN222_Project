using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class updateRecruiter_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'recruiter_id' AND object_id = OBJECT_ID('interview'))
                BEGIN
                    ALTER TABLE [interview] ADD [recruiter_id] int NOT NULL DEFAULT 0;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'recruiter_id' AND object_id = OBJECT_ID('package_transaction'))
                   AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_package_transaction_recruiter_id' AND object_id = OBJECT_ID('package_transaction'))
                BEGIN
                    CREATE INDEX [IX_package_transaction_recruiter_id] ON [package_transaction] ([recruiter_id]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_package_transaction_recruiter",
                table: "package_transaction");

            migrationBuilder.DropIndex(
                name: "IX_package_transaction_recruiter_id",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "recruiter_id",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "recruiter_id",
                table: "interview");
        }
    }
}
