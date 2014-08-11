// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Live.Desktop.Samples.ApiExplorer
{
    public class AuthResult
    {
        public AuthResult(Uri resultUri)
        {
            string[] queryParams = resultUri.Query.TrimStart('?').Split('&');
            foreach (string param in queryParams)
            {
                string[] kvp = param.Split('=');
                switch (kvp[0])
                {
                    case "code":
                        this.AuthorizeCode = kvp[1];
                        break;
                    case "error":
                        this.ErrorCode = kvp[1];
                        break;
                    case "error_description":
                        this.ErrorDescription = Uri.UnescapeDataString(kvp[1]);
                        break;
                }
            }
        }

        public string AuthorizeCode { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorDescription { get; private set; }
    }
}
