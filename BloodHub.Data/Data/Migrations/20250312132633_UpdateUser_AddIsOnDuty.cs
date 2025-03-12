using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodHub.Data.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser_AddIsOnDuty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnDuty",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$F.nG7dj.15e939XyEDBMqOCNwRhgnm363uk.UVVpHKqkjO1E1kue6");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$BwVr5lfHGODkVNoH5MiuhOz5xXpOc5wfXNOiCbalbyRJmKDSRdpe6");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$zFQqCUE3L1eyYcXyZSrvfOreGHN6JCnLmkIz36Kk3ztb2wxw3jWTW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnDuty",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$vowlXRmZgClkuqDT3AVI3eChb0m9yzCJsmaGVwts9Rc48OJISVgk6");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$uvModtbqGarYoFdeTkUj4egb0j2dGoSzM.XeBQnyFCXRqAWPEh6Qq");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$JyjxKy1uP1McMS4iWNHVjuPf5vXXQId4NogVRE2iY0WvMJE2dBD8q");
        }
    }
}
