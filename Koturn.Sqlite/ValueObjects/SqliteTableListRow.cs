using Koturn.Sqlite.Enums;


namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of pragma_table_info('xxx') or PRAGMA table_info('xxx').
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/pragma.html#pragma_table_list"/>
    /// </remarks>
    public class SqliteTableListRow
    {
        /// <summary>
        /// The schema in which the table or view appears (for example "main" or "temp").
        /// </summary>
        public string Schema { get; private set; }
        /// <summary>
        /// The name of the table or view.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The type of object.
        /// </summary>
        public SqliteObjectType Type { get; private set; }
        /// <summary>
        /// The number of columns in the table, including generated columns and hidden columns.
        /// </summary>
        public int ColumnCount { get; private set; }
        /// <summary>
        /// true if the table is a WITHOUT ROWID table or false if is not.
        /// </summary>
        public bool IsWithoutRowId { get; private set; }
        /// <summary>
        /// true if the table is a STRICT table or false if it is not.
        /// </summary>
        public bool IsStrict { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="schema">The schema in which the table or view appears (for example "main" or "temp").</param>
        /// <param name="name">The name of the table or view.</param>
        /// <param name="typeName">The type of object - one of "table", "view", "shadow" (for shadow tables), or "virtual" for virtual tables.</param>
        /// <param name="columnCount">The number of columns in the table, including generated columns and hidden columns.</param>
        /// <param name="isWithoutRowId">true if the table is a WITHOUT ROWID table or false if is not.</param>
        /// <param name="isStrict">true if the table is a STRICT table or false if it is not.</param>
        public SqliteTableListRow(string schema, string name, string typeName, int columnCount, bool isWithoutRowId, bool isStrict)
            : this(schema, name, SqliteEnumConverter.ToObjectType(typeName), columnCount, isWithoutRowId, isStrict)
        {
        }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="schema">The schema in which the table or view appears (for example "main" or "temp").</param>
        /// <param name="name">The name of the table or view.</param>
        /// <param name="type">The type of object.</param>
        /// <param name="columnCount">The number of columns in the table, including generated columns and hidden columns.</param>
        /// <param name="isWithoutRowId">true if the table is a WITHOUT ROWID table or false if is not.</param>
        /// <param name="isStrict">true if the table is a STRICT table or false if it is not.</param>
        public SqliteTableListRow(string schema, string name, SqliteObjectType type, int columnCount, bool isWithoutRowId, bool isStrict)
        {
            Schema = schema;
            Name = name;
            Type = type;
            ColumnCount = columnCount;
            IsWithoutRowId = isWithoutRowId;
            IsStrict = isStrict;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            var str = string.Format(
                "{0}.{1} ({2}) ncol={3}",
                Schema,
                Name,
                SqliteEnumConverter.ToObjectTypeName(Type),
                ColumnCount);
            var isNeedComma = false;
            if (IsWithoutRowId)
            {
                str += " WITHOUT ROWID";
                isNeedComma = true;
            }
            if (IsStrict)
            {
                if (isNeedComma)
                {
                    str += ",";
                }
                str += " STRICT";
            }
            return str;
        }
    }
}
