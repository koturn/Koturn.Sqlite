using Koturn.Sqlite.Enums;


namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of sqlite_schema.
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/schematab.html"/>
    /// </remarks>
    public class SqliteSchemaRow
    {
        /// <summary>
        /// One of the following value: <see cref="SqliteObjectType.Table"/>, <see cref="SqliteObjectType.Index"/>,
        /// <see cref="SqliteObjectType.View"/> or <see cref="SqliteObjectType.Trigger"/>.
        /// </summary>
        public SqliteObjectType Type { get; private set; }
        /// <summary>
        /// Name of the object.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of a table or view that the object is associated with.
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// Page number of the root b-tree page for tables and indexes.
        /// </summary>
        public int RootPage { get; private set; }
        /// <summary>
        /// SQL text that describes the object.
        /// </summary>
        public string Sql { get; private set; }


        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="type">Type name of the object.</param>
        /// <param name="name">Name of the object.</param>
        /// <param name="tableName">Name of a table or view that the object is associated with.</param>
        /// <param name="rootPage">Page number of the root b-tree page for tables and indexes.</param>
        /// <param name="sql">SQL text that describes the object.</param>
        public SqliteSchemaRow(string typeName, string name, string tableName, int rootPage, string sql)
            : this(SqliteEnumConverter.ToObjectType(typeName), name, tableName, rootPage, sql)
        {
        }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <param name="name">Name of the object.</param>
        /// <param name="tableName">Name of a table or view that the object is associated with.</param>
        /// <param name="rootPage">Page number of the root b-tree page for tables and indexes.</param>
        /// <param name="sql">SQL text that describes the object.</param>
        public SqliteSchemaRow(SqliteObjectType type, string name, string tableName, int rootPage, string sql)
        {
            Type = type;
            Name = name;
            TableName = tableName;
            RootPage = rootPage;
            Sql = sql;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                "type={0}, name={1}, tbl_name={2}, rootpage={3}, sql={4}",
                SqliteEnumConverter.ToObjectTypeName(Type),
                Name,
                TableName,
                RootPage,
                Sql);
        }
    }
}
