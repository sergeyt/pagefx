using System;
using System.Collections.Generic;

namespace DataDynamics.PageFX.CodeModel
{
    /// <summary>
    /// Represents node in CodeModel
    /// </summary>
    public interface ICodeNode : IFormattable
    {
        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        CodeNodeType NodeType { get; }

        /// <summary>
        /// Gets the child nodes of this node.
        /// </summary>
        IEnumerable<ICodeNode> ChildNodes { get; }

        /// <summary>
        /// Gets or sets user defined data assotiated with this object.
        /// </summary>
        object Tag { get; set; }
    }
}