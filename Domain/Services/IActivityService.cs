using Domain.Enums;
using Domain.Models;

namespace Domain.Services;

public interface IActivityService
{
    public Task<List<ActivityUnit>> GetAll();

    public Task<ActivityUnit?> GetById(int unitId);

    public Task<EntityOperationResult> Create(ActivityUnit newActivityUnit);

    public Task<EntityOperationResult> Update(ActivityUnit updatedActivityUnit);

    public Task<EntityOperationResult> Delete(int deletedActivityUnitId);
}
