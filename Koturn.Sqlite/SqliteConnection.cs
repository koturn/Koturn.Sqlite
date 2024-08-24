#define LEGACY_CSHARP
using System;
using System.Collections.Generic;
using System.Text;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
System.Diagnostics.CodeAnalysis
#endif


namespace Koturn.Sqlite
{
    /// <summary>
    /// SQLite3 connection.
    /// </summary>
    public class SqliteConnection : IDisposable
    {
        /// <summary>
        /// UTF-8 byte sequence of "ANALYZE".
        /// </summary>
        private static readonly byte[] AnalyzeUtf8Bytes;
        /// <summary>
        /// UTF-8 byte sequence of "VACUUM".
        /// </summary>
        private static readonly byte[] VacuumUtf8Bytes;
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
        private static int _versionNumber;

        /// <summary>
        /// Initialize static members.
        /// </summary>
        static SqliteConnection()
        {
            AnalyzeUtf8Bytes = new [] { (byte)'A', (byte)'N', (byte)'A', (byte)'L', (byte)'Y', (byte)'Z', (byte)'E' };
            VacuumUtf8Bytes = new [] { (byte)'V', (byte)'A', (byte)'C', (byte)'U', (byte)'U', (byte)'M' };
            _versionNumber = -1;
        }

        /// <summary>
        /// A flag property which indicates this instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Last insert ROWID.
        /// </summary>
        public long LastInsertRowId
        {
            get => SqliteLibrary.LastInsertRowId(_db);
            set => SqliteLibrary.SetLastInsertRowId(_db, value);
        }
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
        /// Create SQLite3 connection and open SQLite3 database on memory.
        /// </summary>
        public SqliteConnection()
        {
            Open(null);
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
            ExecuteSingle(Encoding.UTF8.GetBytes(sql));
        }

