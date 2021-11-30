// <copyright file="ExecutionType.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
namespace tiny.WebApi.Enums
{
    /// <summary>
    ///     Values that represent execution types.
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        ///     An enum constant representing the scalar text option.
        /// </summary>
        ScalarText,
        /// <summary>
        ///     An enum constant representing the non query text option.
        /// </summary>
        NonQueryText,
        /// <summary>
        ///     An enum constant representing the scalar procedure option.
        /// </summary>
        ScalarProcedure,
        /// <summary>
        ///     An enum constant representing the non query procedure option.
        /// </summary>
        NonQueryProcedure,
        /// <summary>
        ///     An enum constant representing the data table text option.
        /// </summary>
        DataTableText,
        /// <summary>
        ///     An enum constant representing the data set text option.
        /// </summary>
        DataSetText,
        /// <summary>
        ///     An enum constant representing the data table procedure option.
        /// </summary>
        DataTableProcedure,
        /// <summary>
        ///     An enum constant representing the data set procedure option.
        /// </summary>
        DataSetProcedure
    }
}