using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeterinaryClinic.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiagnosisId",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Species = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Diagnoses",
                columns: new[] { "Id", "Description", "Name", "Observations", "Species", "Treatment" },
                values: new object[,]
                {
                    { 1, "Inflamație a urechii externe", "Otită externă", "Monitorizare săptămânală. Evitați intrarea apei în urechi.", "Câine", "Curățare auriculară cu soluție antiseptică. Picături auriculare cu antibiotic și antiinflamator (ex: Otomax) timp de 7-10 zile." },
                    { 2, "Reacție alergică cutanată", "Dermatită alergică", "Identificați și eliminați alergenul. Dietă hipoalergenică recomandată.", "", "Antihistaminice (Cetirizină 5mg/zi). Șampoane hipoalergenice. Corticosteroizi în cazuri severe (Prednisolon 0.5mg/kg/zi, 5 zile)." },
                    { 3, "Inflamație a tractului gastrointestinal", "Gastroenterită", "Monitorizați hidratarea. Dacă vărsăturile persistă peste 24h, repetați consultul.", "", "Dietă blândă 48-72h (orez fiert + pui fiert fără sare). Hidratare corespunzătoare. Probiotice (Fortiflora 1 plic/zi, 7 zile). Metronidazol 15mg/kg de 2 ori pe zi, 5 zile dacă este necesar." },
                    { 4, "Infecție bacteriană a tractului urinar", "Infecție urinară", "Recoltați urocultură dacă infecțiile sunt recurente. Reevaluare la 2 săptămâni.", "", "Amoxicilină-clavulanat 15mg/kg de 2 ori pe zi, 10-14 zile. Creșteți aportul de apă. Urocolvin sau alt supliment urinar." },
                    { 5, "Inflamație a conjunctivei oculare", "Conjunctivită", "Evitați expunerea la vânt și praf. Dacă nu se ameliorează în 5 zile, consultați un oftalmolog veterinar.", "", "Curățarea ochilor cu ser fiziologic steril. Picături oftalmice cu antibiotic (Tobramicină sau Cloramfenicol) de 3 ori pe zi, 7 zile." },
                    { 6, "Infestare cu paraziți intestinali", "Parazitoză intestinală", "Igienizarea mediului. Deparazitare și la alți animale de companie din gospodărie.", "", "Fenbendazol 50mg/kg/zi, 3-5 zile sau Pyrantel 5mg/kg doză unică. Deparazitare internă periodică la 3-6 luni." },
                    { 7, "Insuficiență a reglării glicemice", "Diabet zaharat", "Educarea proprietarului privind semnele de hipoglicemie. Control lunar.", "", "Insulină (dozaj individualizat în funcție de glicemie). Dietă cu conținut redus de carbohidrați. Monitorizare glicemie zilnică." },
                    { 8, "Producție insuficientă de hormoni tiroidieni", "Hipotiroidism", "Tratament pe viață. Control tiroidian la 6 luni.", "Câine", "Levotiroxină sodică 0.02mg/kg de 2 ori pe zi. Ajustarea dozei după 4-6 săptămâni în funcție de TSH." },
                    { 9, "Inflamație a mucoasei bucale", "Stomatită", "Periaj dentar regulat. Dietă cu hrană umedă în perioada de recuperare.", "", "Curățare dentară profesională sub anestezie. Antibiotice (Clindamicină 5mg/kg de 2 ori pe zi, 10 zile). Gel oral antiseptic local." },
                    { 10, "Inflamație articulară degenerativă", "Artrită", "Evitați efortul fizic intens. Suprafețe moi pentru odihnă. Monitorizare renală la tratament lung cu AINS.", "", "Meloxicam 0.1mg/kg/zi cu mâncare. Suplimente condroprotectoare (Glucozamină + Condroitin). Fizioterapie recomandată." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DiagnosisId",
                table: "MedicalRecords",
                column: "DiagnosisId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Diagnoses_DiagnosisId",
                table: "MedicalRecords",
                column: "DiagnosisId",
                principalTable: "Diagnoses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Diagnoses_DiagnosisId",
                table: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DiagnosisId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DiagnosisId",
                table: "MedicalRecords");
        }
    }
}
