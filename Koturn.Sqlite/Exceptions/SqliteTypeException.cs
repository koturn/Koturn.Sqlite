using Koturn.Sqlite.Enums;
using System;
using System.Runtime.Serialization;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif


namespace Koturn.Sqlite.Exceptions
{
    /// <summary>
    /// Exception class for type mismatch of SQLite3 column value.
    /// </summary>
    [Serializable]
    public class SqliteTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class.
        /// </summary>
        public SqliteTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class with a specified error message.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="index">Index of column.</param>
        public SqliteTypeException(SqliteValueType expectedType, SqliteValueType actualType, int index)
            : base(string.Format("Attempt to get column value at {0} as an improper type, expected type is {1}, but actual type is {2}", index, expectedType, actualType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class with a specified error message.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="name">Name of column.</param>
        public SqliteTypeException(SqliteValueType expectedType, SqliteValueType actualType, string name)
            : base(string.Format("Attempt to get column value of \"{0}\" as an improper type, expected type is {1}, but actual type is {2}", name, expectedType, actualType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class with
        /// a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="index">Index of column.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.</param>
        public SqliteTypeException(SqliteValueType expectedType, SqliteValueType actualType, int index, Exception inner)
            : base(
                string.Format("Attempt to get column value at {0} as an improper type, expected type is {1}, but actual type is {2}", index, expectedType, actualType),
                inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class with
        /// a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="name">Index of column.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.</param>
        public SqliteTypeException(SqliteValueType expectedType, SqliteValueType actualType, string name, Exception inner)
            : base(
                string.Format("Attempt to get column value of \"{0}\" as an improper type, expected type is {1}, but actual type is {2}", name, expectedType, actualType),
                inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteTypeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected SqliteTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Throws <see cref="SqliteTypeException"/>.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="index">Index of column.</param>
        /// <exception cref="SqliteTypeException">Always thrown.</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        [DoesNotReturn]
#endif
        public static void Throw(SqliteValueType expectedType, SqliteValueType actualType, int index)
        {
            throw new SqliteTypeException(expectedType, actualType, index);
        }

        /// <summary>
        /// Throws <see cref="SqliteTypeException"/>.
        /// </summary>
        /// <param name="expectedType">Expected type of value.</param>
        /// <param name="actualType">Actual type of value.</param>
        /// <param name="name">Name of column.</param>
        /// <exception cref="SqliteTypeException">Always thrown.</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        [DoesNotReturn]
#endif
        public static void Throw(SqliteValueType expectedType, SqliteValueType actualType, string name)
        {
            throw new SqliteTypeException(expectedType, actualType, name);
        }
    }
}
