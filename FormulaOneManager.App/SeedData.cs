// Provides a small starter dataset used when the application launches.
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Domain.Teams;
using FormulaOneManager.Library.Services;

namespace FormulaOneManager.App;

// Centralizing the seed data keeps MainForm's constructor short and lets us
// use rich object-initializer lists, exactly the style suggested in the brief.
internal static class SeedData
{
    // Populates the supplied service with a few demo entities.
    public static void Populate(RacingService service)
    {
        // List of seed drivers using object-initializer syntax (initialization lists).
        Driver[] drivers = new Driver[]
        {
            new F1Driver
            {
                FirstName = "Lewis",
                LastName = "Hamilton",
                Nationality = "United Kingdom",
                Age = 41,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Fit,
                ChampionshipTitles = 7,
                PolePositions = 104,
                IsChampionshipContender = true,
                RegistrationDate = DateTime.Today.AddDays(-365)
            },
            new F1Driver
            {
                FirstName = "Max",
                LastName = "Verstappen",
                Nationality = "Netherlands",
                Age = 28,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Fit,
                ChampionshipTitles = 4,
                PolePositions = 40,
                IsChampionshipContender = true,
                RegistrationDate = DateTime.Today.AddDays(-200)
            },
            new F1Driver
            {
                FirstName = "Charles",
                LastName = "Leclerc",
                Nationality = "Monaco",
                Age = 28,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Injured,
                ChampionshipTitles = 0,
                PolePositions = 26,
                RegistrationDate = DateTime.Today.AddDays(-180)
            },
            new F2Driver
            {
                FirstName = "Andrea Kimi",
                LastName = "Antonelli",
                Nationality = "Italy",
                Age = 19,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Fit,
                IsJuniorAcademyMember = true,
                CurrentStanding = 2,
                RegistrationDate = DateTime.Today.AddDays(-90)
            },
            new F2Driver
            {
                FirstName = "Theo",
                LastName = "Pourchaire",
                Nationality = "France",
                Age = 22,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Suspended,
                IsJuniorAcademyMember = false,
                CurrentStanding = 5,
                RegistrationDate = DateTime.Today.AddDays(-60)
            },
            new KartingDriver
            {
                FirstName = "Oliver",
                LastName = "Bearman",
                Nationality = "United Kingdom",
                Age = 16,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Fit,
                KartingClass = "OK-Junior",
                SeasonWins = 5,
                RegistrationDate = DateTime.Today.AddDays(-30)
            },
            new RookieDriver
            {
                FirstName = "Jack",
                LastName = "Doohan",
                Nationality = "Australia",
                Age = 23,
                Gender = DriverGender.Male,
                Fitness = FitnessStatus.Fit,
                TestingKilometers = 4500,
                RegistrationDate = DateTime.Today.AddDays(-15)
            }
        };

        // Add every seed driver through the service so events fire normally.
        foreach (Driver driver in drivers)
            service.AddDriver(driver);

        // List of seed teams using object initializers as well.
        Team[] teams = new Team[]
        {
            new Team
            {
                TeamName = "Mercedes-AMG Petronas F1 Team",
                Headquarters = "Brackley",
                Country = "United Kingdom",
                BudgetMillions = 145,
                FoundedYear = 2010
            },
            new Team
            {
                TeamName = "Oracle Red Bull Racing",
                Headquarters = "Milton Keynes",
                Country = "United Kingdom",
                BudgetMillions = 145,
                FoundedYear = 2005
            },
            new Team
            {
                TeamName = "Scuderia Ferrari",
                Headquarters = "Maranello",
                Country = "Italy",
                BudgetMillions = 145,
                FoundedYear = 1929
            },
            new Team
            {
                TeamName = "McLaren F1 Team",
                Headquarters = "Woking",
                Country = "United Kingdom",
                BudgetMillions = 145,
                FoundedYear = 1963
            }
        };

        // Add every seed team via the service.
        foreach (Team team in teams)
            service.AddTeam(team);
    }
}
