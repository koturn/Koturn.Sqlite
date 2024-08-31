namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of pragma_table_info('xxx') or PRAGMA table_info('xxx').
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/pragma.html#pragma_table_info"/>
    /// </remarks>
    public class SqliteTableInfoRow
    {
        /// <summary>
        /// Column ID.
        /// </summary>
        public int Cid { get; private set; }
        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Type name.
        /// </summary>
        public string TypeName { get; private set; }
        /// <summary>
        /// This column has "NOT NULL" contraint or not.
        /// </summary>
        public bool IsNotNull { get; private set; }
        /// <summary>
        /// Default value for this column.
        /// </summary>
        public string DefaultValue { get; private set; }
        /// <summary>
        /// This column is PRIMARY KEY or not.
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="cid">Column ID.</param>
        /// <param name="name">Column name.</param>
        /// <param name="typeName">Type name.</param>
        /// <param name="isNotNull">This column has "NOT NULL" contraint or not.</param>
        /// <param name="defaultValue">Default value for this column.</param>
        /// <param name="isPrimaryKey">This column is PRIMARY KEY or not.</param>
        public SqliteTableInfoRow(int cid, string name, string typeName, bool isNotNull, string defaultValue, bool isPrimaryKey)
        {
            Cid = cid;
            Name = name;
            TypeName = typeName;
            IsNotNull = isNotNull;
            DefaultValue = defaultValue;
            IsPrimaryKey = isPrimaryKey;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            var str = string.Format("{0}: {1} {2}", Cid, Name, TypeName);
            if (IsNotNull)
            {
                str += " NOT NULL";
            }
            if (IsPrimaryKey)
            {
                str += " PRIMARY KEY";
            }
            if (DefaultValue != null)
            {
                str += " DEFAULT " + DefaultValue;
            }
            return str;
        }
    }
}
