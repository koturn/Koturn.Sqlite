using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace Koturn.Sqlite
{
    /// <summary>
    /// The first 100 bytes of the database file comprise the database file header.
    /// </summary>
    /// <remarks>
    /// <seealso href="https://www.sqlite.org/fileformat.html#the_database_header"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class SqliteFileHeader
    {
        /// <summary>
        /// The header string: "SQLite format 3\000".
        /// </summary>
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string HeaderString { get; }
        /// <summary>
        /// The database page size in bytes.
        /// Must be a power of two between 512 and 32768 inclusive, or the value 1 representing a page size of 65536.
        /// </summary>
        public ushort PageSize { get; set; }
        /// <summary>
        /// File format write version. 1 for legacy; 2 for WAL.
        /// </summary>
        /// <remarks>
        /// <para><seealso href="https://www.sqlite.org/wal.html"/></para>
        /// </remarks>
        public byte WriteVersion { get; set; }
        /// <summary>
        /// File format read version. 1 for legacy; 2 for WAL.
        /// </summary>
        /// <remarks>
        /// <para><seealso href="https://www.sqlite.org/wal.html"/></para>
        /// </remarks>
        public byte ReadVersion { get; set; }
        /// <summary>
        /// Bytes of unused "reserved" space at the end of each page. Usually 0.
        /// </summary>
        public byte UnusedSpaceBytes { get; set; }
        /// <summary>
        /// Maximum embedded payload fraction. Must be 64.
        /// </summary>
        public byte MaxEmbedPayloadFraction { get; set; }
        /// <summary>
        /// Minimum embedded payload fraction. Must be 32.
        /// </summary>
        public byte MinEmbedPayloadFraction { get; set; }
        /// <summary>
        /// Leaf payload fraction. Must be 32.
        /// </summary>
        public byte LeafPayloadFraction { get; set; }
        /// <summary>
        /// File change counter.
        /// </summary>
        public uint FileChangeCount { get; set; }
        /// <summary>
        /// Size of the database file in pages. The "in-header database size".
        /// </summary>
        public uint PageCount { get; set; }
        /// <summary>
        /// Page number of the first freelist trunk page.
        /// </summary>
        public uint FirstFreeListPage { get; set; }
        /// <summary>
        /// Total number of freelist pages.
        /// </summary>
        public uint FreeListPageCount { get; set; }
        /// <summary>
        /// The schema cookie.
        /// </summary>
        public uint SchemaCookie { get; set; }
        /// <summary>
        /// The schema format number. Supported schema formats are 1, 2, 3, and 4.
        /// </summary>
        public uint SchemaLayerFileFormat { get; set; }
        /// <summary>
        /// Default page cache size.
        /// </summary>
        public uint PageCacheSize { get; set; }
        /// <summary>
        /// The page number of the largest root b-tree page when in auto-vacuum or incremental-vacuum modes, or zero otherwise.
        /// </summary>
        public uint LargestRootPage { get; set; }
        /// <summary>
        /// The database text encoding. A value of 1 means UTF-8. A value of 2 means UTF-16le. A value of 3 means UTF-16be.
        /// </summary>
        public SqliteEncodingValues TextEncodingValue { get; set; }
        /// <summary>
        /// The "user version" as read and set by the user_version pragma.
        /// </summary>
        public uint UserVersion { get; set; }
        /// <summary>
        /// True (non-zero) for incremental-vacuum mode. False (zero) otherwise.
        /// </summary>
        public uint VacuumMode { get; set; }
        /// <summary>
        /// The "Application ID" set by PRAGMA application_id.
        /// </summary>
        /// <remarks>
        /// <para><seealso href="https://www.sqlite.org/pragma.html#pragma_application_id"/></para>
        /// </remarks>
        public uint ApplicationId { get; set; }
        /// <summary>
        /// Reserved for expansion. Must be zero.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private byte[] _reserved;
        /// <summary>
        /// The version-valid-for number.
        /// </summary>
        /// <remarks>
        /// <para><seealso href="https://www.sqlite.org/fileformat2.html#validfor"/></para>
        /// </remarks>
        public uint VersionValidForNumber { get; set; }
        /// <summary>
        /// SQLITE_VERSION_NUMBER.
        /// </summary>
        /// <remarks>
        /// <para><seealso href="https://www.sqlite.org/c3ref/c_source_id.html"/></para>
        /// </remarks>
        public uint VersionNumber { get; set; }


        /// <summary>
        /// Text encoding of database file.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                switch (TextEncodingValue)
                {
                    case SqliteEncodingValues.Utf8:
                        return Encoding.UTF8;
                    case SqliteEncodingValues.Utf16:
                        return Encoding.Unicode;
                    case SqliteEncodingValues.Utf16BigEndian:
                        return Encoding.BigEndianUnicode;
                    default:
                        throw new NotSupportedException("Unrecognized text encoding value " + TextEncodingValue);
                }
            }
        }

        /// <summary>
        /// Get string represents of this instance.
        /// </summary>
        /// <returns>String represents of this instance.</returns>
        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("HeaderString={0}; ", HeaderString)
                .AppendFormat("PageSize={0}; ", PageSize)
                .AppendFormat("WriteVersion={0}({1}); ", WriteVersion, WriteVersion == 1 ? "legacy" : "WAL")
                .AppendFormat("ReadVersion={0}({1}); ", ReadVersion, ReadVersion == 1 ? "legacy" : "WAL")
                .AppendFormat("UnusedSpaceBytes={0}; ", UnusedSpaceBytes)
                .AppendFormat("MaxEmbedPayloadFraction={0}; ", MaxEmbedPayloadFraction)
                .AppendFormat("MinEmbedPayloadFraction={0}; ", MinEmbedPayloadFraction)
                .AppendFormat("LeafPayloadFraction={0}; ", LeafPayloadFraction)
                .AppendFormat("FileChangeCount={0}; ", FileChangeCount)
                .AppendFormat("PageCount={0}; ", PageCount)
                .AppendFormat("FirstFreeListPage={0}; ", FirstFreeListPage)
                .AppendFormat("FreeListPageCount={0}; ", FreeListPageCount)
                .AppendFormat("SchemaCookie={0}; ", SchemaCookie)
                .AppendFormat("SchemaLayerFileFormat={0}; ", SchemaLayerFileFormat)
                .AppendFormat("PageCacheSize={0}; ", PageCacheSize)
                .AppendFormat("LargestRootPage={0}; ", LargestRootPage)
                .AppendFormat("TextEncodingValue={0}; ", TextEncodingValue)
                .AppendFormat("UserVersion={0}; ", UserVersion)
                .AppendFormat("VacuumMode={0}; ", VacuumMode)
                .AppendFormat("ApplicationId={0}; ", ApplicationId)
                .AppendFormat("VersionValidForNumber={0}; ", VersionValidForNumber)
                .AppendFormat("VersionNumber={0}; ", VersionNumber)
                .ToString();
        }


        /// <summary>
        /// Read SQLite3 file header.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <returns>SQLite3 file header.</returns>
        /// <exception cref="InvalidDataException">Throw then specified file size is less than 100 bytes.</exception>
        public static SqliteFileHeader Read(string filePath)
        {
            var size = Marshal.SizeOf(typeof(SqliteFileHeader));
            var headerBytes = new byte[size];

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var nRead = fs.Read(headerBytes, 0, headerBytes.Length);
                if (nRead < headerBytes.Length)
                {
                    throw new InvalidDataException($"Specified file is too small to analyze as SQLite3 file; {nRead} bytes");
                }
            }

            unsafe
            {
                fixed (byte* pHeaderBytes = &headerBytes[0])
                {
                    var header = (SqliteFileHeader)Marshal.PtrToStructure((IntPtr)pHeaderBytes, typeof(SqliteFileHeader)); 
                    if (!BitConverter.IsLittleEndian) {
                        return header;
                    }

                    // Swap bytes for little endian environment.
                    header.FileChangeCount = SwapBytes(header.FileChangeCount);
                    header.FileChangeCount = SwapBytes(header.FileChangeCount);
                    header.FirstFreeListPage = SwapBytes(header.FirstFreeListPage);
                    header.FreeListPageCount = SwapBytes(header.FreeListPageCount);
                    header.SchemaCookie = SwapBytes(header.SchemaCookie);
                    header.SchemaLayerFileFormat = SwapBytes(header.SchemaLayerFileFormat);
                    header.PageCacheSize = SwapBytes(header.PageCacheSize);
                    header.LargestRootPage = SwapBytes(header.LargestRootPage);
                    header.TextEncodingValue = (SqliteEncodingValues)SwapBytes((uint)header.TextEncodingValue);
                    header.UserVersion = SwapBytes(header.UserVersion);
                    header.VacuumMode = SwapBytes(header.VacuumMode);
                    header.ApplicationId = SwapBytes(header.ApplicationId);
                    header.VersionValidForNumber = SwapBytes(header.VersionValidForNumber);
                    header.VersionNumber = SwapBytes(header.VersionNumber);

                    return header;
                }
            }
        }

        /// <summary>
        /// Swap byte ordering.
        /// </summary>
        /// <param name="n">A <see cref="uint"/> value.</param>
        /// <returns>Byte swapped <paramref name="n"/>.</returns>
        private static uint SwapBytes(uint n)
        {
            return (n << 24)
                | ((n & 0x0000ff00u) << 8)
                | ((n & 0x00ff0000u) >> 8)
                | (n >> 24);
        }
    }
}
