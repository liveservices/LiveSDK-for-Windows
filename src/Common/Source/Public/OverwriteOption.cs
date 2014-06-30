// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------

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
