using System;
using System.Runtime.InteropServices;
using System.Text;
using Koturn.Sqlite.Enums;
using Koturn.Sqlite.Exceptions;
using Koturn.Sqlite.Handles;


namespace Koturn.Sqlite
{
    /// <summary>
    /// Entry point class
    /// </summary>
    public static class SqliteLibrary
    {
        /// <summary>
        /// Callback delegate of third argument of <see cref="Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.</param>
        /// <param name="colCount">Number of columns.</param>
        /// <param name="pColumns">Pointer to null-terminated string array of column text.</param>
        /// <param name="pColumnNames">Pointer to null-terminated string array of column name.</param>
        /// <returns>0 to continue, otherwise to abotd.</returns>
        public delegate int ExecCallbackFunc(IntPtr arg, int colCount, IntPtr pColumns, IntPtr pColumnNames);
        /// <summary>
        /// Callback delegate of second argument of <see cref="BusyHandler"/>.
        /// </summary>
        /// <param name="arg">Third argument of <see cref="BusyHandler"/>.</param>
        /// <param name="invokeCount">The number of times that the busy handler has been invoked previously for the same locking event.</param>
        public delegate int BusyCallbackFunc(IntPtr arg, int invokeCount);

        /// <summary>
        /// Delegate for <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult OpenFunc(string filePath, out SqliteHandle db);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.OpenV2"/> or <see cref="NativeMethods.OpenV2W"/>.
        /// </summary>
        /// <param name="pFilePath">Pointer to SQLite3 database file path string (UTF-8).</param>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="flags">Open options.</param>
        /// <param name="pVfs">Pointer to string of name of VFS module to use (UTF-8).</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult OpenV2Func(IntPtr pFilePath, out SqliteHandle db, SqliteOpenFlags flags, IntPtr pVfs);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        /// <param name="errmsgHandle">Error msg written here.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult ExecuteFunc(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmsgHandle);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Prepare"/> or <see cref="NativeMethods.PrepareW"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult PrepareFunc(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Prepare16"/> or <see cref="NativeMethods.Prepare16W"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pSql">Pointer to SQL to be evaluated (UTF-16).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult Prepare16Func(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.BlobOpen"/> or <see cref="NativeMethods.BlobOpenW"/>.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="pDbName">Pointer to UTF-8 byte-sequence of symbolic name of the database.</param>
        /// <param name="pTableName">Pointer to UTF-8 byte-sequence of table name.</param>
        /// <param name="pColumnName">Pointer to UTF-8 byte-sequence of column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        /// <param name="blobHandle">Output BLOB handle.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult BlobOpenFunc(SqliteHandle db, IntPtr pDbName, IntPtr pTableName, IntPtr pColumnName, long rowId, SqliteOpenFlags flags, out SqliteBlobHandle blobHandle);


        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.LibVersion"/> or <see cref="NativeMethods.LibVersionW"/>.
        /// </summary>
        private static readonly Func<IntPtr> _libVersion;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.LibSourceId"/> or <see cref="NativeMethods.LibSourceIdW"/>.
        /// </summary>
        private static readonly Func<IntPtr> _libSourceId;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.LibVersionNumber"/> or <see cref="NativeMethods.LibVersionNumberW"/>.
        /// </summary>
        private static readonly Func<int> _libVersionNumber;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        private static readonly OpenFunc _open;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.OpenV2"/> or <see cref="NativeMethods.OpenV2W"/>.
        /// </summary>
        private static readonly OpenV2Func _openV2;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Close"/> or <see cref="NativeMethods.CloseW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteResult> _close;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        private static readonly ExecuteFunc _execute;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Free"/> or <see cref="NativeMethods.FreeW"/>.
        /// </summary>
        private static readonly Action<IntPtr> _free;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.GetErrorMessage"/> or <see cref="NativeMethods.GetErrorMessageW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, IntPtr> _getErrorMessage;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.GetErrorString"/> or <see cref="NativeMethods.GetErrorStringW"/>.
        /// </summary>
        private static readonly Func<SqliteResult, IntPtr> _getErrorString;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Prepare"/> or <see cref="NativeMethods.PrepareW"/>.
        /// </summary>
        private static readonly PrepareFunc _prepare;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Prepare16"/> or <see cref="NativeMethods.Prepare16W"/>.
        /// </summary>
        private static readonly Prepare16Func _prepare16;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Sql"/> or <see cref="NativeMethods.SqlW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, IntPtr> _sql;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ExpandedSql"/> or <see cref="NativeMethods.ExpandedSqlW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, SqliteMemoryHandle> _expandedSql;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.NormalizedSql"/> or <see cref="NativeMethods.NormalizedSqlW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, IntPtr> _normalizedSql;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Step"/> or <see cref="NativeMethods.StepW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, SqliteResult> _step;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Finalize"/> or <see cref="NativeMethods.FinalizeW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteResult> _finalize;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindParameterCount"/> or <see cref="NativeMethods.BindParameterCountW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int> _bindParameterCount;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindParameterName"/> or <see cref="NativeMethods.BindParameterNameW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _bindParameterName;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindParameterIndex"/> or <see cref="NativeMethods.BindParameterIndexW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, IntPtr, int> _bindParameterIndex;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindInt"/> or <see cref="NativeMethods.BindIntW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, int, SqliteResult> _bindInt;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindInt64"/> or <see cref="NativeMethods.BindInt64W"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, long, SqliteResult> _bindInt64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindDouble"/> or <see cref="NativeMethods.BindDoubleW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, double, SqliteResult> _bindDouble;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindText"/> or <see cref="NativeMethods.BindTextW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, string, int, IntPtr, SqliteResult> _bindText;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindBlob"/> or <see cref="NativeMethods.BindBlobW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr, int, IntPtr, SqliteResult> _bindBlob;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindBlob64"/> or <see cref="NativeMethods.BindBlob64W"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr, ulong, IntPtr, SqliteResult> _bindBlob64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindZeroBlob"/> or <see cref="NativeMethods.BindZeroBlobW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, int, SqliteResult> _bindZeroBlob;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindZeroBlob"/> or <see cref="NativeMethods.BindZeroBlob64W"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, ulong, SqliteResult> _bindZeroBlob64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BindNull"/> or <see cref="NativeMethods.BindNullW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, SqliteResult> _bindNull;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Reset"/> or <see cref="NativeMethods.ResetW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, SqliteResult> _reset;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnCount"/> or <see cref="NativeMethods.ColumnCountW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int> _columnCount;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnName"/> or <see cref="NativeMethods.ColumnNameW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnName;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnValue"/> or <see cref="NativeMethods.ColumnValueW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnValue;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnInt"/> or <see cref="NativeMethods.ColumnIntW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, int> _columnInt;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnInt64"/> or <see cref="NativeMethods.ColumnInt64W"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, long> _columnInt64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnDouble"/> or <see cref="NativeMethods.ColumnDoubleW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, double> _columnDouble;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnText"/> or <see cref="NativeMethods.ColumnTextW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnText;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnBlob"/> or <see cref="NativeMethods.ColumnBlobW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnBlob;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnBytes"/> or <see cref="NativeMethods.ColumnBytesW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, int> _columnBytes;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnBytes16"/> or <see cref="NativeMethods.ColumnBytes16W"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, int> _columnBytes16;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueType"/> or <see cref="NativeMethods.ValueTypeW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteValueType> _valueType;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueInt"/> or <see cref="NativeMethods.ValueIntW"/>.
        /// </summary>
        private static readonly Func<IntPtr, int> _valueInt;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueInt64"/> or <see cref="NativeMethods.ValueInt64W"/>.
        /// </summary>
        private static readonly Func<IntPtr, long> _valueInt64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueDouble"/> or <see cref="NativeMethods.ValueDoubleW"/>.
        /// </summary>
        private static readonly Func<IntPtr, double> _valueDouble;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueText"/> or <see cref="NativeMethods.ValueTextW"/>.
        /// </summary>
        private static readonly Func<IntPtr, IntPtr> _valueText;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueBlob"/> or <see cref="NativeMethods.ValueBlobW"/>.
        /// </summary>
        private static readonly Func<IntPtr, IntPtr> _valueBlob;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueBytes"/> or <see cref="NativeMethods.ValueBytesW"/>.
        /// </summary>
        private static readonly Func<IntPtr, int> _valueBytes;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ValueBytes16"/> or <see cref="NativeMethods.ValueBytes16W"/>.
        /// </summary>
        private static readonly Func<IntPtr, int> _valueBytes16;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.LastInsertRowId"/> or <see cref="NativeMethods.LastInsertRowIdW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, long> _lastInsertRowId;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.SetLastInsertRowId"/> or <see cref="NativeMethods.SetLastInsertRowIdW"/>.
        /// </summary>
        private static readonly Action<SqliteHandle, long> _setLastInsertRowId;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Changes"/> or <see cref="NativeMethods.ChangesW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, int> _changes;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Changes64"/> or <see cref="NativeMethods.Changes64W"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, long> _changes64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.TotalChanges"/> or <see cref="NativeMethods.TotalChangesW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, int> _totalChanges;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.TotalChanges64"/> or <see cref="NativeMethods.TotalChanges64W"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, long> _totalChanges64;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobOpen"/> or <see cref="NativeMethods.BlobOpenW"/>.
        /// </summary>
        private static readonly BlobOpenFunc _blobOpen;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobReOpen"/> or <see cref="NativeMethods.BlobReOpenW"/>.
        /// </summary>
        private static readonly Func<SqliteBlobHandle, long, SqliteResult> _blobReOpen;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobClose"/> or <see cref="NativeMethods.BlobCloseW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteResult> _blobClose;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobBytes"/> or <see cref="NativeMethods.BlobBytesW"/>.
        /// </summary>
        private static readonly Func<SqliteBlobHandle, int> _blobBytes;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobRead"/> or <see cref="NativeMethods.BlobReadW"/>.
        /// </summary>
        private static readonly Func<SqliteBlobHandle, IntPtr, int, int, SqliteResult> _blobRead;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BlobWrite"/> or <see cref="NativeMethods.BlobWriteW"/>.
        /// </summary>
        private static readonly Func<SqliteBlobHandle, IntPtr, int, int, SqliteResult> _blobWrite;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Interrupt"/> or <see cref="NativeMethods.InterruptW"/>.
        /// </summary>
        private static readonly Action<SqliteHandle> _interrupt;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.IsInterrupted"/> or <see cref="NativeMethods.IsInterruptedW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, bool> _isInterrupted;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BusyTimeout"/> or <see cref="NativeMethods.BusyTimeoutW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, int, SqliteResult> _busyTimeout;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.BusyHandler"/> or <see cref="NativeMethods.BusyHandlerW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, BusyCallbackFunc, IntPtr, SqliteResult> _busyHandler;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.MemoryUsed"/> or <see cref="NativeMethods.MemoryUsedW"/>.
        /// </summary>
        private static readonly Func<long> _memoryUsed;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.MemoryHighWater"/> or <see cref="NativeMethods.MemoryHighWaterW"/>.
        /// </summary>
        private static readonly Func<bool, long> _memoryHighWater;


