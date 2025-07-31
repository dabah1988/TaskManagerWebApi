using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateTaskManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateOfStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamSize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "ProjectId", "DateOfStart", "ProjectDescription", "ProjectName", "TeamSize" },
                values: new object[,]
                {
                    { new Guid("1a12e199-b289-4d38-961e-12345876abcd"), new DateTime(2022, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Développement d'une application mobile pour la gestion RH en interne.", "Application RH Mobile", 4 },
                    { new Guid("2c34e0d9-21dd-4f8c-bb6a-b7fdfabcd123"), new DateTime(2023, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mise en place d’un chatbot IA pour le recrutement automatique sur LinkedIn.", "Bot de recrutement", 2 },
                    { new Guid("34cd5c45-0f7a-4f1c-8ee9-98765432abcd"), new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refonte complète de la plateforme e-commerce avec Angular et .NET Core.", "Refonte e-commerce", 8 },
                    { new Guid("889fa0a1-435f-42b0-b254-cccdeedc0001"), new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Développement d’un tableau de bord interactif avec Power BI et Blazor.", "Dashboard analytique", 6 },
                    { new Guid("9be8f999-cccd-44d7-a4f1-5eaa87654321"), new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Projet de traitement de gros volumes de données clients pour recommandations.", "Analyse Big Data", 9 },
                    { new Guid("a341f66b-98cb-40b7-a6a2-90f3b33cf010"), new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Création d'une API pour la cartographie des utilisateurs à l'international.", "API géolocalisation", 3 },
                    { new Guid("bd123c8a-9832-4ed8-a1b5-7ab32ca45678"), new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Plateforme de réservation multilingue pour des services médicaux.", "Système de réservation", 4 },
                    { new Guid("d8f1e128-1d1d-4f53-a8cc-bd79217b2a05"), new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Migration complète de l'infrastructure vers Azure avec automatisation CI/CD.", "Migration Cloud Azure", 6 },
                    { new Guid("e109eabc-12aa-42ff-9090-aba123aa3456"), new DateTime(2023, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Développement d'une solution IoT pour surveiller les machines en temps réel.", "Monitoring industriel", 5 },
                    { new Guid("fda43c8b-c503-4f5b-81e1-1098ae01bcde"), new DateTime(2022, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Développement d’un outil CRM adapté aux besoins d’un client BTP.", "Outil CRM sur mesure", 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
