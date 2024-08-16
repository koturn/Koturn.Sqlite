namespace Koturn.Sqlite
{
    public interface ISqliteColumnAccessable
    {
        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int GetIntUnchecked(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int GetInt(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int GetInt(string name);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        int? GetNullableIntUnchecked(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        int? GetNullableInt(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/> if column value is not NULL, otherwise null.</returns>
        int? GetNullableInt(string name);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int GetIntStrict(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int GetIntStrict(string name);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int? GetNullableIntStrict(int index);

        /// <summary>
        /// Get result value as <see cref="int"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="int"/>.</returns>
        int? GetNullableIntStrict(string name);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long GetInt64Unchecked(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long GetInt64(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long GetInt64(string name);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        long? GetNullableInt64Unchecked(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        long? GetNullableInt64(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/> if column value is not NULL, otherwise null.</returns>
        long? GetNullableInt64(string name);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double GetDoubleUnchecked(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long GetInt64Strict(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long GetInt64Strict(string name);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long? GetNullableInt64Strict(int index);

        /// <summary>
        /// Get result value as <see cref="long"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="long"/>.</returns>
        long? GetNullableInt64Strict(string name);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double GetDouble(int index);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double GetDouble(string name);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        double? GetNullableDoubleUnchecked(int index);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        double? GetNullableDouble(int index);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/> if column value is not NULL, otherwise null.</returns>
        double? GetNullableDouble(string name);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double GetDoubleStrict(int index);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double GetDoubleStrict(string name);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double? GetNullableDoubleStrict(int index);

        /// <summary>
        /// Get result value as <see cref="double"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="double"/>.</returns>
        double? GetNullableDoubleStrict(string name);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        string GetTextUnchecked(int index);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        string GetText(int index);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query.
        /// </summary>
        /// <param name="name">Name of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/> if column value is not NULL, otherwise null.</returns>
        string GetText(string name);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        string GetTextStrict(int index);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        string GetTextStrict(string name);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        string GetNullableTextStrict(int index);

        /// <summary>
        /// Get result value as <see cref="string"/> from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="string"/>.</returns>
        string GetNullableTextStrict(string name);

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based; value range is not checked).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        byte[] GetBlobUnchecked(int index);

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        byte[] GetBlob(int index);

        /// <summary>
        /// Get result value as BLOB from a query.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB if column value is not NULL or has non-zero length, otherwise null.</returns>
        byte[] GetBlob(string name);

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        byte[] GetBlobStrict(int index);

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        byte[] GetBlobStrict(string name);

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="index">Index of column (0-based).</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        byte[] GetNullableBlobStrict(int index);

        /// <summary>
        /// Get result value as BLOB from a query with strict type checking.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Column value as <see cref="byte"/> array of BLOB.</returns>
        byte[] GetNullableBlobStrict(string name);
    }
}
