using System;
using System.Runtime.InteropServices;
using Koturn.Sqlite.Enums;


namespace Koturn.Sqlite
{
    /// <summary>
    /// Handle for BLOB of SQLite3.
    /// </summary>
    public sealed class SqliteBlobHandle : SafeHandle
    {
        /// <summary>
        /// Initialize BLOB handle with <see cref="IntPtr.Zero"/>.
        /// </summary>
        private SqliteBlobHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// True if the BLOB handle is invalid, otherwise false.
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Close BLOB handle.
        /// </summary>
        /// <returns>True if closing is successful, otherwise false.</returns>
        protected override bool ReleaseHandle()
        {
            return SqliteLibrary.BlobClose(handle) == SqliteResult.OK;
        }
    }
}
