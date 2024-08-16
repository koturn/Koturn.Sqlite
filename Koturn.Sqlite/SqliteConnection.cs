#define LEGACY_CSHARP
using System;
using System.Collections.Generic;
using System.Text;


namespace Koturn.Sqlite
{
    /// <summary>
    /// SQLite3 connection.
    /// </summary>
    public class SqliteConnection : IDisposable
    {
        /// <summary>
        /// Cache of Run-Time Library Version string.
        /// </summary>
        /// <remarks>
        /// <see cref="Version"/>.
        /// </remarks>
        private static string _version;
        /// <summary>
        /// Cache of Run-Time Library Source ID.
        /// </summary>
        /// <remarks>
        /// <see cref="SourceId"/>.
        /// </remarks>
        private static string _sourceId;
        /// <summary>
        /// Cache of Run-Time Library Version Numbers represents in <see cref="int"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="VersionNumber"/>.
        /// </remarks>
        private static int _versionNumber = -1;


        /// <summary>
        /// A flag property which indicates this instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Count the number of rows modified.
        /// </summary>
        public int ModifiedRowsCount => SqliteLibrary.Changes(_db);
        /// <summary>
        /// Count the number of rows modified.
        /// </summary>
        public long LongModifiedRowsCount => SqliteLibrary.Changes64(_db);
        /// <summary>
        /// Count the total number of rows modified since the database connection was opened.
        /// </summary>
        public int TotalModifiedRowsCount => SqliteLibrary.TotalChanges(_db);
        /// <summary>
        /// Count the total number of rows modified since the database connection was opened.
        /// </summary>
        public long LongTotalModifiedRowsCount => SqliteLibrary.TotalChanges64(_db);
        /// <summary>
        /// An interrupt is currently in effect for database connection.
        /// </summary>
        /// <returns>True if an interrupt is currently in effect, or false otherwise.</returns>
        public bool IsInterrupted => SqliteLibrary.IsInterrupted(_db);
        /// <summary>
        /// Busy timeout duration in milliseconds.
        /// </summary>
        public int Timeout
        {
            get
            {
                using (var stmt = Prepare("PRAGMA busy_timeout"))
                {
                    stmt.Step();
                    return stmt.GetIntUnchecked(0);
                }
            }
            set => SqliteLibrary.BusyTimeout(_db, value);
        }


        /// <summary>
        /// SQLite3 database handle.
        /// </summary>
        private SqliteHandle _db;


