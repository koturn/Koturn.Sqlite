namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of pragma_table_xinfo('xxx') or PRAGMA table_xinfo('xxx').
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/pragma.html#pragma_table_xinfo"/>
    /// </remarks>
    public class SqliteTableXInfoRow : SqliteTableInfoRow
    {
        /// <summary>
        /// A normal column (0), a dynamic or stored generated column (2 or 3), or a hidden column in a virtual table (1).
        /// </summary>
        public int Hidden { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="cid">Column ID.</param>
        /// <param name="name">Column name.</param>
        /// <param name="typeName">Type name.</param>
        /// <param name="isNotNull">This column has "NOT NULL" contraint or not.</param>
        /// <param name="defaultValue">Default value for this column.</param>
        /// <param name="isPrimaryKey">This column is PRIMARY KEY or not.</param>
        /// <param name="hidden">A normal column (0), a dynamic or stored generated column (2 or 3), or a hidden column in a virtual table (1).</param>
        public SqliteTableXInfoRow(int cid, string name, string typeName, bool isNotNull, string defaultValue, bool isPrimaryKey, int hidden)
            : base(cid, name, typeName, isNotNull, defaultValue, isPrimaryKey)
        {
            Hidden = hidden;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            var str = base.ToString();
            switch (Hidden)
            {
                case 0:
                    str += " /* Normal */";
                    break;
                case 1:
                    str += " /* Hidden */";
                    break;
                case 2:
                    str += " /* Dynamic */";
                    break;
                case 3:
                    str += " /* Stored Generated */";
                    break;
            }
            return str;
        }
    }
}
