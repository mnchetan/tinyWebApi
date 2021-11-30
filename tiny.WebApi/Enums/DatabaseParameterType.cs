// <copyright file="DatabaseParameterType.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
namespace tiny.WebApi.Enums
{
    /// <summary>
    ///     Values that represent database parameter types.
    /// </summary>
    public enum DatabaseParameterType
    {
        /// <summary>
        ///     An enum constant representing the ANSI string option.
        /// </summary>
        AnsiString,
        /// <summary>
        ///     An enum constant representing the binary option.
        /// </summary>
        Binary,
        /// <summary>
        ///     An enum constant representing the byte option.
        /// </summary>
        Byte,
        /// <summary>
        ///     An enum constant representing the boolean option.
        /// </summary>
        Boolean,
        /// <summary>
        ///     An enum constant representing the currency option.
        /// </summary>
        Currency,
        /// <summary>
        ///     An enum constant representing the date option.
        /// </summary>
        Date,
        /// <summary>
        ///     An enum constant representing the date time option.
        /// </summary>
        DateTime,
        /// <summary>
        ///     An enum constant representing the decimal option.
        /// </summary>
        Decimal,
        /// <summary>
        ///     An enum constant representing the double option.
        /// </summary>
        Double,
        /// <summary>
        ///     An enum constant representing the Unique identifier option.
        /// </summary>
        Guid,
        /// <summary>
        ///     An enum constant representing the int 16 option.
        /// </summary>
        Int16,
        /// <summary>
        ///     An enum constant representing the int 32 option.
        /// </summary>
        Int32,
        /// <summary>
        ///     An enum constant representing the int 64 option.
        /// </summary>
        Int64,
        /// <summary>
        ///     An enum constant representing the object option.
        /// </summary>
        Object,
        /// <summary>
        ///     An enum constant representing the byte option.
        /// </summary>
        SByte,
        /// <summary>
        ///     An enum constant representing the single option.
        /// </summary>
        Single,
        /// <summary>
        ///     An enum constant representing the string option.
        /// </summary>
        String,
        /// <summary>
        ///     An enum constant representing the time option.
        /// </summary>
        Time,
        /// <summary>
        ///     An enum constant representing the int 16 option.
        /// </summary>
        UInt16,
        /// <summary>
        ///     An enum constant representing the int 32 option.
        /// </summary>
        UInt32,
        /// <summary>
        ///     An enum constant representing the int 64 option.
        /// </summary>
        UInt64,
        /// <summary>
        ///     An enum constant representing the Variable numeric option.
        /// </summary>
        VarNumeric,
        /// <summary>
        ///     An enum constant representing the ANSI string fixed length option.
        /// </summary>
        AnsiStringFixedLength,
        /// <summary>
        ///     An enum constant representing the string fixed length option.
        /// </summary>
        StringFixedLength,
        /// <summary>
        ///     An enum constant representing the XML option.
        /// </summary>
        Xml,
        /// <summary>
        ///     An enum constant representing the date time 2 option.
        /// </summary>
        DateTime2,
        /// <summary>
        ///     An enum constant representing the date time offset option.
        /// </summary>
        DateTimeOffset,
        /// <summary>
        ///     An enum constant representing the structured option.
        /// </summary>
        Structured,
        /// <summary>
        ///     An enum constant representing the un known option.
        /// </summary>
        UnKnown,
        /// <summary>
        ///     An enum constant representing the Reference cursor option.
        /// </summary>
        RefCursor
    }
}