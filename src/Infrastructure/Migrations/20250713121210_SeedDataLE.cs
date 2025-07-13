using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "public",
                table: "expertises",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Mentorship in web, backend, mobile, etc.", "Software Engineering" },
                    { 2, "Mentorship in machines, manufacturing, CAD, etc.", "Mechanical Engineering" },
                    { 3, "Mentorship in circuits, power systems, etc.", "Electrical Engineering" },
                    { 4, "Mentorship in construction, infrastructure, etc.", "Civil Engineering" },
                    { 5, "Mentorship in chemical processes, materials, etc.", "Chemical Engineering" },
                    { 6, "Mentorship in aircraft, spacecraft, etc.", "Aerospace Engineering" },
                    { 7, "Mentorship in sustainability, environment, etc.", "Environmental Engineering" },
                    { 8, "Frontend, backend, and fullstack web mentoring.", "Web Development" },
                    { 9, "Android, iOS, cross-platform app mentoring.", "Mobile Development" },
                    { 10, "Mentorship in data analysis, ML, statistics.", "Data Science" },
                    { 11, "Mentorship in ethical hacking, defense, etc.", "Cybersecurity" },
                    { 12, "AWS, Azure, CI/CD, and infrastructure mentoring.", "Cloud & DevOps" },
                    { 13, "Mentorship in ML models, AI theory, etc.", "AI & Machine Learning" },
                    { 14, "Mentorship in user experience and interface design.", "UI/UX Design" },
                    { 15, "Mentorship for startup founders and entrepreneurs.", "Startup Coaching" },
                    { 16, "Mentorship in business models and scaling.", "Business Strategy" },
                    { 17, "Mentorship in digital marketing, social media, etc.", "Marketing & Branding" },
                    { 18, "Mentorship in B2B, B2C, pitching, etc.", "Sales" },
                    { 19, "Mentorship in online business and marketplaces.", "E-commerce" },
                    { 20, "Mentorship in product lifecycle, agile, etc.", "Product Management" },
                    { 21, "Mentorship in managing teams and tasks.", "Project Management" },
                    { 22, "Mentorship in stocks, real estate, etc.", "Investment" },
                    { 23, "Budgeting, saving, financial planning.", "Personal Finance" },
                    { 24, "Corporate and freelance financial help.", "Accounting & Auditing" },
                    { 25, "Medical school and residency mentorship.", "General Medicine" },
                    { 26, "Clinical mentorship and nursing school support.", "Nursing" },
                    { 27, "Pharmaceutical career and education guidance.", "Pharmacy" },
                    { 28, "Psychology, therapy, and emotional support.", "Mental Health" },
                    { 29, "Mentorship in epidemiology, policy, etc.", "Public Health" },
                    { 30, "Mentorship in contracts, companies, etc.", "Corporate Law" },
                    { 31, "Legal career coaching in criminal justice.", "Criminal Law" },
                    { 32, "Mentorship in visa and immigration processes.", "Immigration Law" },
                    { 33, "Mentorship in patents, copyrights, etc.", "Intellectual Property Law" },
                    { 34, "Mentorship in team building and leading.", "Leadership & Management" },
                    { 35, "Mentorship in effective communication.", "Communication Skills" },
                    { 36, "Coaching on personal productivity.", "Time Management" },
                    { 37, "Confidence building and speech coaching.", "Public Speaking" },
                    { 38, "Mock interviews and job prep.", "Job Interview Coaching" },
                    { 39, "Profile and resume optimization.", "Resume & LinkedIn Review" },
                    { 40, "Long-term goal mentorship.", "Career Planning" },
                    { 41, "Mentorship in tools like Photoshop, Figma.", "Graphic Design" },
                    { 42, "Camera use, editing, and career advice.", "Photography" },
                    { 43, "Mentorship in composition, mixing, etc.", "Music Production" },
                    { 44, "Mentorship in writing books or articles.", "Writing & Publishing" },
                    { 45, "Mentorship in Premiere Pro, storytelling, etc.", "Video Editing" },
                    { 46, "Unity, Unreal, and career mentorship.", "Game Development" },
                    { 47, "Mentorship for language fluency.", "Language Learning" },
                    { 48, "Mentorship for international students.", "Study Abroad Guidance" },
                    { 49, "Mentorship in motivation, habits, etc.", "Life Coaching" },
                    { 50, "System building, discipline, deep work.", "Productivity Coaching" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "languages",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "English" },
                    { 2, "French" },
                    { 3, "Arabic" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "expertises",
                keyColumn: "id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "languages",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "languages",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "languages",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
