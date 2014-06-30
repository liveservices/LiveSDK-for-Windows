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

namespace Microsoft.Live.Controls
{
    using System.Resources;

    internal static class ResourceHelper
    {
        private static readonly ResourceManager stringResourceManager;
        private static readonly ResourceManager errorResourceManager;

        static ResourceHelper()
        {
            stringResourceManager = new ResourceManager(typeof(Microsoft.Live.Controls.Resources.WLText));
            errorResourceManager = new ResourceManager(typeof(Microsoft.Live.Controls.Resources.Errors));
        }

        public static string GetErrorString(string name)
        {
            return errorResourceManager.GetString(name);
        }

        public static string GetResourceString(string name)
        {
            return stringResourceManager.GetString(name);
        }
    }
}
