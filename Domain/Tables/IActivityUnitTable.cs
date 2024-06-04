using Domain.Models;

namespace Domain.Tables;

public interface IActivityUnitTable
{

    public Task<List<ActivityUnit>> GetAllAsync();

    public Task<ActivityUnit?> GetByIdAsync(int id);

    public Task AddAsync(ActivityUnit entity);

    public Task UpdateAsync(ActivityUnit entity);

    public Task DeleteAsync(int id);
}
