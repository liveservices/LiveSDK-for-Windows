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
