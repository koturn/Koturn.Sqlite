Koturn.Sqlite
=============

Very simple [SQLite3](https://www.sqlite.org/index.html "SQLite Home Page") P/Invoke library in .NET Standard 2.0.

## Sample code

```cs
using System;
using Koturn.Sqlite;


namespace Sample
{
    /// <summary>
    /// Entry point class
    /// </summary>
    private static class Program
    {
        /// <summary>
        /// An entry point of this program.
        /// </summary>
        private static void Main()
        {
            Console.WriteLine("Version: " + SqliteConnection.Version);
            Console.WriteLine("SourceId: " + SqliteConnection.SourceId);
            Console.WriteLine("VersionNumber: " + SqliteConnection.VersionNumber);

            // Open sample.db for Read/Write
            using (var conn = new SqliteConnection("sample.db"))
            {
                conn.ExecuteSingle("CREATE TABLE IF NOT EXISTS t_sample (pkey INTEGER NOT NULL PRIMARY KEY, val INTEGER NOT NULL, msg TEXT)");
                conn.ExecuteSingle("DELETE FROM t_sample");
                Console.WriteLine("DELETE " + conn.ModifiedRowsCount);
                using (var transaction = conn.BeginTransaction())
                {
                    using (var stmt = conn.Prepare("INSERT INTO t_sample (pkey, val, msg) VALUES (:pkey, :val, :msg)"))
                    {
                        stmt.Bind(":pkey", 1);
                        stmt.Bind(":val", 111);
                        stmt.Bind(":msg", "first");
                        stmt.Step();  // Execute INSERT
                        Console.WriteLine("INSERT " + conn.ModifiedRowsCount);
                        stmt.Reset();

                        stmt.Bind(":pkey", 2);
                        stmt.Bind(":val", 222);
                        stmt.Bind(":msg", "second");
                        stmt.Step();  // Execute INSERT
                        Console.WriteLine("INSERT " + conn.ModifiedRowsCount);
                        stmt.Reset();

                        stmt.Bind(":pkey", 5);
                        stmt.Bind(":val", 333);
                        stmt.BindNull(":msg");
                        stmt.Step();  // Execute INSERT
                        Console.WriteLine("INSERT " + conn.ModifiedRowsCount);
                        stmt.Reset();

                        stmt.Bind(":pkey", 6);
                        stmt.Bind(":val", 444);
                        stmt.Bind(":msg", null);
                        stmt.Step();  // Execute INSERT
                        Console.WriteLine("INSERT " + conn.ModifiedRowsCount);
                    }
                    Console.WriteLine("Total modified " + conn.TotalModifiedRowsCount);

                    transaction.Commit();
                }

                // Raw version of fetching SELECT results.
                using (var stmt = conn.Prepare("SELECT * FROM t_sample"))
                {
                    while (stmt.Step())  // Fetch one row
                    {
                        Console.WriteLine(
                            "(pkey, val, msg) = ({0}, {1}, {2})",
                            stmt.GetInt("pkey"),
                            stmt.GetInt("val"),
                            stmt.GetText("msg"));
                    }
                }

                Console.WriteLine("Memory used: {0} / {1}", SqliteConnection.MemoryUsed, SqliteConnection.MemoryHighWater);
            }

            // Open sample.db for Read only.
            using (var conn = new SqliteConnection("sample.db", SqliteOpenFlags.ReadOnly))
            {
                // Convenient version of fetching SELECT results.
                conn.ExecuteSingle("SELECT pkey, val, COALESCE(msg, 'NULL') AS msg FROM t_sample", accessor =>
                {
                    Console.WriteLine(
                        "(pkey, val, msg) = ({0}, {1}, {2})",
                        accessor.GetInt("pkey"),
                        accessor.GetInt("val"),
                        accessor.GetText("msg"));
                    return true;
                });

                Console.WriteLine("Memory used: {0} / {1}", SqliteConnection.MemoryUsed, SqliteConnection.MemoryHighWater);
            }
        }
    }
}
```

## Supported API

- `sqlite3_libversion`
- `sqlite3_sourceid`
- `sqlite3_libversion_number`
- `sqlite3_open16`
- `sqlite3_open_v2`
- `sqlite3_close`
- `sqlite3_exec`
- `sqlite3_free`
- `sqlite3_prepare`
- `sqlite3_sql`
- `sqlite3_expanded_sql`
- `sqlite3_normalized_sql`
- `sqlite3_step`
- `sqlite3_finalize`
- `sqlite3_bind_parameter_count`
- `sqlite3_bind_parameter_name`
- `sqlite3_bind_parameter_index`
- `sqlite3_bind_int`
- `sqlite3_bind_int64`
- `sqlite3_bind_double`
- `sqlite3_bind_text16`
- `sqlite3_bind_blob`
- `sqlite3_bind_blob64`
- `sqlite3_bind_zeroblob`
- `sqlite3_bind_zeroblob64`
- `sqlite3_bind_null`
- `sqlite3_reset`
- `sqlite3_column_count`
- `sqlite3_column_name16`
- `sqlite3_column_value`
- `sqlite3_column_int`
- `sqlite3_column_int64`
- `sqlite3_column_double`
- `sqlite3_column_text16`
- `sqlite3_column_blob`
- `sqlite3_column_bytes`
- `sqlite3_column_bytes16`
- `sqlite3_value_type`
- `sqlite3_value_int`
- `sqlite3_value_int64`
- `sqlite3_value_double`
- `sqlite3_value_text16`
- `sqlite3_value_blob`
- `sqlite3_value_bytes`
- `sqlite3_value_bytes16`
- `sqlite3_changes`
- `sqlite3_changes64`
- `sqlite3_total_changes`
- `sqlite3_total_changes64`
- `sqlite3_blob_open`
- `sqlite3_blob_reopen`
- `sqlite3_blob_close`
- `sqlite3_blob_bytes`
- `sqlite3_blob_read`
- `sqlite3_blob_write`
- `sqlite3_interrupt`
- `sqlite3_is_interrupted`
- `sqlite3_busy_timeout`
- `sqlite3_busy_handler`
- `sqlite3_errmsg16`
- `sqlite3_errstr`
- `sqlite3_memory_used`
- `sqlite3_memory_highwater`

## LICENSE

This software is released under the Public Domain, see [LICENSE](LICENSE "LICENSE").
