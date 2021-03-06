﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a stored grid row
    /// </summary>
    [Serializable]
    public class StoredValueRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredValueRow"/> class.
        /// </summary>
        public StoredValueRow()
        {
            this.Id = 0;
            this.Cells = new List<StoredValue>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>The cells.</value>
        public List<StoredValue> Cells { get; set; }
    }
}
