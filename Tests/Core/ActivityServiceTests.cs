using Core.Services;
using Core.Validators;
using Domain.Enums;
using Domain.Models;
using Domain.Tables;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Core;

public class ActivityServiceTests
{
	[Fact]
	public async Task ActivityUnit_Create_ReturnsSuccess()
	{
		// Arrange
		var activityUnit = new ActivityUnit()
		{
			ActivityUnitId = 1,
			DisplayName = "Second",
			DisplayNameShort = "s"
		};

		var table = Substitute.For<IActivityUnitTable>();
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.Create(activityUnit);

		// Assert
		await table.Received(1).AddAsync(activityUnit);
		Assert.Equal(EntityOperationResult.Success, result);
	}

	[Fact]
	public async Task ActivityUnit_Create_ReturnsEntityNotValid_WhenDisplayNameIsTooLong()
	{
		// Arrange
		var activityUnit = new ActivityUnit()
		{
			ActivityUnitId = 1,
			DisplayName = "This is a very long display name that exceeds the maximum allowed length.",
			DisplayNameShort = "s"
		};

		var table = Substitute.For<IActivityUnitTable>();
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.Create(activityUnit);

		// Assert
		await table.DidNotReceive().AddAsync(activityUnit);
		Assert.Equal(EntityOperationResult.EntityNotValid, result);
	}

	[Fact]
	public async Task ActivityUnit_Create_ReturnsEntityNotValid_WhenDisplayNameIsEmpty()
	{
		// Arrange
		var activityUnit = new ActivityUnit()
		{
			ActivityUnitId = 1,
			DisplayName = "",
			DisplayNameShort = "s"
		};

		var table = Substitute.For<IActivityUnitTable>();
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.Create(activityUnit);

		// Assert
		await table.DidNotReceive().AddAsync(activityUnit);
		Assert.Equal(EntityOperationResult.EntityNotValid, result);
	}

	[Fact]
	public async Task ActivityUnit_GetAll_ReturnsAllUnits()
	{
		// Arrange
		var units = new List<ActivityUnit>
	{
		new() { ActivityUnitId = 1, DisplayName = "Unit 1", DisplayNameShort = "U1" },
		new() { ActivityUnitId = 2, DisplayName = "Unit 2", DisplayNameShort = "U2" }
	};

		var table = Substitute.For<IActivityUnitTable>();
		table.GetAllAsync().Returns(units);
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.GetAll();

		// Assert
		Assert.Equal(units, result);
	}


	[Fact]
	public async Task ActivityUnit_GetById_ReturnsUnitById()
	{
		// Arrange
		var unit = new ActivityUnit { ActivityUnitId = 1, DisplayName = "Unit 1", DisplayNameShort = "U1" };

		var table = Substitute.For<IActivityUnitTable>();
		table.GetByIdAsync(1).Returns(unit);
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.GetById(1);

		// Assert
		Assert.Equal(unit, result);
	}

	[Fact]
	public async Task ActivityUnit_Update_ReturnsSuccess()
	{
		// Arrange
		var updatedUnit = new ActivityUnit { ActivityUnitId = 1, DisplayName = "Updated Unit", DisplayNameShort = "UU" };

		var table = Substitute.For<IActivityUnitTable>();
		var originalUnit = new ActivityUnit { ActivityUnitId = 1, DisplayName = "Original Unit", DisplayNameShort = "OU" };
		table.GetByIdAsync(1).Returns(originalUnit);

		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.Update(updatedUnit);

		// Assert
		await table.Received(1).UpdateAsync(originalUnit);
		Assert.Equal(EntityOperationResult.Success, result);
	}

	[Fact]
	public async Task ActivityUnit_Delete_ReturnsSuccess()
	{
		// Arrange
		var deletedUnitId = 1;

		var table = Substitute.For<IActivityUnitTable>();
		var unitToDelete = new ActivityUnit { ActivityUnitId = 1, DisplayName = "Unit to delete", DisplayNameShort = "UD" };
		table.GetByIdAsync(deletedUnitId).Returns(unitToDelete);

		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.Delete(deletedUnitId);

		// Assert
		await table.Received(1).DeleteAsync(deletedUnitId);
		Assert.Equal(EntityOperationResult.Success, result);
	}

	[Fact]
	public async Task ActivityUnit_GetAll_ReturnsEmptyList_WhenNoUnitsExist()
	{
		// Arrange
		var units = new List<ActivityUnit>();

		var table = Substitute.For<IActivityUnitTable>();
		table.GetAllAsync().Returns(units);
		var logger = Substitute.For<ILogger<ActivityService>>();
		var activityService = new ActivityService(table, new ActivityUnitValidator(), logger);

		// Act
		var result = await activityService.GetAll();

		// Assert
		await table.Received(1).GetAllAsync();
		Assert.Empty(result);
	}


}
