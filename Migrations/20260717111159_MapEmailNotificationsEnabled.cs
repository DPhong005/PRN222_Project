using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class MapEmailNotificationsEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE name = 'EmailNotificationsEnabled' AND object_id = OBJECT_ID('user_account')
                )
                BEGIN
                    EXEC sp_rename 'user_account.EmailNotificationsEnabled', 'email_notifications_enabled', 'COLUMN';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email_notifications_enabled",
                table: "user_account",
                newName: "EmailNotificationsEnabled");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailNotificationsEnabled",
                table: "user_account",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }
    }
}
