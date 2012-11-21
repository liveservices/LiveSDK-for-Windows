using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Live.Web.Samples.AuthWithMSAccount.Models
{
    public class AccountDataModel
    {
        public string HomeURL { get; set; }
        public string LogoutURL { get; set; }
        public string ShowContactsURL { get; set; }
        public string Name { get; set; }
        public byte[] ProfileImage { get; set; }
        public string Error { get; set; }
        public object Contacts { get; set; }
    }
}