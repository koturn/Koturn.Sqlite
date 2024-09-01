using System;
using System.Collections.Generic;
using Koturn.Sqlite.Enums;
using Koturn.Sqlite.Exceptions;
using Koturn.Sqlite.Handles;


namespace Koturn.Sqlite
{
    /// <summary>
    /// SQLite statement handle wrapper.
    /// </summary>
    public class SqliteStatement : IDisposable, ISqliteColumnAccessable
    {
        /// <summary>
        /// Number of columns.
        /// </summary>
        public int ColumnCount { get; }
        /// <summary>
        /// Number of parameters.
        /// </summary>
        public int ParameterCount { get; }
        /// <summary>
        /// A flag property which indicates this instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// SQLite statement handle.
        /// </summary>
        private readonly SqliteStatementHandle _stmt;
        /// <summary>
        /// Dictionary of pairs of column name and column index.
        /// </summary>
        private Dictionary<string, int> _nameDict;
        /// <summary>
        /// Cache of <see cref="SqliteLibrary.Sql"/>.
        /// </summary>
        private string _sql;


        /// <summary>
        /// SQL text.
        /// </summary>
        public string Sql
        {
            get
            {
                if (_sql == null)
                {
                    _sql = SqliteLibrary.Sql(_stmt);
                }
                return _sql;
            }
        }
        /// <summary>
        /// SQL text with bound parameters expanded.
        /// </summary>
        public string ExpandedSql
        {
            get
            {
                return SqliteLibrary.ExpandedSql(_stmt);
            }
        }
        /// <summary>
        /// Normalized SQL text.
        /// </summary>
        public string NormalizedSql
        {
            get
            {
                return SqliteLibrary.NormalizedSql(_stmt);
            }
        }


        /// <summary>
        /// Create statement handle wrapper with specified statement handle.
        /// </summary>
        public SqliteStatement(SqliteStatementHandle stmt)
        {
            _stmt = stmt;
            _nameDict = null;
            ColumnCount = SqliteLibrary.ColumnCount(stmt);
            ParameterCount = SqliteLibrary.BindParameterCount(stmt);
            IsDisposed = false;
        }

        /// <summary>
        /// Evaluate an SQL statement.
        /// </summary>
        /// <returns>True if obtained records remain, otherwise false.</returns>
        public bool Step()
        {
            return SqliteLibrary.Step(_stmt);
        }

        /// <summary>
        /// Bind <see cref="int"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, int val)
        {
            SqliteLibrary.Bind(_stmt, index, val);
        }

        /// <summary>
        /// Bind <see cref="int"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, int? val)
        {
            if (val.HasValue)
            {
                SqliteLibrary.Bind(_stmt, index, val.Value);
            }
            else
            {
                BindNull(index);
            }
        }

        /// <summary>
        /// Bind <see cref="int"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, int val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="int"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, int? val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="long"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, long val)
        {
            SqliteLibrary.Bind(_stmt, index, val);
        }

        /// <summary>
        /// Bind <see cref="long"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, long? val)
        {
            if (val.HasValue)
            {
                SqliteLibrary.Bind(_stmt, index, val.Value);
            }
            else
            {
                BindNull(index);
            }
        }

        /// <summary>
        /// Bind <see cref="long"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, long val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="long"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, long? val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="double"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, double val)
        {
            SqliteLibrary.Bind(_stmt, index, val);
        }

        /// <summary>
        /// Bind <see cref="double"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, double? val)
        {
            if (val.HasValue)
            {
                SqliteLibrary.Bind(_stmt, index, val.Value);
            }
            else
            {
                BindNull(index);
            }
        }

