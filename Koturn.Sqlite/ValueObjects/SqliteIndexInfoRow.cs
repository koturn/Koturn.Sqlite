namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of pragma_index_info('xxx') or PRAGMA index_info('xxx').
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/pragma.html#pragma_index_info"/>
    /// </remarks>
    public class SqliteIndexInfoRow
    {
        /// <summary>
        /// The rank of the column within the index. (0 means left-most.)
        /// </summary>
        public int SeqNo { get; private set; }
        /// <summary>
        /// The rank of the column within the table being indexed.
        /// A value of -1 means rowid and a value of -2 means that an expression is being used.
        /// </summary>
        public int Cid { get; private set; }
        /// <summary>
        /// The name of the column being indexed. This columns is NULL if the column is the rowid or an expression.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="seqNo">The rank of the column within the index. (0 means left-most.).</param>
        /// <param name="cid">The rank of the column within the table being indexed.</param>
        /// <param name="name">The name of the column being indexed.</param>
        public SqliteIndexInfoRow(int seqNo, int cid, string name)
        {
            SeqNo = seqNo;
            Cid = cid;
            Name = name;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            return string.Format("seqno={0} cid={1} name={2}", Cid, Name, Name);
        }
    }
}
