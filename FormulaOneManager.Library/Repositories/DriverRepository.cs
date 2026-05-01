// Concrete repository for the Driver entity.
using FormulaOneManager.Library.Domain.Drivers;

namespace FormulaOneManager.Library.Repositories;

// Inherits all CRUD logic from Repository<T> and only adds driver-specific helpers.
public class DriverRepository : Repository<Driver>
{
    // LINQ powered helper returning every driver currently available to sign.
    public IEnumerable<Driver> GetAvailable() =>
        GetAll().Where(d => d.IsAvailable());

    // LINQ powered helper grouping drivers by their racing category.
    public IEnumerable<IGrouping<string, Driver>> GroupByCategory() =>
        GetAll().GroupBy(d => d.Category);
}