        /// <summary>
        /// Run-Time Library Version string.
        /// </summary>
        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    _version = SqliteLibrary.LibVersion();
                }
                return _version;
            }
        }
        /// <summary>
        /// Run-Time Library Source ID.
        /// </summary>
        public static string SourceId
        {
            get
            {
                if (_sourceId == null)
                {
                    _sourceId = SqliteLibrary.LibSourceId();
                }
                return _sourceId;
            }
        }
        /// <summary>
        /// Run-Time Library Version Numbers represents in <see cref="int"/>.
        /// </summary>
        public static int VersionNumber
        {
            get
            {
                if (_versionNumber == -1)
                {
                    _versionNumber = SqliteLibrary.LibVersionNumber();
                }
                return _versionNumber;
            }
        }
        /// <summary>
        /// The number of bytes of memory currentry outstanding.
        /// </summary>
        public static long MemoryUsed => SqliteLibrary.MemoryUsed();
        /// <summary>
        /// The maximum value of <see cref="MemoryUsed"/>.
        /// </summary>
        public static long MemoryHighWater => SqliteLibrary.MemoryHighWater(false);


        /// <summary>
        /// Create SQLite3 connection and open SQLite3 database file.
        /// </summary>
        public SqliteConnection()
        {
        }

        /// <summary>
        /// Create SQLite3 connection and open SQLite3 database file.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        public SqliteConnection(string filePath)
        {
            Open(filePath);
        }

        /// <summary>
        /// Create SQLite3 connection and open SQLite3 database file.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        public SqliteConnection(string filePath, SqliteOpenFlags flags)
        {
            Open(filePath, flags, null);
        }

        /// <summary>
        /// Create SQLite3 connection and open SQLite3 database file.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        /// <param name="vfs">Name of VFS module to use.</param>
        public SqliteConnection(string filePath, SqliteOpenFlags flags, string vfs)
        {
            Open(filePath, flags, vfs);
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        public void Open(string filePath)
        {
            _db = SqliteLibrary.Open(filePath);
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        public void Open(string filePath, SqliteOpenFlags flags)
        {
            _db = SqliteLibrary.Open(filePath, flags, null);
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        /// <param name="vfs">Name of VFS module to use.</param>
        public void Open(string filePath, SqliteOpenFlags flags, string vfs)
        {
            _db = SqliteLibrary.Open(filePath, flags, vfs);
        }

        /// <summary>
        /// Close database.
        /// </summary>
        public void Close()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        [Obsolete("This method uses legacy API, sqlite3_exec()")]
        public void ExecuteLegacy(string sql)
        {
            SqliteLibrary.Execute(_db, sql);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        public void Execute(string sql)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            var byteCount = sqlUtf8Bytes.Length;
            unsafe
            {
                fixed (byte *pbSqlBase = &sqlUtf8Bytes[0])
                {
                    var pbSql = pbSqlBase;
                    while (*pbSql != 0)
                    {
                        using (var stmt = SqliteLibrary.Prepare(_db, ref pbSql, ref byteCount))
                        {
                            while (SqliteLibrary.Step(stmt, _db))
                            {

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        public void Execute(string sql, Func<string[], string[], bool> callback)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            var byteCount = sqlUtf8Bytes.Length;
            unsafe
            {
                fixed (byte *pbSqlBase = &sqlUtf8Bytes[0])
                {
                    var pbSql = pbSqlBase;
                    string[] columnNames = null;
                    string[] columnTexts = null;
                    while (*pbSql != 0)
                    {
                        using (var stmt = SqliteLibrary.Prepare(_db, ref pbSql, ref byteCount))
                        {
                            var columnCount = SqliteLibrary.ColumnCount(stmt);
                            if (columnNames == null || columnNames.Length != columnCount)
                            {
                                columnNames = new string[columnCount];
                                columnTexts = new string[columnCount];
                            }

                            for (int i = 0; i < columnNames.Length; i++)
                            {
                                columnNames[i] = SqliteLibrary.ColumnName(stmt, i);
                            }
                            while (SqliteLibrary.Step(stmt, _db))
                            {
                                for (int i = 0; i < columnTexts.Length; i++)
                                {
                                    columnTexts[i] = SqliteLibrary.ColumnText(stmt, i);
                                }
                                if (!callback(columnTexts, columnNames))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute first query of specified SQL string.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        public void ExecuteSingle(string sql)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            unsafe
            {
                fixed (byte *pbSqlBase = sqlUtf8Bytes)
                {
                    byte *_;
                    using (var stmt = SqliteLibrary.Prepare(_db, pbSqlBase, sqlUtf8Bytes.Length, out _))
                    {
                        SqliteLibrary.Step(stmt, _db);
                    }
                }
            }
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        public void ExecuteSingle(string sql, Func<ISqliteColumnAccessable, bool> callback)
        {
            using (var stmt = Prepare(sql))
            {
                var accessor = (ISqliteColumnAccessable)stmt;
                while (stmt.Step() && callback(accessor))
                {
                }
            }
        }

        /// <summary>
        /// Compile SQL statement and construct prepared statement object.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <returns>Statement handle.</returns>
        public SqliteStatement Prepare(string sql)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            unsafe
            {
                fixed (byte *pbSqlBase = sqlUtf8Bytes)
                {
                    byte *_;
                    return new SqliteStatement(SqliteLibrary.Prepare(_db, pbSqlBase, sqlUtf8Bytes.Length, out _));
                }
            }
        }

        /// <summary>
        /// Open BLOB stream.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        /// <returns>Opened <see cref="SqliteBlobStream"/>.</returns>
        public SqliteBlobStream OpenBlob(string tableName, string columnName, long rowId, SqliteOpenFlags flags)
        {
            return new SqliteBlobStream(_db, tableName, columnName, rowId, flags);
        }

        /// <summary>
        /// Open BLOB stream.
        /// </summary>
        /// <param name="dbName">Symbolic name of the database.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        /// <returns>Opened <see cref="SqliteBlobStream"/>.</returns>
        public SqliteBlobStream OpenBlob(string dbName, string tableName, string columnName, long rowId, SqliteOpenFlags flags)
        {
            return new SqliteBlobStream(_db, "main", tableName, columnName, rowId, flags);
        }

        /// <summary>
        /// Begin transaction.
        /// </summary>
        public SqliteTransaction BeginTransaction()
        {
            ExecuteSingle("BEGIN");
            return new SqliteTransaction(this);
        }

        /// <summary>
        /// Interrupt a long-running query.
        /// </summary>
        public void Interrupt()
        {
            SqliteLibrary.Interrupt(_db);
        }

        /// <summary>
        /// Execute "EXPLAIN".
        /// </summary>
        /// <param name="sql">Target query.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteExplainRow"/>.</returns>
        public IEnumerable<SqliteExplainRow> Explain(string sql)
        {
            using (var stmt = Prepare("EXPLAIN " + sql))
            {
                while (stmt.Step())
                {
                    yield return new SqliteExplainRow(
                        stmt.GetIntUnchecked(0),  // addr
                        stmt.GetTextUnchecked(1),  // opcode
                        stmt.GetIntUnchecked(2),  // p1
                        stmt.GetIntUnchecked(3),  // p2
                        stmt.GetIntUnchecked(4),  // p3
                        stmt.GetTextUnchecked(5),  // p4
                        (ushort)stmt.GetIntUnchecked(6),  // p5
                        stmt.GetTextUnchecked(7));  // comment
                }
            }
        }

        /// <summary>
        /// Execute "EXPLAIN QUERY PLAN".
        /// </summary>
        /// <param name="sql">Target query.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteQueryPlan"/>.</returns>
        public IEnumerable<SqliteQueryPlan> ExplainQueryPlan(string sql)
        {
            using (var stmt = Prepare("EXPLAIN QUERY PLAN " + sql))
            {
                while (stmt.Step())
                {
                    yield return new SqliteQueryPlan(
                        stmt.GetIntUnchecked(0),  // id
                        stmt.GetIntUnchecked(1),  // parent
                        stmt.GetIntUnchecked(2),  // notused
                        stmt.GetTextUnchecked(3));  // detail
                }
            }
        }

        /// <summary>
        /// Execute VACUUM.
        /// </summary>
        public void Vacuum()
        {
            ExecuteSingle("VACUUM");
        }

        /// <summary>
        /// Execute ANALYZE.
        /// </summary>
        public void Analyze()
        {
            ExecuteSingle("ANALYZE");
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Release all resources used by the <see cref="SqliteConnection"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Get the maximum value of <see cref="MemoryUsed"/> and reset it to the current value.
        /// </summary>
        /// <returns>The maximum value of <see cref="MemoryUsed"/> since the high-water mark was last reset.</returns>
        public static long ResetMemoryHighWater()
        {
            return SqliteLibrary.MemoryHighWater(true);
        }


        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="p">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static string PtrToStringUtf8(IntPtr p)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8(p);
#else
            unsafe
            {
                return PtrToStringUtf8((sbyte *)p);
            }
#endif
        }

        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="psb">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static unsafe string PtrToStringUtf8(sbyte *psb)
        {
            return new string(psb, 0, ByteLengthOf(psb), Encoding.UTF8);
        }

        /// <summary>
        /// Get byte length of null-terminated string.
        /// </summary>
        /// <param name="psb">Pointer to null-terminated string.</param>
        /// <returns>Byte length of null-terminated string.</returns>
        private static unsafe int ByteLengthOf(sbyte *psb)
        {
            var psbEnd = psb;
            for (; *psbEnd != 0; psbEnd++)
            {
            }
            return (int)(psbEnd - psb);
        }
    }
}