        /// <summary>
        /// Bind <see cref="double"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, double val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="double"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, double? val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind <see cref="string"/> value to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(int index, string val)
        {
            if (val == null)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, val);
            }
        }

        /// <summary>
        /// Bind <see cref="string"/> value to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="val">Value to bind.</param>
        public void Bind(string name, string val)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), val);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        public void Bind(int index, byte[] blob)
        {
            if (blob == null)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, blob);
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="blob">BLOB data to bind.</param>
        public void Bind(string name, byte[] blob)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), blob);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public void Bind(int index, byte[] blob, int offset, int length)
        {
            if (blob == null)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, blob, offset, length);
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public void Bind(string name, byte[] blob, int offset, int length)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), blob, offset, length);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public void Bind(int index, byte[] blob, ulong offset, ulong length)
        {
            if (blob == null)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, blob, offset, length);
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="blob">BLOB data to bind.</param>
        /// <param name="offset">Offset of BLOB data.</param>
        /// <param name="length">Length of BLOB data.</param>
        public void Bind(string name, byte[] blob, ulong offset, ulong length)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), blob, offset, length);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public void Bind(int index, IntPtr pBlob, int length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            if (pBlob == IntPtr.Zero)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, pBlob, length, dtorType);
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public void Bind(string name, IntPtr pBlob, int length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), pBlob, length, dtorType);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public void Bind(int index, IntPtr pBlob, ulong length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            if (pBlob == IntPtr.Zero)
            {
                BindNull(index);
            }
            else
            {
                SqliteLibrary.Bind(_stmt, index, pBlob, length, dtorType);
            }
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="pBlob">Pointer to BLOB data to bind.</param>
        /// <param name="length">Length of BLOB data.</param>
        /// <param name="dtorType">Special destructor for BLOB data.</param>
        public void Bind(string name, IntPtr pBlob, ulong length, SqliteDestructorType dtorType = SqliteDestructorType.Transient)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), pBlob, length, dtorType);
        }

        /// <summary>
        /// Bind NULL to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="_">NULL value (not used).</param>
        public void Bind(int index, DBNull _)
        {
            BindNull(index);
        }

        /// <summary>
        /// Bind NULL to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="_">NULL value (not used).</param>
        public void Bind(string name, DBNull _)
        {
            BindNull(SqliteLibrary.BindParameterIndex(_stmt, name));
        }

        /// <summary>
        /// Bind any data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="obj">An object to bind.</param>
        public void Bind(int index, object obj)
        {
            var t = obj.GetType();
            if (t == typeof(short))
            {
                Bind(index, (int)(short)obj);
            }
            else if (t == typeof(ushort))
            {
                Bind(index, (int)(ushort)obj);
            }
            else if (t == typeof(int))
            {
                Bind(index, (int)obj);
            }
            else if (t == typeof(uint))
            {
                Bind(index, (long)(uint)obj);
            }
            else if (t == typeof(long))
            {
                Bind(index, (long)obj);
            }
            else if (t == typeof(ulong))
            {
                Bind(index, (long)obj);
            }
            else if (t == typeof(float))
            {
                Bind(index, (double)(float)obj);
            }
            else if (t == typeof(double))
            {
                Bind(index, (double)obj);
            }
            else if (t == typeof(byte[]))
            {
                Bind(index, (byte[])obj);
            }
            else if (t == typeof(string))
            {
                Bind(index, (string)obj);
            }
            else
            {
                throw new NotSupportedException("Not supported type: " + t.Name);
            }
        }

        /// <summary>
        /// Bind any data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="obj">An object to bind.</param>
        public void Bind(string name, object obj)
        {
            Bind(SqliteLibrary.BindParameterIndex(_stmt, name), obj);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="length">Byte lenght of BLOB data which is filled with zeros.</param>
        public void BindZeroBlob(int index, int length)
        {
            SqliteLibrary.BindZeroBlob(_stmt, index, length);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="length">Byte lenght of BLOB data which is filled with zeros.</param>
        public void BindZeroBlob(string name, int length)
        {
            BindZeroBlob(SqliteLibrary.BindParameterIndex(_stmt, name), length);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        /// <param name="length">Byte lenght of BLOB data which is filled with zeros.</param>
        public void BindZeroBlob(int index, ulong length)
        {
            SqliteLibrary.BindZeroBlob(_stmt, index, length);
        }

        /// <summary>
        /// Bind BLOB data to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="length">Byte lenght of BLOB data which is filled with zeros.</param>
        public void BindZeroBlob(string name, ulong length)
        {
            BindZeroBlob(SqliteLibrary.BindParameterIndex(_stmt, name), length);
        }

        /// <summary>
        /// Bind NULL to prepared statement.
        /// </summary>
        /// <param name="index">Index of parameter (1-based).</param>
        public void BindNull(int index)
        {
            SqliteLibrary.BindNull(_stmt, index);
        }

        /// <summary>
        /// Bind NULL to prepared statement.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        public void BindNull(string name)
        {
            BindNull(SqliteLibrary.BindParameterIndex(_stmt, name));
        }

        /// <summary>
        /// Reset a prepared statement object.
        /// </summary>
        public void Reset()
        {
            SqliteLibrary.Reset(_stmt);
        }

        /// <summary>
        /// Get result object from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Result object.</returns>
        public object Get(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Integer:
                    return SqliteLibrary.ValueInt64(pColumnValue);
                case SqliteValueType.Float:
                    return SqliteLibrary.ValueDouble(pColumnValue);
                case SqliteValueType.Text:
                    return SqliteLibrary.ValueText(pColumnValue);
                case SqliteValueType.Blob:
                    return SqliteLibrary.ValueBlob(pColumnValue);
                case SqliteValueType.Null:
                    return DBNull.Value;
                default:
                    ThrowArgumentOutOfRangeException("valueType", valueType, "Unrecognized value type");
                    return null;
            }
        }

        /// <summary>
        /// Get result object from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Result object.</returns>
        public object Get(string name)
        {
            return Get(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result object as specified type from a query.
        /// </summary>
        /// <typeparam name="T">Result object type.</typeparam>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Result object.</returns>
        /// <exception cref="NotSupportedException">Thrown when unsupported type is specified.</exception>
        public object Get<T>(int index)
        {
            var t = typeof(T);
            return t == typeof(short) ? (short)GetNullableInt(index)
                : t == typeof(ushort) ? (ushort)GetNullableInt(index)
                : t == typeof(int) ? GetNullableInt(index)
                : t == typeof(uint) ? (uint)GetNullableInt(index)
                : t == typeof(long) ? GetNullableInt64(index)
                : t == typeof(ulong) ? (ulong)GetNullableInt64(index)
                : t == typeof(float) ? (float)GetNullableDouble(index)
                : t == typeof(double) ? GetNullableDouble(index)
                : t == typeof(string) ? GetText(index)
                : t == typeof(byte[]) ? (object)GetBlob(index)
                : throw new NotSupportedException("Not supported type: " + t.Name);
        }

        /// <summary>
        /// Get result object as specified type from a query.
        /// </summary>
        /// <typeparam name="T">Result object type.</typeparam>
        /// <param name="name">Name of column.</param>
        /// <returns>Result object.</returns>
        public object Get<T>(string name)
        {
            return Get<T>(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntUnchecked(int index)
        {
            return SqliteLibrary.ColumnInt(_stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetInt(int index)
        {
            CheckColumnIndex("index", index);
            return GetIntUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetInt(string name)
        {
            return GetIntUnchecked(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableIntUnchecked(int index)
        {
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            return SqliteLibrary.ValueType(pColumnValue) == SqliteValueType.Null ? null : (int?)SqliteLibrary.ValueInt(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableInt(int index)
        {
            CheckColumnIndex("index", index);
            return GetNullableIntUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableInt(string name)
        {
            return GetNullableInt(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            if (valueType != SqliteValueType.Integer)
            {
                SqliteTypeException.Throw(SqliteValueType.Integer, valueType, index);
            }
            return SqliteLibrary.ValueInt(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntStrict(string name)
        {
            return GetIntStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int? GetNullableIntStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Integer:
                    return SqliteLibrary.ValueInt(pColumnValue);
                case SqliteValueType.Null:
                    return null;
                default:
                    SqliteTypeException.Throw(SqliteValueType.Integer, valueType, index);
                    return null;
            }
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int? GetNullableIntStrict(string name)
        {
            return GetNullableIntStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Unchecked(int index)
        {
            return SqliteLibrary.ColumnInt64(_stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64(int index)
        {
            CheckColumnIndex("index", index);
            return GetInt64Unchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64(string name)
        {
            return GetInt64(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64Unchecked(int index)
        {
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            return SqliteLibrary.ValueType(pColumnValue) == SqliteValueType.Null ? null : (int?)SqliteLibrary.ValueInt64(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64(int index)
        {
            CheckColumnIndex("index", index);
            return GetNullableInt64Unchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64(string name)
        {
            return GetNullableInt64(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Strict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            if (valueType != SqliteValueType.Integer)
            {
                SqliteTypeException.Throw(SqliteValueType.Integer, valueType, index);
            }
            return SqliteLibrary.ValueInt64(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Strict(string name)
        {
            return GetInt64Strict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long? GetNullableInt64Strict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Integer:
                    return SqliteLibrary.ValueInt64(pColumnValue);
                case SqliteValueType.Null:
                    return null;
                default:
                    SqliteTypeException.Throw(SqliteValueType.Integer, valueType, index);
                    return null;
            }
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long? GetNullableInt64Strict(string name)
        {
            return GetNullableInt64Strict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleUnchecked(int index)
        {
            return SqliteLibrary.ColumnDouble(_stmt, index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDouble(int index)
        {
            CheckColumnIndex("index", index);
            return GetDoubleUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDouble(string name)
        {
            return GetDouble(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDoubleUnchecked(int index)
        {
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            return SqliteLibrary.ValueType(pColumnValue) == SqliteValueType.Null ? null : (int?)SqliteLibrary.ValueDouble(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDouble(int index)
        {
            CheckColumnIndex("index", index);
            return GetNullableDoubleUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDouble(string name)
        {
            return GetNullableDouble(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            if (valueType != SqliteValueType.Float)
            {
                SqliteTypeException.Throw(SqliteValueType.Float, valueType, index);
            }
            return SqliteLibrary.ValueDouble(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleStrict(string name)
        {
            return GetDoubleStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double? GetNullableDoubleStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Float:
                    return SqliteLibrary.ValueDouble(pColumnValue);
                case SqliteValueType.Null:
                    return null;
                default:
                    SqliteTypeException.Throw(SqliteValueType.Float, valueType, index);
                    return null;
            }
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double? GetNullableDoubleStrict(string name)
        {
            return GetNullableDoubleStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetTextUnchecked(int index)
        {
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            return SqliteLibrary.ValueType(pColumnValue) == SqliteValueType.Null ? null : SqliteLibrary.ValueText(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetText(int index)
        {
            CheckColumnIndex("index", index);
            return GetTextUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="name">Name of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetText(string name)
        {
            return GetText(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetTextStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            if (valueType != SqliteValueType.Text)
            {
                SqliteTypeException.Throw(SqliteValueType.Text, valueType, index);
            }
            return SqliteLibrary.ValueText(pColumnValue);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetTextStrict(string name)
        {
            return GetTextStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetNullableTextStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Text:
                    return SqliteLibrary.ValueText(pColumnValue);
                case SqliteValueType.Null:
                    return null;
                default:
                    SqliteTypeException.Throw(SqliteValueType.Text, valueType, index);
                    return null;
            }
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetNullableTextStrict(string name)
        {
            return GetNullableTextStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlobUnchecked(int index)
        {
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            return SqliteLibrary.ValueType(pColumnValue) == SqliteValueType.Null ? null : SqliteLibrary.ValueBlob(pColumnValue);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlob(int index)
        {
            CheckColumnIndex("index", index);
            return GetBlobUnchecked(index);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlob(string name)
        {
            return GetBlob(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetBlobStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            if (valueType != SqliteValueType.Text)
            {
                SqliteTypeException.Throw(SqliteValueType.Blob, valueType, index);
            }
            return SqliteLibrary.ValueBlob(pColumnValue);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetBlobStrict(string name)
        {
            return GetBlobStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetNullableBlobStrict(int index)
        {
            CheckColumnIndex("index", index);
            var pColumnValue = SqliteLibrary.ColumnValue(_stmt, index);
            var valueType = SqliteLibrary.ValueType(pColumnValue);
            switch (valueType)
            {
                case SqliteValueType.Blob:
                    return SqliteLibrary.ValueBlob(pColumnValue);
                case SqliteValueType.Null:
                    return null;
                default:
                    SqliteTypeException.Throw(SqliteValueType.Blob, valueType, index);
                    return null;
            }
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetNullableBlobStrict(string name)
        {
            return GetNullableBlobStrict(GetNameDict()[name]);
        }

        /// <summary>
        /// Get bind parameter index of specified parameter name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Bind parameter index (1-based).</returns>
        public int ParameterIndexOf(string name)
        {
            return SqliteLibrary.BindParameterIndex(_stmt, name);
        }

        /// <summary>
        /// Get bind parameter name of specified index.
        /// </summary>
        /// <param name="index">Parameter index (1-based).</param>
        /// <returns>Bind parameter index.</returns>
        public string ParameterNameAt(int index)
        {
            return SqliteLibrary.BindParameterName(_stmt, index);
        }


        #region IDisposable Support
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
                _stmt.Dispose();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Release all resources used by the <see cref="SqliteStatement"/> object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            // TODO: Add following code when finalizer is implemented.
            // GC.SuppressFinalize(this);
        }
        #endregion


#if false
        private SqliteValueType[] GetTypeArray()
        {
            if (_typeArray == null)
            {
                return _typeArray = BuildTypeArray();
            }
            return _typeArray;
        }

        private SqliteValueType[] BuildTypeArray()
        {
            var typeArray = new SqliteValueType[_columnCount];
            for (int i = 0; i < typeArray.Length; i++)
            {
                typeArray[i] = SqliteUtil.ColumnType(_stmt, i);
            }

            return typeArray;
        }
#endif


        private Dictionary<string, int> GetNameDict()
        {
            if (_nameDict == null)
            {
                return _nameDict = BuildNameDict();
            }
            return _nameDict;
        }

        private Dictionary<string, int> BuildNameDict()
        {
            var columnCount = ColumnCount;
            var nameDict = new Dictionary<string, int>(columnCount);
            for (int i = 0; i < columnCount; i++)
            {
                nameDict[SqliteLibrary.ColumnName(_stmt, i)] = i;
            }

            return nameDict;
        }

        /// <summary>
        /// Check index of column.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="index">Index of column.</param>
        private void CheckColumnIndex(string paramName, int index)
        {
            if (index < 0 || index >= ColumnCount)
            {
                ThrowArgumentOutOfRangeException(paramName, index, "Column index is out of range");
            }
        }


        /// <summary>
        /// Throw <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="actualValue">Actual value.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentOutOfRangeException">Always thrown.</exception>
        private static void ThrowArgumentOutOfRangeException<T>(string paramName, T actualValue, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }
    }
}
