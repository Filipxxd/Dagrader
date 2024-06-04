using Domain.Tables;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
	public string ConnectionString { get; set; } = string.Empty;

	public IDbConnection Create()
	{
		var sqlConnection = new SqlConnection(ConnectionString);
		sqlConnection.Open();
		return sqlConnection;
	}
}