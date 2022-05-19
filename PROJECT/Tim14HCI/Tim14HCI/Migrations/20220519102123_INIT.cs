using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tim14HCI.Migrations
{
    public partial class INIT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stations",
                columns: table => new
                {
                    StationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.StationID);
                });

            migrationBuilder.CreateTable(
                name: "trains",
                columns: table => new
                {
                    TrainID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    MaxSpeed = table.Column<float>(nullable: false),
                    Capacity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trains", x => x.TrainID);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UserRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "linkedStations",
                columns: table => new
                {
                    Station1ID = table.Column<int>(nullable: false),
                    Station2ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_linkedStations", x => new { x.Station1ID, x.Station2ID });
                    table.ForeignKey(
                        name: "FK_linkedStations_stations_Station1ID",
                        column: x => x.Station1ID,
                        principalTable: "stations",
                        principalColumn: "StationID");
                    table.ForeignKey(
                        name: "FK_linkedStations_stations_Station2ID",
                        column: x => x.Station2ID,
                        principalTable: "stations",
                        principalColumn: "StationID");
                });

            migrationBuilder.CreateTable(
                name: "trainLines",
                columns: table => new
                {
                    TrainLineID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartStationID = table.Column<int>(nullable: false),
                    EndStationID = table.Column<int>(nullable: false),
                    TrainID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trainLines", x => x.TrainLineID);
                    table.ForeignKey(
                        name: "FK_trainLines_stations_StartStationID",
                        column: x => x.StartStationID,
                        principalTable: "stations",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trainLines_trains_TrainID",
                        column: x => x.TrainID,
                        principalTable: "trains",
                        principalColumn: "TrainID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departures",
                columns: table => new
                {
                    DepartureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startDate = table.Column<DateTime>(nullable: false),
                    endDate = table.Column<DateTime>(nullable: false),
                    StartTimeEveryday = table.Column<DateTime>(nullable: false),
                    TrainLineID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departures", x => x.DepartureID);
                    table.ForeignKey(
                        name: "FK_departures_trainLines_TrainLineID",
                        column: x => x.TrainLineID,
                        principalTable: "trainLines",
                        principalColumn: "TrainLineID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "onWayStations",
                columns: table => new
                {
                    StationOrder = table.Column<int>(nullable: false),
                    TrainLineID = table.Column<int>(nullable: false),
                    Price = table.Column<float>(nullable: false),
                    Time = table.Column<float>(nullable: false),
                    StationID = table.Column<int>(nullable: false),
                    TrainLineID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onWayStations", x => new { x.TrainLineID, x.StationOrder });
                    table.ForeignKey(
                        name: "FK_onWayStations_stations_StationID",
                        column: x => x.StationID,
                        principalTable: "stations",
                        principalColumn: "StationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_onWayStations_trainLines_TrainLineID",
                        column: x => x.TrainLineID,
                        principalTable: "trainLines",
                        principalColumn: "TrainLineID");
                    table.ForeignKey(
                        name: "FK_onWayStations_trainLines_TrainLineID1",
                        column: x => x.TrainLineID1,
                        principalTable: "trainLines",
                        principalColumn: "TrainLineID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    TicketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForReservation = table.Column<bool>(nullable: false),
                    DepartureID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_tickets_departures_DepartureID",
                        column: x => x.DepartureID,
                        principalTable: "departures",
                        principalColumn: "DepartureID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_users_UserID",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "stations",
                columns: new[] { "StationID", "Name" },
                values: new object[,]
                {
                    { 1, "Beograd" },
                    { 2, "Novi Sad" },
                    { 3, "Subotica" },
                    { 4, "Zrenjanin" },
                    { 5, "Niš" },
                    { 6, "Leskovac" }
                });

            migrationBuilder.InsertData(
                table: "trains",
                columns: new[] { "TrainID", "Capacity", "MaxSpeed", "Name" },
                values: new object[,]
                {
                    { 1, 120, 160f, "Soko" },
                    { 2, 50, 60f, "Cira" },
                    { 3, 100, 100f, "Krsko" },
                    { 4, 200, 200f, "Soko II" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "UserID", "Email", "FirstName", "LastName", "Password", "Phone", "UserRole" },
                values: new object[,]
                {
                    { 1, "mg00@gmail.com", "Mladen", "Gajic", "mladen123", "05603051", 1 },
                    { 2, "ns00@gmail.com", "Nikola", "Stepic", "aaa", "123432", 1 },
                    { 3, "jj00@gmail.com", "Jovan", "Jovanovic", "bbb", "214t5654", 0 },
                    { 4, "dt00@gmail.com", "Djordje", "Tomic", "ccc", "+381064428108", 0 }
                });

            migrationBuilder.InsertData(
                table: "linkedStations",
                columns: new[] { "Station1ID", "Station2ID" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 1, 5 },
                    { 5, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_departures_TrainLineID",
                table: "departures",
                column: "TrainLineID");

            migrationBuilder.CreateIndex(
                name: "IX_linkedStations_Station2ID",
                table: "linkedStations",
                column: "Station2ID");

            migrationBuilder.CreateIndex(
                name: "IX_onWayStations_StationID",
                table: "onWayStations",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_onWayStations_TrainLineID1",
                table: "onWayStations",
                column: "TrainLineID1",
                unique: true,
                filter: "[TrainLineID1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_DepartureID",
                table: "tickets",
                column: "DepartureID");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_UserID",
                table: "tickets",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_trainLines_StartStationID",
                table: "trainLines",
                column: "StartStationID");

            migrationBuilder.CreateIndex(
                name: "IX_trainLines_TrainID",
                table: "trainLines",
                column: "TrainID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "linkedStations");

            migrationBuilder.DropTable(
                name: "onWayStations");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "departures");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "trainLines");

            migrationBuilder.DropTable(
                name: "stations");

            migrationBuilder.DropTable(
                name: "trains");
        }
    }
}
