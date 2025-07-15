using Application.Users.Expertise.Get;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Database.Seed.Users;

internal static class LanguageExpertiseSeeder
{
    public static List<Language> Languages()
    {
        var languages = new List<Language>
        {
            new Language("English" , 1 ),
            new Language("French" , 2),
            new Language("Arabic" , 3)
        };
        return languages;
    }
    public static List<Expertise> Expertises()
    {
        var expertises = new List<Expertise>
        {
            new Expertise("Software Engineering", "Mentorship in web, backend, mobile, etc.", 1),
            new Expertise("Mechanical Engineering", "Mentorship in machines, manufacturing, CAD, etc.", 2),
            new Expertise("Electrical Engineering", "Mentorship in circuits, power systems, etc.", 3),
            new Expertise("Civil Engineering", "Mentorship in construction, infrastructure, etc.", 4),
            new Expertise("Chemical Engineering", "Mentorship in chemical processes, materials, etc.", 5),
            new Expertise("Aerospace Engineering", "Mentorship in aircraft, spacecraft, etc.", 6),
            new Expertise("Environmental Engineering", "Mentorship in sustainability, environment, etc.", 7),

            new Expertise("Web Development", "Frontend, backend, and fullstack web mentoring.", 8),
            new Expertise("Mobile Development", "Android, iOS, cross-platform app mentoring.", 9),
            new Expertise("Data Science", "Mentorship in data analysis, ML, statistics.", 10),
            new Expertise("Cybersecurity", "Mentorship in ethical hacking, defense, etc.", 11),
            new Expertise("Cloud & DevOps", "AWS, Azure, CI/CD, and infrastructure mentoring.", 12),
            new Expertise("AI & Machine Learning", "Mentorship in ML models, AI theory, etc.", 13),
            new Expertise("UI/UX Design", "Mentorship in user experience and interface design.", 14),

            new Expertise("Startup Coaching", "Mentorship for startup founders and entrepreneurs.", 15),
            new Expertise("Business Strategy", "Mentorship in business models and scaling.", 16),
            new Expertise("Marketing & Branding", "Mentorship in digital marketing, social media, etc.", 17),
            new Expertise("Sales", "Mentorship in B2B, B2C, pitching, etc.", 18),
            new Expertise("E-commerce", "Mentorship in online business and marketplaces.", 19),
            new Expertise("Product Management", "Mentorship in product lifecycle, agile, etc.", 20),
            new Expertise("Project Management", "Mentorship in managing teams and tasks.", 21),

            new Expertise("Investment", "Mentorship in stocks, real estate, etc.", 22),
            new Expertise("Personal Finance", "Budgeting, saving, financial planning.", 23),
            new Expertise("Accounting & Auditing", "Corporate and freelance financial help.", 24),

            new Expertise("General Medicine", "Medical school and residency mentorship.", 25),
            new Expertise("Nursing", "Clinical mentorship and nursing school support.", 26),
            new Expertise("Pharmacy", "Pharmaceutical career and education guidance.", 27),
            new Expertise("Mental Health", "Psychology, therapy, and emotional support.", 28),
            new Expertise("Public Health", "Mentorship in epidemiology, policy, etc.", 29),

            new Expertise("Corporate Law", "Mentorship in contracts, companies, etc.", 30),
            new Expertise("Criminal Law", "Legal career coaching in criminal justice.", 31),
            new Expertise("Immigration Law", "Mentorship in visa and immigration processes.", 32),
            new Expertise("Intellectual Property Law", "Mentorship in patents, copyrights, etc.", 33),

            new Expertise("Leadership & Management", "Mentorship in team building and leading.", 34),
            new Expertise("Communication Skills", "Mentorship in effective communication.", 35),
            new Expertise("Time Management", "Coaching on personal productivity.", 36),
            new Expertise("Public Speaking", "Confidence building and speech coaching.", 37),
            new Expertise("Job Interview Coaching", "Mock interviews and job prep.", 38),
            new Expertise("Resume & LinkedIn Review", "Profile and resume optimization.", 39),
            new Expertise("Career Planning", "Long-term goal mentorship.", 40),

            new Expertise("Graphic Design", "Mentorship in tools like Photoshop, Figma.", 41),
            new Expertise("Photography", "Camera use, editing, and career advice.", 42),
            new Expertise("Music Production", "Mentorship in composition, mixing, etc.", 43),
            new Expertise("Writing & Publishing", "Mentorship in writing books or articles.", 44),
            new Expertise("Video Editing", "Mentorship in Premiere Pro, storytelling, etc.", 45),
            new Expertise("Game Development", "Unity, Unreal, and career mentorship.", 46),
            new Expertise("Language Learning", "Mentorship for language fluency.", 47),
            new Expertise("Study Abroad Guidance", "Mentorship for international students.", 48),
            new Expertise("Life Coaching", "Mentorship in motivation, habits, etc.", 49),
            new Expertise("Productivity Coaching", "System building, discipline, deep work.", 50)
        };
        return expertises;
    }
}