namespace Koturn.Sqlite
{
    /// <summary>
    /// Result row of pragma_index_xinfo('xxx') or PRAGMA index_xinfo('xxx').
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/pragma.html#pragma_index_xinfo"/>
    /// </remarks>
    public class SqliteIndexXInfoRow : SqliteIndexInfoRow
    {
        /// <summary>
        /// 1 if the index-column is sorted in reverse (DESC) order by the index and 0 otherwise.
        /// </summary>
        public int Desc { get; private set; }
        /// <summary>
        /// The name for the collating sequence used to compare values in the index-column.
        /// </summary>
        public string Coll { get; private set; }
        /// <summary>
        /// 1 if the index-column is a key column and 0 if the index-column is an auxiliary column.
        /// </summary>
        public int Key { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="seqNo">Sequence number.</param>
        /// <param name="cid">Column ID.</param>
        /// <param name="name">Column name.</param>
        /// <param name="desc">1 if the index-column is sorted in reverse (DESC) order by the index and 0 otherwise.</param>
        /// <param name="coll">The name for the collating sequence used to compare values in the index-column.</param>
        /// <param name="key">1 if the index-column is a key column and 0 if the index-column is an auxiliary column.</param>
        public SqliteIndexXInfoRow(int seqNo, int cid, string name, int desc, string coll, int key)
            : base(seqNo, cid, name)
        {
            Desc = desc;
            Coll = coll;
            Key = key;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            return string.Format("seqno={0} cid={1} name={2} desc={3} Coll={4} key={5}", Cid, Name, Name, Desc, Coll, Key);
        }
    }
}
