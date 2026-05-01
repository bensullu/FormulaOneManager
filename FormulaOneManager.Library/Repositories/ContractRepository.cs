// Concrete repository for the Contract entity.
using FormulaOneManager.Library.Domain.Contracts;

namespace FormulaOneManager.Library.Repositories;

// Contract specific helpers based on LINQ.
public class ContractRepository : Repository<Contract>
{
    // Returns every contract signed on or after the given date.
    public IEnumerable<Contract> GetSince(DateTime fromDate) =>
        GetAll().Where(c => c.SignedDate >= fromDate);

    // Returns every contract involving the supplied driver id.
    public IEnumerable<Contract> GetForDriver(Guid driverId) =>
        GetAll().Where(c => c.DriverId == driverId);
}
