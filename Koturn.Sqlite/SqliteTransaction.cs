using System;


namespace Koturn.Sqlite
{
    /// <summary>
    /// SQLite3 transaction object.
    /// </summary>
    public class SqliteTransaction : IDisposable
    {
        /// <summary>
        /// UTF-8 byte sequence of "BEGIN".
        /// </summary>
        private static readonly byte[] BeginUtf8Bytes;
        /// <summary>
        /// UTF-8 byte sequence of "COMMIT".
        /// </summary>
        private static readonly byte[] CommitUtf8Bytes;
        /// <summary>
        /// UTF-8 byte sequence of "ROLLBACK".
        /// </summary>
        private static readonly byte[] RollbackUtf8Bytes;

        /// <summary>
        /// Initialize static members.
        /// </summary>
        static SqliteTransaction()
        {
            BeginUtf8Bytes = new [] { (byte)'B', (byte)'E', (byte)'G', (byte)'I', (byte)'N' };
            CommitUtf8Bytes = new [] { (byte)'C', (byte)'O', (byte)'M', (byte)'M', (byte)'I', (byte)'T' };
            RollbackUtf8Bytes = new [] { (byte)'R', (byte)'O', (byte)'L', (byte)'L', (byte)'B', (byte)'A', (byte)'C', (byte)'K' };
        }


        /// <summary>
        /// A flag property which indicates this instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// SQLite3 connection.
        /// </summary>
        private readonly SqliteConnection _connection;

        /// <summary>
        /// Initialize with specified SQLite connection.
        /// </summary>
        internal SqliteTransaction(SqliteConnection connection)
        {
            connection.ExecuteSingle(BeginUtf8Bytes);
            _connection = connection;
        }

        /// <summary>
        /// Do commut.
        /// </summary>
        public void Commit()
        {
            _connection.ExecuteSingle(CommitUtf8Bytes);
            IsDisposed = true;
        }

        /// <summary>
        /// Do rollback.
        /// </summary>
        public void Rollback()
        {
            _connection.ExecuteSingle(RollbackUtf8Bytes);
            IsDisposed = true;
        }

        /// <summary>
        /// Do rollback to specified save point.
        /// </summary>
        /// <param name="savePoint">Name of save point.</param>
        public void Rollback(string savePoint)
        {
            _connection.ExecuteSingle("ROLLBACK TO SAVEPOINT " + savePoint);
        }

        /// <summary>
        /// Create save point.
        /// </summary>
        /// <param name="savePoint">Name of save point.</param>
        public void Save(string savePoint)
        {
            _connection.ExecuteSingle("SAVEPOINT " + savePoint);
        }

        /// <summary>
        /// Release save point.
        /// </summary>
        /// <param name="savePoint">Name of save point.</param>
        public void Release(string savePoint)
        {
            _connection.ExecuteSingle("RELEASE SAVEPOINT " + savePoint);
        }


        #region IDisposable Support
        /// <summary>
        /// Release resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                Rollback();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Release all resources used by the <see cref="SqliteTransaction"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
