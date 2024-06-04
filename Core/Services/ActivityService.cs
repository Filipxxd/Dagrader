using Core.Validators;
using Domain.Enums;
using Domain.Models;
using Domain.Services;
using Domain.Tables;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public sealed class ActivityService(IActivityUnitTable activityUnitRepository, ActivityUnitValidator validator, ILogger<ActivityService> logger) : IActivityService
{
	private readonly IActivityUnitTable _activityUnitRepository = activityUnitRepository;
	private readonly ActivityUnitValidator _validator = validator;
	private readonly ILogger<ActivityService> _logger = logger;

	public async Task<List<ActivityUnit>> GetAll()
	{
		try
		{
			return await _activityUnitRepository.GetAllAsync();
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, ex?.ToString());
			return new();
		}
	}

	public async Task<ActivityUnit?> GetById(int unitId)
	{
		try
		{
			return await _activityUnitRepository.GetByIdAsync(unitId);
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, ex?.ToString());
			return null;
		}
	}

	public async Task<EntityOperationResult> Create(ActivityUnit newActivityUnit)
	{
		if (_validator.Validate(newActivityUnit).IsValid is false) return EntityOperationResult.EntityNotValid;

		newActivityUnit.IsDeletable = true;
		newActivityUnit.CreatedDate = DateTime.Now;
		newActivityUnit.UpdatedDate = DateTime.Now;

		try
		{
			await _activityUnitRepository.AddAsync(newActivityUnit);

			return EntityOperationResult.Success;
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, ex?.ToString());
			return EntityOperationResult.InternalServerError;
		}
	}

	public async Task<EntityOperationResult> Update(ActivityUnit updatedActivityUnit)
	{
		if (_validator.Validate(updatedActivityUnit).IsValid is false) return EntityOperationResult.EntityNotValid;

		try
		{
			var original = await _activityUnitRepository.GetByIdAsync(updatedActivityUnit.ActivityUnitId);

			if (original is null) return EntityOperationResult.NotFound;

			original.DisplayName = updatedActivityUnit.DisplayName;
			original.DisplayNameShort = updatedActivityUnit.DisplayNameShort;
			original.UpdatedDate = DateTime.Now;

			await _activityUnitRepository.UpdateAsync(original);

			return EntityOperationResult.Success;
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, ex?.ToString());
			return EntityOperationResult.InternalServerError;
		}

	}

	public async Task<EntityOperationResult> Delete(int deletedActivityUnitId)
	{
		if (deletedActivityUnitId is 0) return EntityOperationResult.NoActionTaken;

		var original = await _activityUnitRepository.GetByIdAsync(deletedActivityUnitId);

		if (original is null) return EntityOperationResult.NotFound;

		if (original.IsDeletable is false) return EntityOperationResult.NotAllowed;

		try
		{
			await _activityUnitRepository.DeleteAsync(deletedActivityUnitId);

			return EntityOperationResult.Success;
		}
		catch (Exception ex)
		{
			_logger.Log(LogLevel.Error, ex?.ToString());
			//TODO: await _logger.Log($"Database failure. Service: '{nameof(ActivityService)}' Action: '{nameof(Delete)}' Error: '{exception.Message}'");
			return EntityOperationResult.InternalServerError;
		}
	}
}