        /// <summary>
        /// Setup delegates for P/Invoke functions.
        /// </summary>
        static SqliteLibrary()
        {
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            var useWinSqlite = false;
            try
            {
                // Try to load sqlite3.dll.
                // If sqlite3.dll is not found, DllNotFoundException is thrown.
                NativeMethods.Free(IntPtr.Zero);
            }
            catch (DllNotFoundException)
            {
                try
                {
                    // Fallback to winsqlite3.dll
                    NativeMethods.FreeW(IntPtr.Zero);
                    useWinSqlite = true;
                }
                catch (DllNotFoundException)
                {
                    // If winsqlite.dll not found, Refer to sqlite3.dll, assuming the user will add it later.
                }
            }

            if (useWinSqlite)
            {
                _libVersion = NativeMethods.LibVersionW;
                _libSourceId = NativeMethods.LibSourceIdW;
                _libVersionNumber = NativeMethods.LibVersionNumberW;
                _open = NativeMethods.OpenW;
                _openV2 = NativeMethods.OpenV2W;
                _close = NativeMethods.CloseW;
                _execute = NativeMethods.ExecuteW;
                _free = NativeMethods.FreeW;
                _getErrorMessage = NativeMethods.GetErrorMessageW;
                _getErrorString = NativeMethods.GetErrorStringW;
                _prepare = NativeMethods.PrepareW;
                _prepare16 = NativeMethods.Prepare16W;
                _sql = NativeMethods.SqlW;
                _expandedSql = NativeMethods.ExpandedSqlW;
                _normalizedSql = NativeMethods.NormalizedSqlW;
                _step = NativeMethods.StepW;
                _finalize = NativeMethods.FinalizeW;
                _bindParameterCount = NativeMethods.BindParameterCountW;
                _bindParameterName = NativeMethods.BindParameterNameW;
                _bindParameterIndex = NativeMethods.BindParameterIndexW;
                _bindInt = NativeMethods.BindIntW;
                _bindInt64 = NativeMethods.BindInt64W;
                _bindDouble = NativeMethods.BindDoubleW;
                _bindText = NativeMethods.BindTextW;
                _bindBlob =  NativeMethods.BindBlobW;
                _bindBlob64 =  NativeMethods.BindBlob64W;
                _bindZeroBlob =  NativeMethods.BindZeroBlobW;
                _bindZeroBlob64 =  NativeMethods.BindZeroBlob64W;
                _bindNull = NativeMethods.BindNullW;
                _reset = NativeMethods.ResetW;
                _columnCount = NativeMethods.ColumnCountW;
                _columnName = NativeMethods.ColumnNameW;
                _columnValue = NativeMethods.ColumnValueW;
                _columnInt = NativeMethods.ColumnIntW;
                _columnInt64 = NativeMethods.ColumnInt64W;
                _columnDouble = NativeMethods.ColumnDoubleW;
                _columnText = NativeMethods.ColumnTextW;
                _columnBlob = NativeMethods.ColumnBlobW;
                _columnBytes = NativeMethods.ColumnBytesW;
                _columnBytes16 = NativeMethods.ColumnBytes16W;
                _valueType = NativeMethods.ValueTypeW;
                _valueInt = NativeMethods.ValueIntW;
                _valueInt64 = NativeMethods.ValueInt64W;
                _valueDouble = NativeMethods.ValueDoubleW;
                _valueText = NativeMethods.ValueTextW;
                _valueBlob = NativeMethods.ValueBlobW;
                _valueBytes = NativeMethods.ValueBytesW;
                _valueBytes16 = NativeMethods.ValueBytes16W;
                _lastInsertRowId = NativeMethods.LastInsertRowIdW;
                _setLastInsertRowId = NativeMethods.SetLastInsertRowIdW;
                _changes = NativeMethods.ChangesW;
                _changes64 = NativeMethods.Changes64W;
                _totalChanges = NativeMethods.TotalChangesW;
                _totalChanges64 = NativeMethods.TotalChanges64W;
                _blobOpen = NativeMethods.BlobOpenW;
                _blobReOpen = NativeMethods.BlobReOpenW;
                _blobClose = NativeMethods.BlobCloseW;
                _blobBytes = NativeMethods.BlobBytesW;
                _blobWrite = NativeMethods.BlobWriteW;
                _blobRead = NativeMethods.BlobReadW;
                _interrupt = NativeMethods.InterruptW;
                _isInterrupted = NativeMethods.IsInterruptedW;
                _busyTimeout = NativeMethods.BusyTimeoutW;
                _busyHandler = NativeMethods.BusyHandlerW;
                _memoryUsed = NativeMethods.MemoryUsedW;
                _memoryHighWater = NativeMethods.MemoryHighWaterW;
            }
            else
            {
#endif
                _libVersion = NativeMethods.LibVersion;
                _libSourceId = NativeMethods.LibSourceId;
                _libVersionNumber = NativeMethods.LibVersionNumber;
                _open = NativeMethods.Open;
                _openV2 = NativeMethods.OpenV2;
                _close = NativeMethods.Close;
                _execute = NativeMethods.Execute;
                _free = NativeMethods.Free;
                _getErrorMessage = NativeMethods.GetErrorMessage;
                _getErrorString = NativeMethods.GetErrorString;
                _prepare = NativeMethods.Prepare;
                _prepare16 = NativeMethods.Prepare16W;
                _sql = NativeMethods.Sql;
                _expandedSql = NativeMethods.ExpandedSql;
                _normalizedSql = NativeMethods.NormalizedSql;
                _step = NativeMethods.Step;
                _finalize = NativeMethods.Finalize;
                _bindParameterCount = NativeMethods.BindParameterCount;
                _bindParameterName = NativeMethods.BindParameterName;
                _bindParameterIndex = NativeMethods.BindParameterIndex;
                _bindInt = NativeMethods.BindInt;
                _bindInt64 = NativeMethods.BindInt64;
                _bindDouble = NativeMethods.BindDouble;
                _bindText = NativeMethods.BindText;
                _bindBlob =  NativeMethods.BindBlob;
                _bindBlob64 =  NativeMethods.BindBlob64;
                _bindZeroBlob =  NativeMethods.BindZeroBlob;
                _bindZeroBlob64 =  NativeMethods.BindZeroBlob64;
                _bindNull = NativeMethods.BindNull;
                _reset = NativeMethods.Reset;
                _columnCount = NativeMethods.ColumnCount;
                _columnName = NativeMethods.ColumnName;
                _columnValue = NativeMethods.ColumnValue;
                _columnInt = NativeMethods.ColumnInt;
                _columnInt64 = NativeMethods.ColumnInt64;
                _columnDouble = NativeMethods.ColumnDouble;
                _columnText = NativeMethods.ColumnText;
                _columnBlob = NativeMethods.ColumnBlob;
                _columnBytes = NativeMethods.ColumnBytes;
                _columnBytes16 = NativeMethods.ColumnBytes16;
                _valueType = NativeMethods.ValueType;
                _valueInt = NativeMethods.ValueInt;
                _valueInt64 = NativeMethods.ValueInt64;
                _valueDouble = NativeMethods.ValueDouble;
                _valueText = NativeMethods.ValueText;
                _valueBlob = NativeMethods.ValueBlob;
                _valueBytes = NativeMethods.ValueBytes;
                _valueBytes16 = NativeMethods.ValueBytes16;
                _lastInsertRowId = NativeMethods.LastInsertRowId;
                _setLastInsertRowId = NativeMethods.SetLastInsertRowId;
                _changes = NativeMethods.Changes;
                _changes64 = NativeMethods.Changes64;
                _totalChanges = NativeMethods.TotalChanges;
                _totalChanges64 = NativeMethods.TotalChanges64;
                _blobOpen = NativeMethods.BlobOpen;
                _blobReOpen = NativeMethods.BlobReOpen;
                _blobClose = NativeMethods.BlobClose;
                _blobBytes = NativeMethods.BlobBytes;
                _blobWrite = NativeMethods.BlobWrite;
                _blobRead = NativeMethods.BlobRead;
                _interrupt = NativeMethods.Interrupt;
                _isInterrupted = NativeMethods.IsInterrupted;
                _busyTimeout = NativeMethods.BusyTimeout;
                _busyHandler = NativeMethods.BusyHandler;
                _memoryUsed = NativeMethods.MemoryUsed;
                _memoryHighWater = NativeMethods.MemoryHighWater;
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            }
#endif
            var version = LibVersion();
            var versionNumber = _libVersionNumber();
            if (versionNumber < 3037000)
            {
                _changes64 = _ =>
                {
                    throw new NotSupportedException("sqlite3_changes64 is available version 3.37.0 or later, but library version is " + version);
                };
                _totalChanges64 = _ =>
                {
                    throw new NotSupportedException("sqlite3_total_changes64 is available version 3.37.0 or later, but library version is " + version);
                };
            }
            if (versionNumber < 3041000)
            {
                _isInterrupted = _ =>
                {
                    throw new NotSupportedException("sqlite3_is_interrupted is available version 3.41.0 or later, but library version is " + version);
                };
            }
        }

