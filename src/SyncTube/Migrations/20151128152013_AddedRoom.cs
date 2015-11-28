using Microsoft.Data.Entity.Migrations;

namespace SyncTube.Migrations
{
    public partial class AddedRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Room", table => new
            {
                Id = table.Column<long>(nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CreatedById = table.Column<string>(nullable: true),
                Description = table.Column<string>(nullable: true),
                Duration = table.Column<int>(nullable: false),
                Name = table.Column<string>(nullable: true),
                VideoId = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey("FK_Room_ApplicationUser_CreatedById", x => x.CreatedById, "AspNetUsers", "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Room");
        }
    }
}