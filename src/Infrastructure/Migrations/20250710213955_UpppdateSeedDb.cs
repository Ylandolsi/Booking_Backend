using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpppdateSeedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name_first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status_is_mentor = table.Column<bool>(type: "boolean", nullable: false),
                    status_is_active = table.Column<bool>(type: "boolean", nullable: false),
                    profile_picture_url_profile_picture_link = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    profile_picture_url_thumbnail_url_picture_link = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    social_links_linked_in = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_twitter = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_github = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_youtube = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_facebook = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_instagram = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    social_links_portfolio = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    profile_completion_status_has_name = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_email = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_profile_picture = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_bio = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_gender = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_social_links = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_education = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_languages = table.Column<bool>(type: "boolean", nullable: false),
                    profile_completion_status_has_expertise = table.Column<bool>(type: "boolean", nullable: false),
                    bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "expertises",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expertises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "public",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "educations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    field = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    university = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    to_present = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_educations", x => x.id);
                    table.ForeignKey(
                        name: "fk_educations_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "email_verification_token",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_verification_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_verification_token_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "experiences",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    company_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    to_present = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_experiences", x => x.id);
                    table.ForeignKey(
                        name: "fk_experiences_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_ip = table.Column<string>(type: "text", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_mentors",
                schema: "public",
                columns: table => new
                {
                    mentor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mentee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_mentors", x => new { x.mentor_id, x.mentee_id });
                    table.ForeignKey(
                        name: "fk_user_mentors_asp_net_users_mentee_id",
                        column: x => x.mentee_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_mentors_asp_net_users_mentor_id",
                        column: x => x.mentor_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_expertises",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expertise_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_expertises", x => new { x.user_id, x.expertise_id });
                    table.ForeignKey(
                        name: "fk_user_expertises_expertises_expertise_id",
                        column: x => x.expertise_id,
                        principalSchema: "public",
                        principalTable: "expertises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_expertises_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_languages",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_languages", x => new { x.user_id, x.language_id });
                    table.ForeignKey(
                        name: "fk_user_languages_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "public",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_languages_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                schema: "public",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                schema: "public",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                schema: "public",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                schema: "public",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_educations_user_id",
                schema: "public",
                table: "educations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_email_verification_token_user_id",
                schema: "public",
                table: "email_verification_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_experiences_user_id",
                schema: "public",
                table: "experiences",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on_utc",
                schema: "public",
                table: "outbox_messages",
                column: "processed_on_utc");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                schema: "public",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "public",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_expertises_expertise_id",
                schema: "public",
                table: "user_expertises",
                column: "expertise_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_languages_language_id",
                schema: "public",
                table: "user_languages",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_mentors_mentee_id",
                schema: "public",
                table: "user_mentors",
                column: "mentee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "educations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "email_verification_token",
                schema: "public");

            migrationBuilder.DropTable(
                name: "experiences",
                schema: "public");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_expertises",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_languages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_mentors",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "expertises",
                schema: "public");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "public");
        }
    }
}
