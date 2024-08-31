namespace Koturn.Sqlite.Enums
{
    /// <summary>
    /// Constants Defining Special Destructor Behavior.
    /// </summary>
    /// <seealso href="https://www.sqlite.org/c3ref/c_static.html"/>
    public enum SqliteDestructorType
    {
        /// <summary>
        /// the content pointer is constant and will never change.
        /// It does not need to be destroyed.
        /// </summary>
        Static = 0,
        /// <summary>
        /// the content will likely change in the near future and that
        /// SQLite should make its own private copy of the content before returning.
        /// </summary>
        Transient = -1
    }
}
