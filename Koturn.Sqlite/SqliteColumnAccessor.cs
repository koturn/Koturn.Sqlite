namespace Koturn.Sqlite
{
    /// <summary>
    /// Wrapper of <see cref="SqliteStatement"/> to get column values.
    /// </summary>
    public class SqliteColumnAccessor
    {
        /// <summary>
        /// Statement handle.
        /// </summary>
        private readonly SqliteStatement _stmt;

        /// <summary>
        /// Create accessor with specified statement handle.
        /// </summary>
        public SqliteColumnAccessor(SqliteStatement stmt)
        {
            _stmt = stmt;
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntUnchecked(int index)
        {
            return _stmt.GetIntUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetInt(int index)
        {
            return _stmt.GetInt(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetInt(string name)
        {
            return _stmt.GetInt(name);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableIntUnchecked(int index)
        {
            return _stmt.GetNullableIntUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableInt(int index)
        {
            return _stmt.GetNullableInt(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        public int? GetNullableInt(string name)
        {
            return _stmt.GetNullableInt(name);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntStrict(int index)
        {
            return _stmt.GetIntStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int GetIntStrict(string name)
        {
            return _stmt.GetIntStrict(name);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int? GetNullableIntStrict(int index)
        {
            return _stmt.GetNullableIntStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        public int? GetNullableIntStrict(string name)
        {
            return _stmt.GetNullableIntStrict(name);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Unchecked(int index)
        {
            return _stmt.GetInt64Unchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64(int index)
        {
            return _stmt.GetInt64(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64(string name)
        {
            return _stmt.GetInt64(name);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64Unchecked(int index)
        {
            return _stmt.GetNullableInt64Unchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64(int index)
        {
            return _stmt.GetNullableInt64(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        public long? GetNullableInt64(string name)
        {
            return _stmt.GetNullableInt64(name);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleUnchecked(int index)
        {
            return _stmt.GetDoubleUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Strict(int index)
        {
            return _stmt.GetInt64Strict(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long GetInt64Strict(string name)
        {
            return _stmt.GetInt64Strict(name);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long? GetNullableInt64Strict(int index)
        {
            return _stmt.GetNullableInt64Strict(index);
        }

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        public long? GetNullableInt64Strict(string name)
        {
            return _stmt.GetNullableInt64Strict(name);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDouble(int index)
        {
            return _stmt.GetDouble(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDouble(string name)
        {
            return _stmt.GetDouble(name);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDoubleUnchecked(int index)
        {
            return _stmt.GetNullableDoubleUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDouble(int index)
        {
            return _stmt.GetNullableDouble(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        public double? GetNullableDouble(string name)
        {
            return _stmt.GetNullableDouble(name);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleStrict(int index)
        {
            return _stmt.GetDoubleStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double GetDoubleStrict(string name)
        {
            return _stmt.GetDoubleStrict(name);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double? GetNullableDoubleStrict(int index)
        {
            return _stmt.GetNullableDoubleStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        public double? GetNullableDoubleStrict(string name)
        {
            return _stmt.GetNullableDoubleStrict(name);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetTextUnchecked(int index)
        {
            return _stmt.GetTextUnchecked(index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetText(int index)
        {
            return _stmt.GetText(index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="name">Name of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        public string GetText(string name)
        {
            return _stmt.GetText(name);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetTextStrict(int index)
        {
            return _stmt.GetTextStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetTextStrict(string name)
        {
            return _stmt.GetTextStrict(name);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetNullableTextStrict(int index)
        {
            return _stmt.GetNullableTextStrict(index);
        }

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        public string GetNullableTextStrict(string name)
        {
            return _stmt.GetNullableTextStrict(name);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlobUnchecked(int index)
        {
            return _stmt.GetBlobUnchecked(index);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlob(int index)
        {
            return _stmt.GetBlob(index);
        }

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        public byte[] GetBlob(string name)
        {
            return _stmt.GetBlob(name);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetBlobStrict(int index)
        {
            return _stmt.GetBlobStrict(index);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetBlobStrict(string name)
        {
            return _stmt.GetBlobStrict(name);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetNullableBlobStrict(int index)
        {
            return _stmt.GetNullableBlobStrict(index);
        }

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        public byte[] GetNullableBlobStrict(string name)
        {
            return _stmt.GetNullableBlobStrict(name);
        }
    }
}
