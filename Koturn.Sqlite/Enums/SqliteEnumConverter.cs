using System;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif


namespace Koturn.Sqlite.Enums
{
    /// <summary>
    /// Enum value <-> string converter.
    /// </summary>
    public static class SqliteEnumConverter
    {
        /// <summary>
        /// Convert object type name to <see cref="SqliteObjectType"/>.
        /// </summary>
        /// <param name="objTypeName">Object type name.</param>
        /// <returns><see cref="SqliteObjectType"/> value of the <paramref name="objTypeName"/>.</returns>
        public static SqliteObjectType ToObjectType(string objTypeName)
        {
            switch (objTypeName)
            {
                case "table":
                    return SqliteObjectType.Table;
                case "view":
                    return SqliteObjectType.View;
                case "shadow":
                    return SqliteObjectType.Shadow;
                case "virtual":
                    return SqliteObjectType.Virtual;
                case "index":
                    return SqliteObjectType.Index;
                case "trigger":
                    return SqliteObjectType.Trigger;
                default:
                    ThrowArgumentOutOfRangeException("objTypeName", objTypeName, string.Format("{0} is not string representation of SqliteObjectType", objTypeName));
                    return default;
            }
        }

        /// <summary>
        /// Convert <see cref="SqliteObjectType"/> value to string representation.
        /// </summary>
        /// <param name="objType">Object type value.</param>
        /// <returns>Name of the <paramref name="objType"/>.</returns>
        public static string ToObjectTypeName(SqliteObjectType objType)
        {
            switch (objType)
            {
                case SqliteObjectType.Table:
                    return "table";
                case SqliteObjectType.View:
                    return "view";
                case SqliteObjectType.Shadow:
                    return "shadow";
                case SqliteObjectType.Virtual:
                    return "virtual";
                case SqliteObjectType.Index:
                    return "index";
                case SqliteObjectType.Trigger:
                    return "trigger";
                default:
                    ThrowArgumentOutOfRangeException("type", objType, string.Format("{0} is not value of SqliteObjectType", (int)objType));
                    return null;
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">The value of the argument that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <exception cref="ArgumentOutOfRangeException">Always throws.</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        [DoesNotReturn]
#endif
        private static void ThrowArgumentOutOfRangeException<T>(string paramName, T actualValue, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }
    }
}
