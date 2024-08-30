namespace Koturn.Sqlite
{
    /// <summary>
    /// SQLite3 text encoding values.
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/c3ref/c_any.html"/>
    /// </remarks>
    public enum SqliteEncodingValues : uint
    {
        /// <summary>
        /// Means "UTF-8".
        /// </summary>
        Utf8 = 1,
        /// <summary>
        /// Means "UTF-16LE".
        /// </summary>
        Utf16LittleEndian = 1,
        /// <summary>
        /// Means "UTF-16BE".
        /// </summary>
        Utf16BigEndian = 2,
        /// <summary>
        /// Means "UTF-16" with native byte order.
        /// </summary>
        Utf16 = 3,
        /// <summary>
        /// Deprecated.
        /// </summary>
        Any = 5,
        /// <summary>
        /// sqlite3_create_collation only.
        /// </summary>
        Utf16Aligned =8
    }
}
