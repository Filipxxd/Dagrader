using Dapper;
using Domain.Models;
using Domain.Tables;

namespace Data.Tables;

public class ActivityUnitTable : DbConnectionTable, IActivityUnitTable
{
	public ActivityUnitTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
	{
	}

	public async Task<List<ActivityUnit>> GetAllAsync()
	{
		using (var connection = DbConnection)
		{
			var sql = "SELECT * FROM ActivityUnits";

			return (await connection.QueryAsync<ActivityUnit>(sql)).ToList();
		};
	}

	public async Task<ActivityUnit?> GetByIdAsync(int id)
	{
		using var connection = DbConnection;
		var sql = "SELECT * FROM ActivityUnits WHERE ActivityUnitId = @Id";

		return await connection.QuerySingleOrDefaultAsync<ActivityUnit>(sql, new { Id = id });
	}

	public async Task AddAsync(ActivityUnit entity)
	{
		var sql = "INSERT INTO ActivityUnits (DisplayName,DisplayNameShort,IsDeletable,DateCreated, DateUpdated) " +
					"VALUES (@DisplayName,@DisplayNameShort,@IsDeletable,@DateCreated, @DateUpdated)";

		await DbConnection.ExecuteAsync(sql, entity);
	}

	public async Task UpdateAsync(ActivityUnit entity)
	{
		var sql = "UPDATE ActivityUnits " +
				  "SET DisplayName = @DisplayName, DisplayNameShort = @DisplayNameShort, DateUpdated = @Rate" +
				  "WHERE ActivityUnitId = @Id";

		await DbConnection.ExecuteAsync(sql, entity);
	}

	public async Task DeleteAsync(int id)
	{
		var sql = "DELETE FROM ActivityUnits WHERE IsDeletable = 1 AND ActivityUnitId = @Id";

		await DbConnection.ExecuteAsync(sql, new { Id = id });
	}
}