        /// <summary>
        /// Try to load sqlite3.dll (and winsqlite3.dll).
        /// </summary>
        /// <returns>True if the load succeeded, otherwise false.</returns>
        public static bool TryLoad()
        {
            try
            {
                _free(IntPtr.Zero);
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get Run-Time Library Version Numbers String.
        /// </summary>
        /// <returns>A pointer to the to the version string constant.</returns>
        public static string LibVersion()
        {
            return Marshal.PtrToStringAnsi(_libVersion());
        }

        /// <summary>
        /// Get Run-Time Library Source ID.
        /// </summary>
        /// <returns>Source ID which contains the date and time of the check-in (UTC) and a SHA1 or SHA3-256 hash of the entire source tree.</returns>
        public static string LibSourceId()
        {
            return Marshal.PtrToStringAnsi(_libSourceId());
        }

        /// <summary>
        /// Get Run-Time Library Version Numbers represents in <see cref="int"/>.
        /// </summary>
        /// <returns>
        /// An integer with a value (X * 1000000 + Y + 1000 + Z)
        /// where X, Y and Z are the components of SQLite version number string, "X.Y.Z".
        /// </returns>
        public static int LibVersionNumber()
        {
            return _libVersionNumber();
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <returns>SQLite db handle.</returns>
        public static SqliteHandle Open(string filePath)
        {
            var result = _open(filePath, out var db);
            SqliteException.ThrowIfFailed("sqlite3_open16", result, db, "Failed to open: " + filePath);
            return db;
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        /// <returns>SQLite db handle.</returns>
        public static SqliteHandle Open(string filePath, SqliteOpenFlags flags)
        {
            return Open(filePath, flags, null);
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="flags">Open options.</param>
        /// <param name="vfs">Name of VFS module to use.</param>
        /// <returns>SQLite db handle.</returns>
        public static SqliteHandle Open(string filePath, SqliteOpenFlags flags, string vfs = null)
        {
            var utf8FilePathBytes = filePath == null ? null : Encoding.UTF8.GetBytes(filePath);
            var utf8VfsBytes = vfs == null ? null : Encoding.UTF8.GetBytes(vfs);
            unsafe
            {
                fixed (byte *pUtf8FilePath = utf8FilePathBytes)
                fixed (byte *pUtf8Vfs = utf8VfsBytes)
                {
                    var result = _openV2((IntPtr)pUtf8FilePath, out var db, flags, (IntPtr)pUtf8Vfs);
                    SqliteException.ThrowIfFailed("sqlite3_open_v2", result, db, "Failed to open: " + filePath);
                    return db;
                }
            }
        }

        /// <summary>
        /// Close database.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        internal static SqliteResult Close(IntPtr db)
        {
            return _close(db);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        public static void Execute(SqliteHandle db, string sql)
        {
            Execute(db, sql, null, IntPtr.Zero);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        public static void Execute(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg = default)
        {
            var result = _execute(db, sql, callback, callbackArg, out var errmsgHandle);
            if (result != SqliteResult.OK)
            {
                using (errmsgHandle)
                {
                    SqliteException.Throw("sqlite3_exec", result, PtrToStringUTF8(errmsgHandle.DangerousGetHandle()));
                }
            }
        }

        /// <summary>
        /// Free memory allocated in SQLite3 functions.
        /// </summary>
        /// <param name="pMemory">Allocated memory pointer.</param>
        internal static void Free(IntPtr pMemory)
        {
            _free(pMemory);
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sqlUtf8Bytes">UTF-8 byte sequence of SQL to be evaluated.</param>
        /// <returns>Statement handle.</returns>
        public static SqliteStatementHandle Prepare(SqliteHandle db, byte[] sqlUtf8Bytes)
        {
            unsafe
            {
                fixed (byte *pbSqlBase = &sqlUtf8Bytes[0])
                {
                    return Prepare(db, pbSqlBase, sqlUtf8Bytes.Length, out _);
                }
            }
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sqlUtf8Bytes">UTF-8 byte sequence of SQL to be evaluated.</param>
        /// <param name="offset">Offset position of <paramref name="sqlUtf8Bytes"/>.</param>
        /// <returns>Statement handle.</returns>
        public static SqliteStatementHandle Prepare(SqliteHandle db, byte[] sqlUtf8Bytes, ref int offset)
        {
            unsafe
            {
                fixed (byte *pbSqlBase = &sqlUtf8Bytes[offset])
                {
                    byte *pbSqlNext;
                    var stmt = Prepare(db, pbSqlBase, sqlUtf8Bytes.Length, out pbSqlNext);
                    offset = pbSqlNext == pbSqlBase ? -1 : (offset + (int)(pbSqlNext - pbSqlBase));
                    return stmt;
                }
            }
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pbSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, ref byte *pbSql, ref int nBytes)
        {
            var result = Prepare(db, pbSql, nBytes, out var pbSqlNext);
            if (nBytes >= 0)
            {
                nBytes -= (int)(pbSqlNext - pbSql);
            }
            pbSql = pbSqlNext;
            return result;
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pbSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="pbSqlNext">Pointer to unused portion of <paramref name="pbSql"/>.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, byte *pbSql, int nBytes, out byte *pbSqlNext)
        {
            var result = _prepare(db, (IntPtr)pbSql, nBytes, out var stmt, out var pSqlNext);
            SqliteException.ThrowIfFailed("sqlite3_prepare_v2", result, db);
            pbSqlNext = (byte *)pSqlNext;
            return stmt;
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <returns>Statement handle.</returns>
        public static SqliteStatementHandle Prepare(SqliteHandle db, string sql)
        {
            unsafe
            {
                fixed (char *pbSqlBase = sql)
                {
                    return Prepare(db, pbSqlBase, sql.Length * sizeof(char), out _);
                }
            }
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="offset">Offset position of <paramref name="sql"/>.</param>
        /// <returns>Statement handle.</returns>
        public static SqliteStatementHandle Prepare(SqliteHandle db, string sql, ref int offset)
        {
            unsafe
            {
                fixed (char *pcSqlBase = sql)
                {
                    char* pcSql = &pcSqlBase[offset];
                    char *pcSqlNext;
                    var stmt = Prepare(db, pcSql, (sql.Length - offset) * sizeof(char), out pcSqlNext);
                    offset = pcSqlNext == pcSql ? -1 : (offset + (int)(pcSqlNext - pcSql));
                    return stmt;
                }
            }
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pcSql">Pointer to SQL to be evaluated (UTF-16).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, ref char *pcSql, ref int nBytes)
        {
            var result = Prepare(db, pcSql, nBytes, out var pbSqlNext);
            if (nBytes >= 0)
            {
                nBytes -= (int)((byte *)pbSqlNext - (byte *)pcSql);
            }
            pcSql = pbSqlNext;
            return result;
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pcSql">Pointer to SQL to be evaluated (UTF-16).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="pcSqlNext">Pointer to unused portion of <paramref name="pcSql"/>.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, char *pcSql, int nBytes, out char *pcSqlNext)
        {
            var result = _prepare16(db, (IntPtr)pcSql, nBytes, out var stmt, out var pSqlNext);
            SqliteException.ThrowIfFailed("sqlite3_prepare16_v2", result, db);
            pcSqlNext = (char *)pSqlNext;
            return stmt;
        }

        /// <summary>
        /// Retrive SQL text.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>SQL text used to create prepared statement.</returns>
        public static string Sql(SqliteStatementHandle stmt)
        {
            return PtrToStringUTF8(_sql(stmt));
        }

        /// <summary>
        /// Retrive SQL text with bound parameters expanded.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>SQL text of prepared statement with bound parameters expanded.</returns>
        public static string ExpandedSql(SqliteStatementHandle stmt)
        {
            using (var sqlHandle = _expandedSql(stmt))
            {
                return PtrToStringUTF8(sqlHandle.DangerousGetHandle());
            }
        }

        /// <summary>
        /// Retrive normalized SQL text.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>Normalized SQL text of prepared statement.</returns>
        public static string NormalizedSql(SqliteStatementHandle stmt)
        {
            return PtrToStringUTF8(_normalizedSql(stmt));
        }

        /// <summary>
        /// Evaluate an SQL statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>True if row exists, otherwise false.</returns>
        public static bool Step(SqliteStatementHandle stmt)
        {
            var result = _step(stmt);
            switch (result)
            {
                case SqliteResult.Done:
                case SqliteResult.Misuse:  // Skip trailing white space or comment.
                    return false;
                case SqliteResult.Row:
                    return true;
                default:
                    SqliteException.ThrowIfFailed("sqlite3_step", result);
                    break;
            }

            return false;
        }

        /// <summary>
        /// Evaluate an SQL statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="db">An open database to get error message with <see cref="GetErrorMessage(SqliteHandle)"/>.</param>
        /// <returns>True if row exists, otherwise false.</returns>
        public static bool Step(SqliteStatementHandle stmt, SqliteHandle db)
        {
            var result = _step(stmt);
            switch (result)
            {
                case SqliteResult.Done:
                case SqliteResult.Misuse:  // Skip trailing white space or comment.
                    return false;
                case SqliteResult.Row:
                    return true;
                default:
                    SqliteException.Throw("sqlite3_step", result, GetErrorMessage(db));
                    break;
            }

            return false;
        }

        /// <summary>
        /// Destroy a prepared statement object.
        /// </summary>
        /// <param name="pStmt">Statement handle.</param>
        /// <returns>Result code.</returns>
        internal static SqliteResult Finalize(IntPtr pStmt)
        {
            return _finalize(pStmt);
        }

        /// <summary>
        /// Get the number of columns in a result set returned by the prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>The number of columns in the result set.</returns>
        public static int ColumnCount(SqliteStatementHandle stmt)
        {
            return _columnCount(stmt);
        }

        /// <summary>
        /// Get column name in a result set.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column name.</returns>
        public static string ColumnName(SqliteStatementHandle stmt, int index)
        {
            return Marshal.PtrToStringUni(_columnName(stmt, index));
        }

        /// <summary>
        /// Count number of SQL parameters.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>Number of SQL parameters.</returns>
        public static int BindParameterCount(SqliteStatementHandle stmt)
        {
            return _bindParameterCount(stmt);
        }

        /// <summary>
        /// Get name of a host parameter.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <returns>Bind parameter name.</returns>
        public static string BindParameterName(SqliteStatementHandle stmt, int index)
        {
            return PtrToStringUTF8(_bindParameterName(stmt, index));
        }

        /// <summary>
        /// Get index of a parameter with a given name.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="name">Parameter name (UTF-8).</param>
        /// <returns>Index of the parameter.</returns>
        public static int BindParameterIndex(SqliteStatementHandle stmt, string name)
        {
            unsafe
            {
                fixed (byte *pbName = Encoding.UTF8.GetBytes(name))
                {
                    return _bindParameterIndex(stmt, (IntPtr)pbName);
                }
            }
        }

        /// <summary>
        /// Bind <see cref="int"/> value to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, int val)
        {
            var result = _bindInt(stmt, index, val);
            SqliteException.ThrowIfFailed("sqlite3_bind_int", result);
        }

        /// <summary>
        /// Bind <see cref="long"/> value to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, long val)
        {
            var result = _bindInt64(stmt, index, val);
            SqliteException.ThrowIfFailed("sqlite3_bind_int64", result);
        }

        /// <summary>
        /// Bind <see cref="double"/> value to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, double val)
        {
            var result = _bindDouble(stmt, index, val);
            SqliteException.ThrowIfFailed("sqlite3_bind_double", result);
        }

        /// <summary>
        /// Bind <see cref="string"/> value to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        /// <param name="n">Max length of the string value, <paramref name="val"/>.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, string val, int n = -1)
        {
            var result = _bindText(stmt, index, val, n, (IntPtr)SqliteDestructorType.Transient);
            SqliteException.ThrowIfFailed("sqlite3_bind_text", result);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, byte[] blob)
        {
            Bind(stmt, index, blob, 0UL, (ulong)blob.LongLength);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, byte[] blob, int offset, int length)
        {
            unsafe
            {
                fixed (byte *pBlob = &blob[offset])
                {
                    Bind(stmt, index, (IntPtr)pBlob, length, SqliteDestructorType.Transient);
                }
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, byte[] blob, ulong offset, ulong length)
        {
            unsafe
            {
                fixed (byte *pBlob = &blob[offset])
                {
                    Bind(stmt, index, (IntPtr)pBlob, length, SqliteDestructorType.Transient);
                }
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, IntPtr pBlob, int length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            var result = _bindBlob(stmt, index, pBlob, length, (IntPtr)dtorType);
            SqliteException.ThrowIfFailed("sqlite3_bind_blob", result);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public static void Bind(SqliteStatementHandle stmt, int index, IntPtr pBlob, ulong length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            var result = _bindBlob64(stmt, index, pBlob, length, (IntPtr)dtorType);
            SqliteException.ThrowIfFailed("sqlite3_bind_blob64", result);
        }

        /// <summary>
        /// Bind BLOB data of length that is filled with zeros to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
        public static void BindZeroBlob(SqliteStatementHandle stmt, int index, int n)
        {
            var result = _bindZeroBlob(stmt, index, n);
            SqliteException.ThrowIfFailed("sqlite3_bind_zeroblob", result);
        }

        /// <summary>
        /// Bind BLOB data of length that is filled with zeros to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
        public static void BindZeroBlob(SqliteStatementHandle stmt, int index, ulong n)
        {
            var result = _bindZeroBlob64(stmt, index, n);
            SqliteException.ThrowIfFailed("sqlite3_bind_zeroblob64", result);
        }

        /// <summary>
        /// Bind NULL to prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of parameter (1-based).</param>
        public static void BindNull(SqliteStatementHandle stmt, int index)
        {
            var result = _bindNull(stmt, index);
            SqliteException.ThrowIfFailed("sqlite3_bind_null", result);
        }

        /// <summary>
        /// Reset a prepared statement object.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        public static void Reset(SqliteStatementHandle stmt)
        {
            var result = _reset(stmt);
            SqliteException.ThrowIfFailed("sqlite3_reset", result);
        }

        /// <summary>
        /// Get pointer to result value object from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Pointer to result value object.</returns>
        public static IntPtr ColumnValue(SqliteStatementHandle stmt, int index)
        {
            return _columnValue(stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public static int ColumnInt(SqliteStatementHandle stmt, int index)
        {
            return _columnInt(stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public static long ColumnInt64(SqliteStatementHandle stmt, int index)
        {
            return _columnInt64(stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public static double ColumnDouble(SqliteStatementHandle stmt, int index)
        {
            return _columnDouble(stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public static string ColumnText(SqliteStatementHandle stmt, int index)
        {
            return Marshal.PtrToStringUni(_columnText(stmt, index));
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public static byte[] ColumnBlob(SqliteStatementHandle stmt, int index)
        {
            var pBlob = _columnBlob(stmt, index);
            if (pBlob == IntPtr.Zero)
            {
                return null;
            }
            var blob = new byte[_columnBytes(stmt, index)];
            Marshal.Copy(pBlob, blob, 0, blob.Length);
            return blob;
        }

        /// <summary>
        /// Get size of a BLOB or a UTF-8 TEXT result in bytes.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Size of a BLOB or a UTF-8 TEXT result in bytes.</returns>
        public static int ColumnBytes(SqliteStatementHandle stmt, int index)
        {
            return _columnBytes(stmt, index);
        }

        /// <summary>
        /// Get size of a UTF-16 TEXT result in bytes.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Size of a UTF-16 TEXT result in bytes.</returns>
        public static int ColumnBytes16(SqliteStatementHandle stmt, int index)
        {
            return _columnBytes16(stmt, index);
        }

        /// <summary>
        /// Get type of result value.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Enum value of type of result value.</returns>
        public static SqliteValueType ValueType(IntPtr pColumnValue)
        {
            return _valueType(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a result value object.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Column value <see cref="int"/>.</returns>
        public static int ValueInt(IntPtr pColumnValue)
        {
            return _valueInt(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a result value object.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public static long ValueInt64(IntPtr pColumnValue)
        {
            return _valueInt64(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a result value object.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public static double ValueDouble(IntPtr pColumnValue)
        {
            return _valueDouble(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a result value object.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public static string ValueText(IntPtr pColumnValue)
        {
            return Marshal.PtrToStringUni(_valueText(pColumnValue));
        }

        /// <summary>
        /// Get result value as BLOB from a result value object.
        /// </summary>
        /// <param name="pColumnValue">Pointer to result value object.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public static byte[] ValueBlob(IntPtr pColumnValue)
        {
            var pBlob = _valueBlob(pColumnValue);
            if (pBlob == IntPtr.Zero)
            {
                return null;
            }
            var blob = new byte[_valueBytes(pColumnValue)];
            Marshal.Copy(pBlob, blob, 0, blob.Length);
            return blob;
        }

        /// <summary>
        /// Count the number of rows modified.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>The number of rows modified, inserted or deleted.</returns>
        public static int Changes(SqliteHandle db)
        {
            return _changes(db);
        }

        /// <summary>
        /// Count the number of rows modified (64-bit signed integer counter).
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>The number of rows modified, inserted or deleted.</returns>
        public static long Changes64(SqliteHandle db)
        {
            return _changes64(db);
        }

        /// <summary>
        /// Last Insert Rowid.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Last insert ROWID.</returns>
        public static long LastInsertRowId(SqliteHandle db)
        {
            return _lastInsertRowId(db);
        }

        /// <summary>
        /// Set the Last Insert Rowid value.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="rowId">ROWID to set.</param>
        public static void SetLastInsertRowId(SqliteHandle db, long rowId)
        {
            _setLastInsertRowId(db, rowId);
        }

        /// <summary>
        /// Count the total number of rows modified since the database connection was opened.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
        public static int TotalChanges(SqliteHandle db)
        {
            return _totalChanges(db);
        }

        /// <summary>
        /// Count the total number of rows modified since the database connection was opened (64-bit signed integer counter).
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
        public static long TotalChanges64(SqliteHandle db)
        {
            return _totalChanges64(db);
        }

        /// <summary>
        /// Open a BLOB for incremental I/O.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="dbName">Symbolic name of the database.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        /// <returns>Open BLOB handle.</returns>
        public static SqliteBlobHandle BlobOpen(SqliteHandle db, string dbName, string tableName, string columnName, long rowId, SqliteOpenFlags flags)
        {
            unsafe
            {
                fixed (byte *pDbName = Encoding.UTF8.GetBytes(dbName))
                fixed (byte *pTableName = Encoding.UTF8.GetBytes(tableName))
                fixed (byte *pColumnName = Encoding.UTF8.GetBytes(columnName))
                {
                    SqliteBlobHandle blobHandle;
                    var result = _blobOpen(db, (IntPtr)pDbName, (IntPtr)pTableName, (IntPtr)pColumnName, rowId, flags, out blobHandle);
                    SqliteException.ThrowIfFailed("sqlite3_blob_open", result);
                    return blobHandle;
                }
            }
        }

        /// <summary>
        /// Move a BLOB handle to a new row.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <param name="rowId">New ROWID of BLOB.</param>
        public static void BlobReOpen(SqliteBlobHandle blobHandle, long rowId)
        {
            var result = _blobReOpen(blobHandle, rowId);
            SqliteException.ThrowIfFailed("sqlite3_blob_reopen", result);
        }

        /// <summary>
        /// Close a BLOB handle.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle to close.</param>
        internal static SqliteResult BlobClose(IntPtr blobHandle)
        {
            return _blobClose(blobHandle);
        }

        /// <summary>
        /// Return the size of an open BLOB.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <returns>The size of an open BLOB.</returns>
        public static int BlobBytes(SqliteBlobHandle blobHandle)
        {
            return _blobBytes(blobHandle);
        }

        /// <summary>
        /// Read data from a BLOB incrementally.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <param name="data">Buffer to read.</param>
        /// <param name="offset">Byte offset of <paramref name="data"/>.</param>
        /// <param name="count">Data size to copy in bytes.</param>
        /// <param name="blobOffset">Offset of BLOB.</param>
        public static void BlobRead(SqliteBlobHandle blobHandle, byte[] data, int offset, int count, int blobOffset)
        {
            unsafe
            {
                fixed (byte *p = &data[offset])
                {
                    BlobRead(blobHandle, (IntPtr)p, count, blobOffset);
                }
            }
        }

        /// <summary>
        /// Read data from a BLOB incrementally.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <param name="pData">Pointer to buffer to read.</param>
        /// <param name="count">Data size to copy in bytes.</param>
        /// <param name="blobOffset">Offset of BLOB.</param>
        public static void BlobRead(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset)
        {
            var result = _blobRead(blobHandle, pData, count, blobOffset);
            SqliteException.ThrowIfFailed("sqlite3_blob_read", result);
        }

        /// <summary>
        /// Write data into a BLOB incrementally.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <param name="data">Buffer to write.</param>
        /// <param name="offset">Offset of <paramref name="data"/>.</param>
        /// <param name="count">Data size to copy in bytes.</param>
        /// <param name="blobOffset">Offset of BLOB.</param>
        public static void BlobWrite(SqliteBlobHandle blobHandle, byte[] data, int offset, int count, int blobOffset)
        {
            unsafe
            {
                fixed (byte *p = &data[offset])
                {
                    BlobWrite(blobHandle, (IntPtr)p, count, blobOffset);
                }
            }
        }

        /// <summary>
        /// Write data into a BLOB incrementally.
        /// </summary>
        /// <param name="blobHandle">A BLOB handle.</param>
        /// <param name="pData">Pointer to buffer to write.</param>
        /// <param name="count">Data size to copy in bytes.</param>
        /// <param name="blobOffset">Offset of BLOB.</param>
        public static void BlobWrite(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset)
        {
            var result = _blobWrite(blobHandle, pData, count, blobOffset);
            SqliteException.ThrowIfFailed("sqlite3_blob_write", result);
        }

        /// <summary>
        /// Interrupt a long-running query.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        public static void Interrupt(SqliteHandle db)
        {
            _interrupt(db);
        }

        /// <summary>
        /// Determine whether or not an interrupt is currently in effect for database connection.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>True if an interrupt is currently in effect, or false otherwise.</returns>
        public static bool IsInterrupted(SqliteHandle db)
        {
            return _isInterrupted(db);
        }

        /// <summary>
        /// Set a busy timeout.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="ms">Sleep time in milliseconds.</param>
        public static void BusyTimeout(SqliteHandle db, int ms)
        {
            var result = _busyTimeout(db, ms);
            SqliteException.ThrowIfFailed("sqlite3_busy_timeout", result);
        }

        /// <summary>
        /// Register A Callback To Handle <see cref="SqliteResult.Busy"/> Errors.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">First argument to callback.</param>
        public static void BusyHandler(SqliteHandle db, BusyCallbackFunc callback, IntPtr callbackArg = default(IntPtr))
        {
            var result = _busyHandler(db, callback, callbackArg);
            SqliteException.ThrowIfFailed("sqlite3_busy_handler", result);
        }

        /// <summary>
        /// Get latest error message occured in SQLite3 functions.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Latest error message (UTF-8).</returns>
        public static string GetErrorMessage(SqliteHandle db)
        {
            return Marshal.PtrToStringUni(_getErrorMessage(db));
        }

        /// <summary>
        /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
        /// </summary>
        /// <param name="result">Result code.</param>
        /// <returns>English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
        public static string GetErrorString(SqliteResult result)
        {
            return PtrToStringUTF8(_getErrorString(result));
        }

        /// <summary>
        /// Get the number of bytes of memory currentry outstanding.
        /// </summary>
        /// <returns>The number of bytes of memory currentry outstanding (malloced but not freed).</returns>
        public static long MemoryUsed()
        {
            return _memoryUsed();
        }

        /// <summary>
        /// Get the maximum value of <see cref="MemoryUsed"/>.
        /// </summary>
        /// <param name="resetFlag">True to reset high-water mark to the current value of <see cref="MemoryUsed"/>.</param>
        /// <returns>The maximum value of <see cref="MemoryUsed"/> since the high-water mark was last reset.</returns>
        public static long MemoryHighWater(bool resetFlag)
        {
            return _memoryHighWater(resetFlag);
        }


        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="p">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static string PtrToStringUTF8(IntPtr p)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8(p);
#else
            unsafe
            {
                return PtrToStringUTF8((sbyte *)p);
            }
#endif
        }

        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="psb">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static unsafe string PtrToStringUTF8(sbyte *psb)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8((IntPtr)psb);
#else
            return new string(psb, 0, ByteLengthOf(psb), Encoding.UTF8);
#endif
        }

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP1_1_OR_GREATER
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
#endif

        /// <summary>
        /// Provides some native methods of SQLite3.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// Get Run-Time Library Version Numbers String.
            /// </summary>
            /// <returns>A pointer to the version string constant.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_libversion", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr LibVersion();

            /// <summary>
            /// Get Run-Time Library Source ID.
            /// </summary>
            /// <returns>A pointer to the source ID constant which contains the date and time of the check-in (UTC) and a SHA1 or SHA3-256 hash of the entire source tree.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_sourceid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr LibSourceId();

            /// <summary>
            /// Get Run-Time Library Version Numbers represents in <see cref="int"/>.
            /// </summary>
            /// <returns>
            /// An integer with a value (X * 1000000 + Y + 1000 + Z)
            /// where X, Y and Z are the components of SQLite version number string, "X.Y.Z".
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int LibVersionNumber();

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">SQLite3 database file path.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_open16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern SqliteResult Open([MarshalAs(UnmanagedType.LPWStr), In] string filePath, out SqliteHandle db);

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="pFilePath">Pointer to SQLite3 database file path string (UTF-8).</param>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="flags">Open options.</param>
            /// <param name="pVfs">Pointer to string of name of VFS module to use (UTF-8).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult OpenV2(IntPtr pFilePath, out SqliteHandle db, SqliteOpenFlags flags, IntPtr pVfs);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_close", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Close(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="errmgHandle">Error msg written here (must be release with <see cref="Free"/>).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_exec", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Execute(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmgHandle);

            /// <summary>
            /// Free memory allocated in SQLite3 functions.
            /// </summary>
            /// <param name="pMemory">Allocated memory pointer.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/free.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_free", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern void Free(IntPtr pMemory);

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Prepare(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-16).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="sql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_prepare16_v2", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Prepare16(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Retrive SQL text.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>A pointer to a copy of the UTF-8 SQL text used to create prepared statement.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_sql", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Sql(SqliteStatementHandle stmt);

            /// <summary>
            /// Retrive SQL text with bound parameters expanded.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Handle of a pointer to a UTF-8 string containing the SQL text of prepared statement with bound parameters expanded.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_expanded_sql", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteMemoryHandle ExpandedSql(SqliteStatementHandle stmt);

            /// <summary>
            /// Retrive normalized SQL text.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>A pointer to a UTF-8 string containing the normalized SQL text of prepared statement.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_normalized_sql", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr NormalizedSql(SqliteStatementHandle stmt);

            /// <summary>
            /// Evaluate an SQL statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/step.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_step", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Step(SqliteStatementHandle stmt);

            /// <summary>
            /// Destroy a prepared statement object.
            /// </summary>
            /// <param name="pStmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/finalize.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Finalize(IntPtr pStmt);

            /// <summary>
            /// Count number of SQL parameters.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Number of SQL parameters.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_count.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_count", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int BindParameterCount(SqliteStatementHandle stmt);

            /// <summary>
            /// Get name of a host parameter.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <returns>Pointer to bind parameter name (UTF-8).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_name.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_name", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr BindParameterName(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get index of a parameter with a given name.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pName">Pointer to parameter name (UTF-8).</param>
            /// <returns>Index of the parameter.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_index.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int BindParameterIndex(SqliteStatementHandle stmt, IntPtr pName);

            /// <summary>
            /// Bind <see cref="int"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindInt(SqliteStatementHandle stmt, int index, int val);

            /// <summary>
            /// Bind <see cref="long"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindInt64(SqliteStatementHandle stmt, int index, long val);

            /// <summary>
            /// Bind <see cref="double"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindDouble(SqliteStatementHandle stmt, int index, double val);

            /// <summary>
            /// Bind <see cref="string"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <param name="n">Max length of the string value, <paramref name="val"/>.</param>
            /// <param name="callback">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            public static extern SqliteResult BindText(SqliteStatementHandle stmt, int index, [MarshalAs(UnmanagedType.LPWStr), In] string val, int n, IntPtr callback);

            /// <summary>
            /// Bind BLOB data to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="pBlob">Pointer to BLOB data to bind.</param>
            /// <param name="n">Byte lenght of BLOB data, <paramref name="pBlob"/>.</param>
            /// <param name="destructor">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindBlob(SqliteStatementHandle stmt, int index, IntPtr pBlob, int n, IntPtr destructor);

            /// <summary>
            /// Bind BLOB data to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="pBlob">Pointer to BLOB data to bind.</param>
            /// <param name="n">Byte lenght of BLOB data, <paramref name="pBlob"/>.</param>
            /// <param name="destructor">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindBlob64(SqliteStatementHandle stmt, int index, IntPtr pBlob, ulong n, IntPtr destructor);

            /// <summary>
            /// Bind BLOB data of length that is filled with zeros to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_zeroblob", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindZeroBlob(SqliteStatementHandle stmt, int index, int n);

            /// <summary>
            /// Bind BLOB data of length that is filled with zeros to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_zeroblob64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindZeroBlob64(SqliteStatementHandle stmt, int index, ulong n);

            /// <summary>
            /// Bind NULL to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BindNull(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Reset a prepared statement object.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/reset.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_reset", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Reset(SqliteStatementHandle stmt);

            /// <summary>
            /// Get the number of columns in a result set returned by the prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>The number of columns in the result set.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_count.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ColumnCount(SqliteStatementHandle stmt);

            /// <summary>
            /// Get column name in a result set.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column name string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_name.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnName(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get pointer to result value object from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to result value object.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_value", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnValue(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="int"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="int"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ColumnInt(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="long"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="long"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long ColumnInt64(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="double"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="double"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern double ColumnDouble(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as string from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnText(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as BLOB from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column value BLOB.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnBlob(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get size of a BLOB or a UTF-8 TEXT result in bytes.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Size of a BLOB or a UTF-8 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ColumnBlob"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ColumnBytes(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get size of a UTF-16 TEXT result in bytes.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Size of a UTF-16 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ColumnText"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ColumnBytes16(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get type of result value.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Enum value of type of result value.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_type", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteValueType ValueType(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="int"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value as <see cref="int"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_int", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ValueInt(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="long"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value as <see cref="long"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_int64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long ValueInt64(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="double"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value as <see cref="double"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_double", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern double ValueDouble(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as string from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Pointer to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_text16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ValueText(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as BLOB from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Pointer to column value BLOB.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_blob", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ValueBlob(IntPtr pColumnValue);

            /// <summary>
            /// Get size of a BLOB or a UTF-8 TEXT result in bytes.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Size of a BLOB or a UTF-8 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ValueBlob"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_bytes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ValueBytes(IntPtr pColumnValue);

            /// <summary>
            /// Get size of a UTF-16 TEXT result in bytes.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Size of a UTF-16 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ValueText"/></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_value_bytes16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ValueBytes16(IntPtr pColumnValue);

            /// <summary>
            /// Last Insert Rowid.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Last insert ROWID.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/last_insert_rowid.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long LastInsertRowId(SqliteHandle db);

            /// <summary>
            /// Set the Last Insert Rowid value.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="rowId">ROWID to set.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/set_last_insert_rowid.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_set_last_insert_rowid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetLastInsertRowId(SqliteHandle db, long rowId);

            /// <summary>
            /// Count the number of rows modified.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The number of rows modified, inserted or deleted.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/changes.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_changes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Changes(SqliteHandle db);

            /// <summary>
            /// Count the number of rows modified (64-bit signed integer counter).
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The number of rows modified, inserted or deleted.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/changes.html"/></para>
            /// <para>sqlite3_changes64 is available version 3.37.0 or later.</para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_changes64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long Changes64(SqliteHandle db);

            /// <summary>
            /// Count the total number of rows modified since the database connection was opened.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/total_changes.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_total_changes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int TotalChanges(SqliteHandle db);

            /// <summary>
            /// Count the total number of rows modified since the database connection was opened (64-bit signed integer counter).
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/total_changes.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_total_changes64", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long TotalChanges64(SqliteHandle db);

            /// <summary>
            /// Open a BLOB for incremental I/O.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="pDbName">Pointer to UTF-8 byte-sequence of symbolic name of the database.</param>
            /// <param name="pTableName">Pointer to UTF-8 byte-sequence of table name.</param>
            /// <param name="pColumnName">Pointer to UTF-8 byte-sequence of column name.</param>
            /// <param name="rowId">ROWID of BLOB.</param>
            /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
            /// <param name="blobHandle">Output BLOB handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/blob_open.html"/></para>
            /// <para>
            /// This function fails with <see cref="SqliteResult.Error"/> if any of the following are true:
            /// <list type="bullet">
            ///   <item>Database <paramref name="pDbName"/> does not exist</item>
            ///   <item>Table <paramref name="pTableName"/> does not exist within database zDb</item>
            ///   <item>Table <paramref name="pTableName"/> is a WITHOUT ROWID table.</item>
            ///   <item>Column <paramref name="pColumnName"/> does not exist.</item>
            ///   <item>Row <paramref name="rowId"/> is not present in the table.</item>
            ///   <item>The specified column of row <paramref name="rowId"/> contains a value that is not a TEXT or BLOB value.</item>
            ///   <item>Column <paramref name="pColumnName"/> is part of an index, PRIMARY KEY or UNIQUE contraint and the blob is being opened for read/write access</item>
            ///   <item>Foreign key constraints are enabled, column <paramref name="pColumnName"/> is part of a [child key] definition and the blob is being opened for read/write access</item>
            /// </list>
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_open", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BlobOpen(SqliteHandle db, IntPtr pDbName, IntPtr pTableName, IntPtr pColumnName, long rowId, SqliteOpenFlags flags, out SqliteBlobHandle blobHandle);

            /// <summary>
            /// Move a BLOB handle to a new row.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="rowId">New ROWID of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_reopen.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_reopen", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BlobReOpen(SqliteBlobHandle blobHandle, long rowId);

            /// <summary>
            /// Close a BLOB handle.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle to close.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_close.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_close", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BlobClose(IntPtr blobHandle);

            /// <summary>
            /// Return the size of an open BLOB.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <returns>The size of an open BLOB.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_bytes.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_bytes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int BlobBytes(SqliteBlobHandle blobHandle);

            /// <summary>
            /// Read data from a BLOB incrementally.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="pData">Pointer to buffer to read.</param>
            /// <param name="count">Data size to copy in bytes.</param>
            /// <param name="blobOffset">Offset of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_read.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_read", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BlobRead(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset);

            /// <summary>
            /// Write data into a BLOB incrementally.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="pData">Pointer to buffer to write.</param>
            /// <param name="count">Data size to copy in bytes.</param>
            /// <param name="blobOffset">Offset of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_write.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_blob_write", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BlobWrite(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset);

            /// <summary>
            /// Interrupt a long-running query.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/interrupt.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_interrupt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern void Interrupt(SqliteHandle db);

            /// <summary>
            /// Determine whether or not an interrupt is currently in effect for database connection.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>True if an interrupt is currently in effect, or false otherwise.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/interrupt.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_is_interrupted", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool IsInterrupted(SqliteHandle db);

            /// <summary>
            /// Set a busy timeout.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="ms">Sleep time in milliseconds.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/busy_timeout.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BusyTimeout(SqliteHandle db, int ms);

            /// <summary>
            /// Register A Callback To Handle <see cref="SqliteResult.Busy"/> Errors.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">First argument to callback.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/busy_handler.html"/></para>
            /// <para>
            /// The busy callback should not take any actions which modify the database connection that invoked the busy handler.
            /// In other words, the busy handler is not reentrant.
            /// Any such actions result in undefined behavior.
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_busy_handler", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult BusyHandler(SqliteHandle db, BusyCallbackFunc callback, IntPtr callbackArg);

            /// <summary>
            /// Get latest error message occured in SQLite3 functions.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Pointer to latest error message (UTF-16).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetErrorMessage(SqliteHandle db);

            /// <summary>
            /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
            /// </summary>
            /// <param name="result">Result code.</param>
            /// <returns>Pointer to English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_errstr", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetErrorString(SqliteResult result);

            /// <summary>
            /// Get the number of bytes of memory currentry outstanding.
            /// </summary>
            /// <returns>The number of bytes of memory currentry outstanding (malloced but not freed).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/memory_highwater.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_memory_used", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long MemoryUsed();

            /// <summary>
            /// Get the maximum value of <see cref="MemoryUsed"/>.
            /// </summary>
            /// <param name="resetFlag">True to reset high-water mark to the current value of <see cref="MemoryUsed"/>.</param>
            /// <returns>The maximum value of <see cref="MemoryUsed"/> since the high-water mark was last reset.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/memory_highwater.html"/></para>
            /// <para>The value returned by <c>MemoryHighWater(ture) is the high-water mark prior to the reset.</c></para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_memory_highwater", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern long MemoryHighWater(bool resetFlag);
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            /// <summary>
            /// Get Run-Time Library Version Numbers String.
            /// </summary>
            /// <returns>A pointer to the version string constant.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_libversion", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr LibVersionW();

            /// <summary>
            /// Get Run-Time Library Source ID.
            /// </summary>
            /// <returns>A pointer to the source ID constant which contains the date and time of the check-in (UTC) and a SHA1 or SHA3-256 hash of the entire source tree.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_sourceid", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr LibSourceIdW();

            /// <summary>
            /// Get Run-Time Library Version Numbers represents in <see cref="int"/>.
            /// </summary>
            /// <returns>
            /// An integer with a value (X * 1000000 + Y + 1000 + Z)
            /// where X, Y and Z are the components of SQLite version number string, "X.Y.Z".
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/libversion.html"/></para>
            /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_libversion_number", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int LibVersionNumberW();

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">Database filename.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_open16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
            public static extern SqliteResult OpenW([MarshalAs(UnmanagedType.LPWStr), In] string filePath, out SqliteHandle db);

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="pFilePath">Pointer to SQLite3 database file path string (UTF-8).</param>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="flags">Open options.</param>
            /// <param name="pVfs">Pointer to string of name of VFS module to use (UTF-8).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_open_v2", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult OpenV2W(IntPtr pFilePath, out SqliteHandle db, SqliteOpenFlags flags, IntPtr pVfs);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_close", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult CloseW(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="errmsgHandle">Error msg written here (must be release with <see cref="FreeW"/>).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_exec", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult ExecuteW(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmsgHandle);

            /// <summary>
            /// Free memory allocated in SQLite3 functions.
            /// </summary>
            /// <param name="pMemory">Allocated memory pointer.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/free.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_free", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern void FreeW(IntPtr pMemory);

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_prepare_v2", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult PrepareW(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-16).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="sql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_prepare16_v2", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult Prepare16W(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Retrive SQL text.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>A pointer to a copy of the UTF-8 SQL text used to create prepared statement.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_sql", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr SqlW(SqliteStatementHandle stmt);

            /// <summary>
            /// Retrive SQL text with bound parameters expanded.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>
            /// A pointer to a UTF-8 string containing the SQL text of prepared statement with bound parameters expanded.
            /// This pointer MUST BE FREE with <see cref="NativeMethods.FreeW"/>.
            /// </returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_expanded_sql", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteMemoryHandle ExpandedSqlW(SqliteStatementHandle stmt);

            /// <summary>
            /// Retrive normalized SQL text.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>A pointer to a UTF-8 string containing the normalized SQL text of prepared statement.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/expanded_sql.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_normalized_sql", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr NormalizedSqlW(SqliteStatementHandle stmt);

            /// <summary>
            /// Evaluate an SQL statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/step.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_step", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult StepW(SqliteStatementHandle stmt);

            /// <summary>
            /// Destroy a prepared statement object.
            /// </summary>
            /// <param name="pStmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/finalize.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_finalize", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult FinalizeW(IntPtr pStmt);

            /// <summary>
            /// Count number of SQL parameters.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Number of SQL parameters.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_count.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_parameter_count", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int BindParameterCountW(SqliteStatementHandle stmt);

            /// <summary>
            /// Get name of a host parameter.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <returns>Pointer to bind parameter name (UTF-8).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_name.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_parameter_name", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr BindParameterNameW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get index of a parameter with a given name.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pName">Pointer to parameter name (UTF-8).</param>
            /// <returns>Index of the parameter.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_parameter_index.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_parameter_index", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int BindParameterIndexW(SqliteStatementHandle stmt, IntPtr pName);

            /// <summary>
            /// Bind <see cref="int"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_int", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindIntW(SqliteStatementHandle stmt, int index, int val);

            /// <summary>
            /// Bind <see cref="long"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_int64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindInt64W(SqliteStatementHandle stmt, int index, long val);

            /// <summary>
            /// Bind <see cref="double"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_double", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindDoubleW(SqliteStatementHandle stmt, int index, double val);

            /// <summary>
            /// Bind <see cref="string"/> value to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="val">Value to bind.</param>
            /// <param name="n">Max length of the string value, <paramref name="val"/>.</param>
            /// <param name="callback">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_text16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
            public static extern SqliteResult BindTextW(SqliteStatementHandle stmt, int index, [MarshalAs(UnmanagedType.LPWStr), In] string val, int n, IntPtr callback);

            /// <summary>
            /// Bind BLOB data to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="pBlob">Pointer to BLOB data to bind.</param>
            /// <param name="n">Byte lenght of BLOB data, <paramref name="pBlob"/>.</param>
            /// <param name="destructor">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_blob", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindBlobW(SqliteStatementHandle stmt, int index, IntPtr pBlob, int n, IntPtr destructor);

            /// <summary>
            /// Bind BLOB data to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="pBlob">Pointer to BLOB data to bind.</param>
            /// <param name="n">Byte lenght of BLOB data, <paramref name="pBlob"/>.</param>
            /// <param name="destructor">Callback of the special destructor.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_blob64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindBlob64W(SqliteStatementHandle stmt, int index, IntPtr pBlob, ulong n, IntPtr destructor);

            /// <summary>
            /// Bind BLOB data of length that is filled with zeros to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_zeroblob", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindZeroBlobW(SqliteStatementHandle stmt, int index, int n);

            /// <summary>
            /// Bind BLOB data of length that is filled with zeros to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <param name="n">Byte lenght of BLOB data which is filled with zeros.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_zeroblob64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindZeroBlob64W(SqliteStatementHandle stmt, int index, ulong n);

            /// <summary>
            /// Bind NULL to prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of parameter (1-based).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/bind_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_bind_null", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BindNullW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Reset a prepared statement object.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/reset.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_reset", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult ResetW(SqliteStatementHandle stmt);

            /// <summary>
            /// Get the number of columns in a result set returned by the prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>The number of columns in the result set.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_count.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_count", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ColumnCountW(SqliteStatementHandle stmt);

            /// <summary>
            /// Get column name in a result set.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column name string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_name.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_name16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnNameW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get pointer to result value object from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Get pointer to result value object.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_value", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnValueW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="int"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="int"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_int", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ColumnIntW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="long"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="long"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_int64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long ColumnInt64W(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as <see cref="double"/> from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Column value as <see cref="double"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_double", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern double ColumnDoubleW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as string from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_text16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnTextW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get result value as BLOB from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Pointer to column value BLOB.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_blob", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnBlobW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get size of a BLOB or a UTF-8 TEXT result in bytes.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Size of a BLOB or a UTF-8 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ColumnBlob"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_bytes", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ColumnBytesW(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get size of a UTF-16 TEXT result in bytes.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="index">Index of column (0-based).</param>
            /// <returns>Size of a UTF-16 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ColumnText"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_bytes16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ColumnBytes16W(SqliteStatementHandle stmt, int index);

            /// <summary>
            /// Get type of result value.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Enum value of type of result value.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_type", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteValueType ValueTypeW(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="int"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value <see cref="int"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_int", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ValueIntW(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="long"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value as <see cref="long"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_int64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long ValueInt64W(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as <see cref="double"/> from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Column value as <see cref="double"/>.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_double", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern double ValueDoubleW(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as string from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Pointer to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_text16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ValueTextW(IntPtr pColumnValue);

            /// <summary>
            /// Get result value as BLOB from a result value object.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Pointer to column value BLOB.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_blob", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ValueBlobW(IntPtr pColumnValue);

            /// <summary>
            /// Get size of a BLOB or a UTF-8 TEXT result in bytes.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Size of a BLOB or a UTF-8 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ValueBlob"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_bytes", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ValueBytesW(IntPtr pColumnValue);

            /// <summary>
            /// Get size of a UTF-16 TEXT result in bytes.
            /// </summary>
            /// <param name="pColumnValue">Pointer to result value object.</param>
            /// <returns>Size of a UTF-16 TEXT result in bytes.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/column_blob.html"/></para>
            /// <para><seealso cref="ValueText"/></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_value_bytes16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ValueBytes16W(IntPtr pColumnValue);

            /// <summary>
            /// Last Insert Rowid.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Last insert ROWID.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/last_insert_rowid.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_last_insert_rowid", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long LastInsertRowIdW(SqliteHandle db);

            /// <summary>
            /// Set the Last Insert Rowid value.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="rowId">ROWID to set.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/set_last_insert_rowid.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_set_last_insert_rowid", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern void SetLastInsertRowIdW(SqliteHandle db, long rowId);

            /// <summary>
            /// Count the number of rows modified.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The number of rows modified, inserted or deleted.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/changes.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_changes", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int ChangesW(SqliteHandle db);

            /// <summary>
            /// Count the number of rows modified (64-bit signed integer counter).
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The number of rows modified, inserted or deleted.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/changes.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_changes64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long Changes64W(SqliteHandle db);

            /// <summary>
            /// Count the total number of rows modified since the database connection was opened.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/total_changes.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_total_changes", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int TotalChangesW(SqliteHandle db);

            /// <summary>
            /// Count the total number of rows modified since the database connection was opened (64-bit signed integer counter).
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>The total number of rows modified, inserted or deleted since the database connection was opened.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/total_changes.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_total_changes64", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long TotalChanges64W(SqliteHandle db);

            /// <summary>
            /// Open a BLOB for incremental I/O.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="pDbName">Pointer to UTF-8 byte-sequence of symbolic name of the database.</param>
            /// <param name="pTableName">Pointer to UTF-8 byte-sequence of table name.</param>
            /// <param name="pColumnName">Pointer to UTF-8 byte-sequence of column name.</param>
            /// <param name="rowId">ROWID of BLOB.</param>
            /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
            /// <param name="blobHandle">Output BLOB handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/blob_open.html"/></para>
            /// <para>
            /// This function fails with <see cref="SqliteResult.Error"/> if any of the following are true:
            /// <list type="bullet">
            ///   <item>Database <paramref name="pDbName"/> does not exist</item>
            ///   <item>Table <paramref name="pTableName"/> does not exist within database zDb</item>
            ///   <item>Table <paramref name="pTableName"/> is a WITHOUT ROWID table.</item>
            ///   <item>Column <paramref name="pColumnName"/> does not exist.</item>
            ///   <item>Row <paramref name="rowId"/> is not present in the table.</item>
            ///   <item>The specified column of row <paramref name="rowId"/> contains a value that is not a TEXT or BLOB value.</item>
            ///   <item>Column <paramref name="pColumnName"/> is part of an index, PRIMARY KEY or UNIQUE contraint and the blob is being opened for read/write access</item>
            ///   <item>Foreign key constraints are enabled, column <paramref name="pColumnName"/> is part of a [child key] definition and the blob is being opened for read/write access</item>
            /// </list>
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_open", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BlobOpenW(SqliteHandle db, IntPtr pDbName, IntPtr pTableName, IntPtr pColumnName, long rowId, SqliteOpenFlags flags, out SqliteBlobHandle blobHandle);

            /// <summary>
            /// Move a BLOB handle to a new row.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="rowId">New ROWID of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_reopen.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_reopen", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BlobReOpenW(SqliteBlobHandle blobHandle, long rowId);

            /// <summary>
            /// Close a BLOB handle.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle to close.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_close.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_close", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BlobCloseW(IntPtr blobHandle);

            /// <summary>
            /// Return the size of an open BLOB.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <returns>The size of an open BLOB..</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_bytes.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_bytes", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern int BlobBytesW(SqliteBlobHandle blobHandle);

            /// <summary>
            /// Read data from a BLOB incrementally.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="pData">Pointer to buffer to read.</param>
            /// <param name="count">Data size to copy in bytes.</param>
            /// <param name="blobOffset">Offset of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_read.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_read", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BlobReadW(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset);

            /// <summary>
            /// Write data into a BLOB incrementally.
            /// </summary>
            /// <param name="blobHandle">A BLOB handle.</param>
            /// <param name="pData">Pointer to buffer to write.</param>
            /// <param name="count">Data size to copy in bytes.</param>
            /// <param name="blobOffset">Offset of BLOB.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/blob_write.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_blob_write", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BlobWriteW(SqliteBlobHandle blobHandle, IntPtr pData, int count, int blobOffset);

            /// <summary>
            /// Interrupt a long-running query.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/interrupt.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_interrupt", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern void InterruptW(SqliteHandle db);

            /// <summary>
            /// Determine whether or not an interrupt is currently in effect for database connection.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>True if an interrupt is currently in effect, or false otherwise.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/interrupt.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_is_interrupted", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool IsInterruptedW(SqliteHandle db);

            /// <summary>
            /// Set a busy timeout.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="ms">Sleep time in milliseconds.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/busy_timeout.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_busy_timeout", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BusyTimeoutW(SqliteHandle db, int ms);

            /// <summary>
            /// Register A Callback To Handle <see cref="SqliteResult.Busy"/> Errors.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">First argument to callback.</param>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/busy_handler.html"/></para>
            /// <para>
            /// The busy callback should not take any actions which modify the database connection that invoked the busy handler.
            /// In other words, the busy handler is not reentrant.
            /// Any such actions result in undefined behavior.
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_busy_handler", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult BusyHandlerW(SqliteHandle db, BusyCallbackFunc callback, IntPtr callbackArg);

            /// <summary>
            /// Get latest error message occured in SQLite3 functions.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Pointer to latest error message (UTF-16).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_errmsg16", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr GetErrorMessageW(SqliteHandle db);

            /// <summary>
            /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
            /// </summary>
            /// <param name="result">Result code.</param>
            /// <returns>Pointer to English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_errstr", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr GetErrorStringW(SqliteResult result);

            /// <summary>
            /// Get the number of bytes of memory currentry outstanding.
            /// </summary>
            /// <returns>The number of bytes of memory currentry outstanding (malloced but not freed).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/memory_highwater.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_memory_used", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long MemoryUsedW();

            /// <summary>
            /// Get the maximum value of <see cref="MemoryUsed"/>.
            /// </summary>
            /// <param name="resetFlag">True to reset high-water mark to the current value of <see cref="MemoryUsed"/>.</param>
            /// <returns>The maximum value of <see cref="MemoryUsed"/> since the high-water mark was last reset.</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/c3ref/memory_highwater.html"/></para>
            /// <para>The value returned by <c>MemoryHighWater(ture) is the high-water mark prior to the reset.</c></para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_memory_highwater", ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern long MemoryHighWaterW(bool resetFlag);
#endif
        }
    }
}
