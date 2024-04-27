using Koturn.Sqlite;
using NuGet.Frameworks;
using Xunit.Abstractions;

namespace Koturn.Sqlite.Tests
{
    public class UnitTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetLibraryInfo()
        {
            _testOutputHelper.WriteLine($"Version: {SqliteConnection.Version}");
            _testOutputHelper.WriteLine($"VersionNumber: {SqliteConnection.VersionNumber}");
            _testOutputHelper.WriteLine($"SourceId: {SqliteConnection.SourceId}");
        }

        [Fact]
        public void Select()
        {
            const string tableName = "t_select_test";
            File.Delete("test.db");
            using (var conn = new SqliteConnection(""))
            {
                conn.ExecuteSingle($"CREATE TABLE {tableName} (pk_value INTEGER NOT NULL, val INTEGER, PRIMARY KEY (pk_value))");
                using (var transaction = conn.BeginTransaction())
                using (var stmt = conn.Prepare($"INSERT INTO {tableName} (pk_value, val) VALUES (:pk_value, :val)"))
                {
                    stmt.Bind(":pk_value", 1);
                    stmt.Bind(":val", 1);
                    stmt.Step();
                    Assert.Equal(1, conn.ModifiedRowsCount);
                    stmt.Reset();

                    stmt.Bind(":pk_value", 2);
                    stmt.Bind(":val", 2);
                    stmt.Step();
                    Assert.Equal(1, conn.ModifiedRowsCount);
                    stmt.Reset();

                    stmt.Bind(":pk_value", 3);
                    stmt.BindNull(":val");
                    stmt.Step();
                    Assert.Equal(1, conn.ModifiedRowsCount);
                    stmt.Reset();

                    transaction.Commit();
                }
                Assert.Equal(3, conn.TotalModifiedRowsCount);

                using (var stmt = conn.Prepare($"SELECT COUNT(*) AS cnt FROM {tableName}"))
                {
                    while (stmt.Step())
                    {
                        Assert.Equal(3, stmt.GetInt("cnt"));
                    }
                }

                using (var stmt = conn.Prepare($"SELECT * FROM {tableName}"))
                {
                    stmt.Step();
                    Assert.Equal(1, stmt.GetInt("pk_value"));
                    Assert.Equal(1, stmt.GetInt("val"));

                    stmt.Step();
                    Assert.Equal(2, stmt.GetInt("pk_value"));
                    Assert.Equal(2, stmt.GetInt("val"));

                    stmt.Step();
                    Assert.Equal(3, stmt.GetInt("pk_value"));
                    Assert.Null(stmt.GetNullableInt("val"));
                }

                _testOutputHelper.WriteLine($"MemoryUsed / MemoryHighWater = {SqliteConnection.MemoryUsed} / {SqliteConnection.MemoryHighWater}");
            }

            _testOutputHelper.WriteLine($"MemoryUsed / MemoryHighWater = {SqliteConnection.MemoryUsed} / {SqliteConnection.MemoryHighWater}");
        }
    }
}
