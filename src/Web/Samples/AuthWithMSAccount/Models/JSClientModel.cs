using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Live.Web.Samples.AuthWithMSAccount.Models
{
    public class JSClientModel
    {
        public string ClientId { get; set; }
        public string JSPath { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
    }
}