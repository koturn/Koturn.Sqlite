namespace Koturn.Sqlite
{
    /// <summary>
    /// Type codes of SQLite3.
    /// </summary>
    public enum SqliteSchemaType
    {
        /// <summary>
        /// 64-bit signed integer.
        /// </summary>
        Table,
        /// <summary>
        /// 64-bit IEEE floating point number.
        /// </summary>
        Index,
        /// <summary>
        /// String.
        /// </summary>
        View,
        /// <summary>
        /// BLOB.
        /// </summary>
        Trigger
    }
}
