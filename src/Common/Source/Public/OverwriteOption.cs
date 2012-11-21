namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// OverwriteOption is used for upload load operations. It specifies what should be done
    /// in case of a file name conflict.
    /// </summary>
    public enum OverwriteOption
    {
        /// <summary>
        /// Does not perform an overwrite if a file with the same name already exists.
        /// </summary>
        DoNotOverwrite = 0,

        /// <summary>
        /// Overwrites the exisiting file with the new file's contents.
        /// </summary>
        Overwrite = 1,

        /// <summary>
        /// If a file with the same name exists, this option will rename the new file.
        /// </summary>
        Rename = 2
    }
}
