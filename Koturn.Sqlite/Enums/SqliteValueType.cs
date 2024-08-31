namespace Koturn.Sqlite.Enums
{
    /// <summary>
    /// Type codes of SQLite3.
    /// </summary>
    public enum SqliteValueType : int
    {
        /// <summary>
        /// 64-bit signed integer.
        /// </summary>
        Integer = 1,
        /// <summary>
        /// 64-bit IEEE floating point number.
        /// </summary>
        Float = 2,
        /// <summary>
        /// String.
        /// </summary>
        Text = 3,
        /// <summary>
        /// BLOB.
        /// </summary>
        Blob = 4,
        /// <summary>
        /// NULL.
        /// </summary>
        Null = 5
    }
}

