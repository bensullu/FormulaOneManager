// Concrete repository for the Team entity.
using FormulaOneManager.Library.Domain.Teams;

namespace FormulaOneManager.Library.Repositories;

// Only declares team-specific helpers; CRUD lives in the base class.
public class TeamRepository : Repository<Team>
{
    // Returns teams headquartered in the given country using a LINQ filter.
    public IEnumerable<Team> GetByCountry(string country) =>
        GetAll().Where(t => string.Equals(t.Country, country, StringComparison.OrdinalIgnoreCase));
}
