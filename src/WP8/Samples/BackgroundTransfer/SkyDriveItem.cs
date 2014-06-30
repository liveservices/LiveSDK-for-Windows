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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Live.WP8.Samples.BackgroundTransfer
{

    /// <summary>
    /// Wrapper around the IDictionary response object to give static typing to it for a SkyDrive Item.
    /// </summary>
    class SkyDriveItem
    {
        private readonly IDictionary<string, object> jsonObject;

        public SkyDriveItem(IDictionary<string, object> jsonObject)
        {
            Debug.Assert(jsonObject != null);
            this.jsonObject = jsonObject;
        }

        public string Id
        {
            get { return this.jsonObject["id"] as string; }
        }

        public string Name
        {
            get { return this.jsonObject["name"] as string; }
        }

        public string Description
        {
            get { return this.jsonObject["description"] as string; }
        }

        public string ParentId
        {
            get { return this.jsonObject["parent_id"] as string; }
        }

        public string UploadLocation
        {
            get { return this.jsonObject["upload_location"] as string; }
        }

        public string Type
        {
            get { return this.jsonObject["type"] as string; }
        }
    }
}
