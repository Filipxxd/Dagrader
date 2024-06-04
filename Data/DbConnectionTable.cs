using Domain.Tables;
using System.Data;

namespace Data;

public abstract class DbConnectionTable(IDbConnectionFactory dbConnectionFactory) : IDisposable
{
	private bool _disposed = false;
	protected IDbConnection DbConnection { get; private set; } = dbConnectionFactory.Create();

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		DbConnection.Close();
		_disposed = true;
		GC.SuppressFinalize(this);
	}
}