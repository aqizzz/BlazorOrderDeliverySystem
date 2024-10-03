using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderDeliverySystem.Share.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActived = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Province = table.Column<string>(type: "TEXT", nullable: true),
                    Postcode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryWorkers",
                columns: table => new
                {
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<int>(type: "INTEGER", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    LastTaskAssigned = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryWorkers", x => x.WorkerId);
                    table.ForeignKey(
                        name: "FK_DeliveryWorkers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    MerchantId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BusinessName = table.Column<string>(type: "TEXT", nullable: true),
                    MerchantPic = table.Column<string>(type: "TEXT", nullable: true),
                    MerchantDescription = table.Column<string>(type: "TEXT", nullable: true),
                    PreparingTime = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.MerchantId);
                    table.ForeignKey(
                        name: "FK_Merchants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerLocations",
                columns: table => new
                {
                    WorkerLocationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(11, 8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11, 8)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerLocations", x => x.WorkerLocationId);
                    table.ForeignKey(
                        name: "FK_WorkerLocations_DeliveryWorkers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "DeliveryWorkers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MerchantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: true),
                    ItemDescription = table.Column<string>(type: "TEXT", nullable: true),
                    ItemPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ItemPic = table.Column<string>(type: "TEXT", nullable: true),
                    IsAvailable = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MerchantId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: true),
                    PickupAddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    DropoffAddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeliveryWorkerWorkerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.CheckConstraint("CK_Order_Status", "Status IN ('Pending', 'Approved', 'In Delivery', 'Delivered', 'Cancelled')");
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_DropoffAddressId",
                        column: x => x.DropoffAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_PickupAddressId",
                        column: x => x.PickupAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_DeliveryWorkers_DeliveryWorkerWorkerId",
                        column: x => x.DeliveryWorkerWorkerId,
                        principalTable: "DeliveryWorkers",
                        principalColumn: "WorkerId");
                    table.ForeignKey(
                        name: "FK_Orders_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "Merchants",
                        principalColumn: "MerchantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTasks", x => x.TaskId);
                    table.CheckConstraint("CK_DeliveryTask_Status", "Status IN ('Assigned', 'Completed', 'Failed')");
                    table.ForeignKey(
                        name: "FK_DeliveryTasks_DeliveryWorkers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "DeliveryWorkers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryTasks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceAtOrder = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5, 2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(5, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reply = table.Column<string>(type: "TEXT", nullable: true),
                    ReplyCreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.CheckConstraint("CK_Reviews_Rating", "Rating BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId_Type",
                table: "Addresses",
                columns: new[] { "UserId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ItemId",
                table: "CartItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTasks_OrderId",
                table: "DeliveryTasks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTasks_WorkerId",
                table: "DeliveryTasks",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryWorkers_UserId",
                table: "DeliveryWorkers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_MerchantId",
                table: "Items",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_UserId",
                table: "Merchants",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryWorkerWorkerId",
                table: "Orders",
                column: "DeliveryWorkerWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DropoffAddressId",
                table: "Orders",
                column: "DropoffAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MerchantId",
                table: "Orders",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PickupAddressId",
                table: "Orders",
                column: "PickupAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerLocations_WorkerId",
                table: "WorkerLocations",
                column: "WorkerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "DeliveryTasks");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "WorkerLocations");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DeliveryWorkers");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