        /// <summary>
        /// Execute first query of specified SQL string.
        /// </summary>
        /// <param name="sqlUtf8Bytes">UTF-8 byte sequence of SQL to be evaluated.</param>
        public void ExecuteSingle(byte[] sqlUtf8Bytes)
        {
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
            ExecuteSingle(Encoding.UTF8.GetBytes(sql), callback);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sqlUtf8Bytes">UTF-8 byte sequence of SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        public void ExecuteSingle(byte[] sqlUtf8Bytes, Func<ISqliteColumnAccessable, bool> callback)
        {
            using (var stmt = Prepare(sqlUtf8Bytes))
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
            return Prepare(Encoding.UTF8.GetBytes(sql));
        }

        /// <summary>
        /// Compile SQL statement and construct prepared statement object.
        /// </summary>
        /// <param name="sqlUtf8Bytes">UTF-8 byte sequence of SQL to be evaluated.</param>
        /// <returns>Statement handle.</returns>
        public SqliteStatement Prepare(byte[] sqlUtf8Bytes)
        {
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
        /// Check if specified schema exists or not.
        /// </summary>
        /// <param name="type">Schema type.</param>
        /// <param name="name">Object name.</param>
        /// <returns>True if specified object exists, otherwise false.</returns>
        public bool IsExists(SqliteSchemaType type, string name)
        {
            return IsExists(GetSchemaTypeName(type), name);
        }

        /// <summary>
        /// Check if specified index exists or not.
        /// </summary>
        /// <param name="typeName">Schema type.</param>
        /// <param name="name">Object name.</param>
        /// <returns>True if specified object exists, otherwise false.</returns>
        public bool IsExists(string typeName, string name)
        {
            // sqlite_master is legacy table, sqlite_schema is preferred.
            using (var stmt = Prepare("SELECT EXISTS(SELECT 1 FROM sqlite_schema WHERE type = :type AND name LIKE :name) AS is_exists"))
            {
                stmt.Bind(1, typeName);
                stmt.Bind(2, name);
                if (!stmt.Step())
                {
                    return false;
                }
                return stmt.GetIntUnchecked(0) != 0;
            }
        }

        /// <summary>
        /// Check if specified table exists or not.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns>True if specified table exists, otherwise false.</returns>
        public bool IsTableExists(string tableName)
        {
            return IsExists("table", tableName);
        }

        /// <summary>
        /// Check if specified index exists or not.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns>True if specified index exists, otherwise false.</returns>
        public bool IsIndexExists(string indexName)
        {
            return IsExists("index", indexName);
        }

        /// <summary>
        /// Check if specified view exists or not.
        /// </summary>
        /// <param name="viewName">view name.</param>
        /// <returns>True if specified view exists, otherwise false.</returns>
        public bool IsViewExists(string viewName)
        {
            return IsExists("view", viewName);
        }

        /// <summary>
        /// Check if specified trigger exists or not.
        /// </summary>
        /// <param name="triggerName">trigger name.</param>
        /// <returns>True if specified view exists, otherwise false.</returns>
        public bool IsTriggerExists(string triggerName)
        {
            return IsExists("trigger", triggerName);
        }

        /// <summary>
        /// Get definition SQL of specified object.
        /// </summary>
        /// <param name="name">Object name.</param>
        /// <returns>Definition SQL.</returns>
        public string GetDefinition(string name)
        {
            using (var stmt = Prepare("SELECT sql FROM sqlite_schema WHERE name LIKE :name"))
            {
                stmt.Bind(1, name);
                if (!stmt.Step())
                {
                    return null;
                }
                return stmt.GetTextUnchecked(0);
            }
        }

        /// <summary>
        /// Get table info with pragma_table_info('xxx'), which is same as PRAGMA table_info('xxx').
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteTableInfoRow"/>.</returns>
        public IEnumerable<SqliteTableInfoRow> GetTableInfo(string tableName)
        {
            using (var stmt = Prepare("SELECT cid, name, type, \"notnull\", dflt_value, pk FROM pragma_table_info(:table_name)"))
            {
                stmt.Bind(1, tableName);
                while (stmt.Step())
                {
                    yield return new SqliteTableInfoRow(
                        stmt.GetIntUnchecked(0),  // cid
                        stmt.GetTextUnchecked(1),  // name
                        stmt.GetTextUnchecked(2),  // type
                        stmt.GetIntUnchecked(3) == 1,  // notnull
                        stmt.GetTextUnchecked(4),  // dflt_value
                        stmt.GetIntUnchecked(5) == 1);  // pk
                }
            }
        }

        /// <summary>
        /// Get table info with pragma_table_xinfo('xxx'), which is same as PRAGMA table_xinfo('xxx').
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteTableXInfoRow"/>.</returns>
        public IEnumerable<SqliteTableXInfoRow> GetTableXInfo(string tableName)
        {
            using (var stmt = Prepare("SELECT cid, name, type, \"notnull\", dflt_value, pk, hidden FROM pragma_table_xinfo(:table_name)"))
            {
                stmt.Bind(1, tableName);
                while (stmt.Step())
                {
                    yield return new SqliteTableXInfoRow(
                        stmt.GetIntUnchecked(0),  // cid
                        stmt.GetTextUnchecked(1),  // name
                        stmt.GetTextUnchecked(2),  // type
                        stmt.GetIntUnchecked(3) == 1,  // notnull
                        stmt.GetTextUnchecked(4),  // dflt_value
                        stmt.GetIntUnchecked(5) == 1,  // pk
                        stmt.GetIntUnchecked(6));  // hidden
                }
            }
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
        /// Get table info with "pragma_index_info('xxx'), which is same as PRAGMA index_info('xxx')."
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteIndexInfoRow"/>.</returns>
        public IEnumerable<SqliteIndexInfoRow> GetIndexInfo(string indexName)
        {
            using (var stmt = Prepare("SELECT seqno, cid, name FROM pragma_index_info(:index_name)"))
            {
                stmt.Bind(1, indexName);
                while (stmt.Step())
                {
                    yield return new SqliteIndexInfoRow(
                        stmt.GetIntUnchecked(0),  // seqno
                        stmt.GetIntUnchecked(1),  // cid
                        stmt.GetTextUnchecked(2));  // name
                }
            }
        }

        /// <summary>
        /// Get table info with "pragma_index_xinfo('xxx'), which is same as PRAGMA index_info('xxx')."
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="SqliteIndexXInfoRow"/>.</returns>
        public IEnumerable<SqliteIndexXInfoRow> GetIndexXInfo(string indexName)
        {
            using (var stmt = Prepare("SELECT seqno, cid, name, desc, coll, key FROM pragma_index_xinfo(:index_name)"))
            {
                stmt.Bind(1, indexName);
                while (stmt.Step())
                {
                    yield return new SqliteIndexXInfoRow(
                        stmt.GetIntUnchecked(0),  // seqno
                        stmt.GetIntUnchecked(1),  // cid
                        stmt.GetTextUnchecked(2),  // name
                        stmt.GetIntUnchecked(3),  // desc
                        stmt.GetTextUnchecked(4),  // coll
                        stmt.GetIntUnchecked(5));  // key
                }
            }
        }

        /// <summary>
        /// Execute VACUUM.
        /// </summary>
        public void Vacuum()
        {
            ExecuteSingle(VacuumUtf8Bytes);
        }

        /// <summary>
        /// Execute ANALYZE.
        /// </summary>
        public void Analyze()
        {
            ExecuteSingle(AnalyzeUtf8Bytes);
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

        /// <summary>
        /// Get name of schema kind.
        /// </summary>
        /// <param name="type">Schema type value.</param>
        /// <returns>Name of schema kind.</returns>
        private static string GetSchemaTypeName(SqliteSchemaType type)
        {
            switch (type)
            {
                case SqliteSchemaType.Table:
                    return "table";
                case SqliteSchemaType.Index:
                    return "index";
                case SqliteSchemaType.View:
                    return "view";
                case SqliteSchemaType.Trigger:
                    return "trigger";
                default:
                    ThrowArgumentOutOfRangeException("type", type, string.Format("{0} is not value of SqliteSchemaType", (int)type));
                    return null;
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">The value of the argument that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <exception cref="ArgumentOutOfRangeException">Always throws.</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        [DoesNotReturn]
#endif
        private static void ThrowArgumentOutOfRangeException<T>(string paramName, T actualValue, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }
    }
}
