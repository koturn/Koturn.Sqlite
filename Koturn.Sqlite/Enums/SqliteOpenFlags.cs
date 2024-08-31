using System;

namespace Koturn.Sqlite.Enums
{
    /// <summary>
    /// Open flags.
    /// </summary>
    /// <remarks>
    /// <para><seealso href="https://www.sqlite.org/c3ref/open.html"/></para>
    /// <para><seealso href="https://www.sqlite.org/c3ref/c_open_autoproxy.html"/></para>
    /// </remarks>
    [Flags]
    public enum SqliteOpenFlags : int
    {
        /// <summary>
        /// The database is opened in read-only mode.
        /// If the database does not already exist, an error is returned.
        /// </summary>
        ReadOnly = 0x00000001,
        /// <summary>
        /// The database is opened for reading and writing if possible, or reading only if the file is write protected by the operating system.
        /// In either case the database must already exist, otherwise an error is returned.
        /// For historical reasons, if opening in read-write mode fails due to OS-level permissions, an attempt is made to open it in read-only mode.
        /// sqlite3_db_readonly() can be used to determine whether the database is actually read-write.
        /// </summary>
        ReadWrite = 0x00000002,
        /// <summary>
        /// Created database if it does not already exist.
        /// </summary>
        Create = 0x00000004,
        /// <summary>
        /// VFS only.
        /// </summary>
        DeleteOnClose = 0x00000008,
        /// <summary>
        /// VFS only.
        /// </summary>
        Exclusive = 0x00000010,
        /// <summary>
        /// VFS only.
        /// </summary>
        AutoProxy = 0x00000020,
        /// <summary>
        /// The filename can be interpreted as a URI if this flag is set.
        /// </summary>
        Uri = 0x00000040,
        /// <summary>
        /// The database will be opened as an in-memory database.
        /// The database is named by the "filename" argument for the purposes of cache-sharing, if shared cache mode is enabled, but the "filename" is otherwise ignored.
        /// </summary>
        Memory = 0x00000080,
        /// <summary>
        /// VFS only.
        /// </summary>
        MainDb = 0x00000100,
        /// <summary>
        /// VFS only.
        /// </summary>
        TempDb = 0x00000200,
        /// <summary>
        /// VFS only.
        /// </summary>
        TransientDb = 0x00000400,
        /// <summary>
        /// VFS only.
        /// </summary>
        MainJournal = 0x00000800,
        /// <summary>
        /// VFS only.
        /// </summary>
        TempJournal = 0x00001000,
        /// <summary>
        /// VFS only.
        /// </summary>
        SubJournal = 0x00002000,
        /// <summary>
        /// VFS only.
        /// </summary>
        MasterJournal = 0x00004000,
        /// <summary>
        /// The new database connection will use the "multi-thread" threading mode.
        /// This means that separate threads are allowed to use SQLite at the same time, as long as each thread is using a different database connection.
        /// </summary>
        NoMutex = 0x00008000,
        /// <summary>
        /// The new database connection will use the "serialized" threading mode.
        /// This means the multiple threads can safely attempt to use the same database connection at the same time.
        /// (Mutexes will block any actual concurrency, but in this mode there is no harm in trying.)
        /// </summary>
        FullMutex = 0x00010000,
        /// <summary>
        /// The database is opened shared cache enabled, overriding the default shared cache setting provided by sqlite3_enable_shared_cache().
        /// The use of shared cache mode is discouraged and hence shared cache capabilities may be omitted from many builds of SQLite.
        /// In such cases, this option is a no-op.
        /// </summary>
        SharedCache = 0x00020000,
        /// <summary>
        /// The database is opened shared cache disabled, overriding the default shared cache setting provided by sqlite3_enable_shared_cache().
        /// </summary>
        PrivateCache = 0x00040000,
        /// <summary>
        /// VFS only.
        /// </summary>
        Wal = 0x00080000,
        /// <summary>
        /// The database filename is not allowed to contain a symbolic link.
        /// </summary>
        /// <remarks>
        /// Available version 3.31.0 or later.
        /// </remarks>
        NoFollow = 0x01000000,
        /// <summary>
        /// The database connection comes up in "extended result code mode".
        /// In other words, the database behaves has if sqlite3_extended_result_codes(db, 1) where called on the database connection as soon as the connection is created.
        /// In addition to setting the extended result code mode, this flag also causes sqlite3_open_v2() to return an extended result code.
        /// </summary>
        /// <remarks>
        /// Available version 3.37.0 or later.
        /// </remarks>
        ExResCode = 0x02000000
    }
}

