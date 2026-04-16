using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QmaService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasError = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Op1Value = table.Column<double>(type: "double precision", nullable: false),
                    Op1Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Op1Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Op2Value = table.Column<double>(type: "double precision", nullable: true),
                    Op2Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Op2Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultValue = table.Column<double>(type: "double precision", nullable: true),
                    ResultUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "measurements");
        }
    }
}
