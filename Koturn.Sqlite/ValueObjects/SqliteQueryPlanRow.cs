namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of "EXPLAIN QUERY PLAN".
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/eqp.html"/>
    /// </remarks>
    public class SqliteQueryPlanRow
    {
        /// <summary>
        /// Node ID.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Parent node ID.
        /// </summary>
        public int Parent { get; private set; }
        /// <summary>
        /// Value of "notused".
        /// </summary>
        public int NotUsed { get; private set; }
        /// <summary>
        /// Detail.
        /// </summary>
        public string Detail { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="id">Node ID.</param>
        /// <param name="parent">Parent node ID.</param>
        /// <param name="notUsed">Value of "notused".</param>
        /// <param name="detail">Detail.</param>
        public SqliteQueryPlanRow(int id, int parent, int notUsed, string detail)
        {
            Id = id;
            Parent = parent;
            NotUsed = notUsed;
            Detail = detail;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Parent={1} [{2}]: {3}", Id, Parent, NotUsed, Detail);
        }
    }
}

