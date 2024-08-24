namespace Koturn.Sqlite
{
    /// <summary>
    /// Type codes of SQLite3.
    /// </summary>
    public enum SqliteObjectType
    {
        /// <summary>
        /// Represents the type of object is "table".
        /// </summary>
        Table,
        /// <summary>
        /// Represents the type of object is "view".
        /// </summary>
        View,
        /// <summary>
        /// Represents the type of object is "shadow" (for shadow tables).
        /// </summary>
        Shadow,
        /// <summary>
        /// Represents the type of object is "virtual" (for virtual tables).
        /// </summary>
        Virtual,
        /// <summary>
        /// Represents the type of object is "index".
        /// </summary>
        Index,
        /// <summary>
        /// Represents the type of object is "trigger".
        /// </summary>
        Trigger
    }
}
