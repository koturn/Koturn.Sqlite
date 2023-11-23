using System;
using System.IO;


namespace Koturn.Sqlite
{
    /// <summary>
    /// Handle for BLOB of SQLite3.
    /// </summary>
    public class SqliteBlobStream : Stream
    {
        /// <summary>
        /// <para>Gets a value indicating whether the current stream supports reading.</para>
        /// <para>Always true.</para>
        /// </summary>
        public override bool CanRead => true;
        /// <summary>
        /// <para>Gets a value indicating whether the current stream supports seeking.</para>
        /// <para>Always true.</para>
        /// </summary>
        public override bool CanSeek => true;
        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite { get; }
        /// <summary>
        /// Gets current opened BLOB size in bytes.
        /// </summary>
        public override long Length => _length;
        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get => _position;
            set
            {
                if (_position < 0 || _position > _length)
                {
                    throw new ArgumentOutOfRangeException("Position", value, "value for Position is out of range");
                }
                _position = value;
            }
        }

        /// <summary>
        /// A BLOB handle.
        /// </summary>
        private readonly SqliteBlobHandle _blobHandle;
        /// <summary>
        /// Current opened BLOB size in bytes.
        /// </summary>
        private long _length;
        /// <summary>
        /// Current read or write position of BLOB.
        /// </summary>
        private long _position;


        /// <summary>
        /// Create Stream for BLOB.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        public SqliteBlobStream(SqliteHandle db, string tableName, string columnName, long rowId, SqliteOpenFlags flags)
            : this(db, "main", tableName, columnName, rowId, flags)
        {
        }

        /// <summary>
        /// Create Stream for BLOB.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <param name="dbName">Symbolic name of the database.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="rowId">ROWID of BLOB.</param>
        /// <param name="flags">A flag which indicates opening for read-only or read and write access.</param>
        public SqliteBlobStream(SqliteHandle db, string dbName, string tableName, string columnName, long rowId, SqliteOpenFlags flags)
        {
            _blobHandle = SqliteLibrary.BlobOpen(db, dbName, tableName, columnName, rowId, flags);
            CanWrite = (flags & SqliteOpenFlags.ReadWrite) != 0x00000000;
            _position = 0;
            _length = SqliteLibrary.BlobBytes(_blobHandle);
        }

        /// <summary>
        /// Move a BLOB handle to a new row.
        /// </summary>
        /// <param name="rowId">New ROWID to open.</param>
        public void ReOpen(long rowId)
        {
            SqliteLibrary.BlobReOpen(_blobHandle, rowId);
            _position = 0;
            _length = SqliteLibrary.BlobBytes(_blobHandle);
        }

        /// <summary>
        /// Reads a sequence of bytes from the BLOB and advances the offset of BLOB by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced by
        /// the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer.
        /// This can be less than the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var readCount = (int)Math.Min(count, _length - _position);
            SqliteLibrary.BlobRead(_blobHandle, buffer, offset, readCount, (int)_position);
            Position += readCount;
            return readCount;
        }

        /// <summary>
        /// Writes a sequence of bytes to the BLOB column and advances the current offset of BLOB by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the BLOB.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin compressing bytes.</param>
        /// <param name="count">The number of bytes to be compressed.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Current stream is readonly");
            }
            SqliteLibrary.BlobWrite(_blobHandle, buffer, offset, count, (int)Position);
            Position += count;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">Seek offset in bytes.</param>
        /// <param name="origin">Seek origin.</param>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = _length - 1 + offset;
                    break;
                default:
                    throw new ArgumentException("origin");
            }

            return Position;
        }

        /// <summary>
        /// This method is not supported, always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported");
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Releases the unmanaged resources used by the System.IO.Stream and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _blobHandle.Dispose();
            base.Dispose(disposing);
        }
    }
}
