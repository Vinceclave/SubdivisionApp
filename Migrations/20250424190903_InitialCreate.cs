using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subdivision.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditDebitCard",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardholderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditDebitCard", x => x.CardId);
                });

            migrationBuilder.CreateTable(
                name: "OnlineBanking",
                columns: table => new
                {
                    OnlineBankingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionRefNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineBanking", x => x.OnlineBankingId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    LoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.LoginId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Users_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Users",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Homeowners",
                columns: table => new
                {
                    HomeownerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homeowners", x => x.HomeownerId);
                    table.ForeignKey(
                        name: "FK_Homeowners_Users_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Users",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_Staffs_Users_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Users",
                        principalColumn: "LoginId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateTimePosted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnouncementId);
                    table.ForeignKey(
                        name: "FK_Announcements_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactPersonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contacts_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventCalendars",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EventDesc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Visibility = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCalendars", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_EventCalendars_Admins_OrganizedById",
                        column: x => x.OrganizedById,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    FacilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeOfFacility = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FacilityCapacity = table.Column<int>(type: "int", nullable: false),
                    FacilityImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkingDays = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RulesRegulations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.FacilityId);
                    table.ForeignKey(
                        name: "FK_Facilities_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId");
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    BillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    BillType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.BillId);
                    table.ForeignKey(
                        name: "FK_Bills_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    ComplaintContent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ComplaintDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_Complaints_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    FeedbackContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitorPassRequests",
                columns: table => new
                {
                    VisitorPassRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    VisitorFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurposeOfVisit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeOfVehicle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlateNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfVisit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOfVisit = table.Column<TimeSpan>(type: "time", nullable: false),
                    VisitorStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorPassRequests", x => x.VisitorPassRequestId);
                    table.ForeignKey(
                        name: "FK_VisitorPassRequests_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forums",
                columns: table => new
                {
                    ForumId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DateTimePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ForumTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forums", x => x.ForumId);
                    table.ForeignKey(
                        name: "FK_Forums_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId");
                    table.ForeignKey(
                        name: "FK_Forums_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId");
                    table.ForeignKey(
                        name: "FK_Forums_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "StaffId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.ServiceRequestId);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "StaffId");
                });

            migrationBuilder.CreateTable(
                name: "EventVisibilities",
                columns: table => new
                {
                    EventVisibilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    LoginId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVisibilities", x => x.EventVisibilityId);
                    table.ForeignKey(
                        name: "FK_EventVisibilities_EventCalendars_EventId",
                        column: x => x.EventId,
                        principalTable: "EventCalendars",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVisibilities_Users_LoginId",
                        column: x => x.LoginId,
                        principalTable: "Users",
                        principalColumn: "LoginId");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    HomeownerId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ReservationTimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationTimeOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfReservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId");
                    table.ForeignKey(
                        name: "FK_Reservations_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId");
                    table.ForeignKey(
                        name: "FK_Reservations_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ModeOfPayment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfPayment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: true),
                    OnlineBankingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "BillId");
                    table.ForeignKey(
                        name: "FK_Payments_CreditDebitCard_CardId",
                        column: x => x.CardId,
                        principalTable: "CreditDebitCard",
                        principalColumn: "CardId");
                    table.ForeignKey(
                        name: "FK_Payments_OnlineBanking_OnlineBankingId",
                        column: x => x.OnlineBankingId,
                        principalTable: "OnlineBanking",
                        principalColumn: "OnlineBankingId");
                });

            migrationBuilder.CreateTable(
                name: "VisitorPasses",
                columns: table => new
                {
                    VisitorPassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitorPassRequestId = table.Column<int>(type: "int", nullable: false),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PassStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoggedById = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorPasses", x => x.VisitorPassId);
                    table.ForeignKey(
                        name: "FK_VisitorPasses_Staffs_LoggedById",
                        column: x => x.LoggedById,
                        principalTable: "Staffs",
                        principalColumn: "StaffId");
                    table.ForeignKey(
                        name: "FK_VisitorPasses_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "StaffId");
                    table.ForeignKey(
                        name: "FK_VisitorPasses_VisitorPassRequests_VisitorPassRequestId",
                        column: x => x.VisitorPassRequestId,
                        principalTable: "VisitorPassRequests",
                        principalColumn: "VisitorPassRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumReplies",
                columns: table => new
                {
                    ForumRepliesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForumId = table.Column<int>(type: "int", nullable: false),
                    HomeownerId = table.Column<int>(type: "int", nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    RepliedContent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumReplies", x => x.ForumRepliesId);
                    table.ForeignKey(
                        name: "FK_ForumReplies_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId");
                    table.ForeignKey(
                        name: "FK_ForumReplies_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "ForumId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumReplies_Homeowners_HomeownerId",
                        column: x => x.HomeownerId,
                        principalTable: "Homeowners",
                        principalColumn: "HomeownerId");
                    table.ForeignKey(
                        name: "FK_ForumReplies_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "StaffId");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "LoginId", "Address", "Email", "FirstName", "Image", "LastName", "Password", "PhoneNumber", "UserType", "Username" },
                values: new object[] { 1, "Admin Address", "admin@example.com", "Admin", "default.png", "User", "$2a$11$8icRySkuU81x31OMdr7m4emQJNBInJzOzSoe.Okg0uISEsMW4YL5i", "0000000000", 0, "admin" });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminId", "LoginId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_LoginId",
                table: "Admins",
                column: "LoginId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_AdminId",
                table: "Announcements",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_HomeownerId",
                table: "Bills",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_HomeownerId",
                table: "Complaints",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_AdminId",
                table: "Contacts",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCalendars_OrganizedById",
                table: "EventCalendars",
                column: "OrganizedById");

            migrationBuilder.CreateIndex(
                name: "IX_EventVisibilities_EventId",
                table: "EventVisibilities",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVisibilities_LoginId",
                table: "EventVisibilities",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_AdminId",
                table: "Facilities",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_HomeownerId",
                table: "Feedbacks",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReplies_AdminId",
                table: "ForumReplies",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReplies_ForumId",
                table: "ForumReplies",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReplies_HomeownerId",
                table: "ForumReplies",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReplies_StaffId",
                table: "ForumReplies",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Forums_AdminId",
                table: "Forums",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Forums_HomeownerId",
                table: "Forums",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_Forums_StaffId",
                table: "Forums",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Homeowners_LoginId",
                table: "Homeowners",
                column: "LoginId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillId",
                table: "Payments",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CardId",
                table: "Payments",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OnlineBankingId",
                table: "Payments",
                column: "OnlineBankingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AdminId",
                table: "Reservations",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FacilityId",
                table: "Reservations",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_HomeownerId",
                table: "Reservations",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_HomeownerId",
                table: "ServiceRequests",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_StaffId",
                table: "ServiceRequests",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_LoginId",
                table: "Staffs",
                column: "LoginId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_HomeownerId",
                table: "Vehicles",
                column: "HomeownerId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPasses_LoggedById",
                table: "VisitorPasses",
                column: "LoggedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPasses_StaffId",
                table: "VisitorPasses",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPasses_VisitorPassRequestId",
                table: "VisitorPasses",
                column: "VisitorPassRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPassRequests_HomeownerId",
                table: "VisitorPassRequests",
                column: "HomeownerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "EventVisibilities");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "ForumReplies");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VisitorPasses");

            migrationBuilder.DropTable(
                name: "EventCalendars");

            migrationBuilder.DropTable(
                name: "Forums");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "CreditDebitCard");

            migrationBuilder.DropTable(
                name: "OnlineBanking");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "VisitorPassRequests");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Homeowners");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
