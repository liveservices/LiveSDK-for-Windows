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

namespace Microsoft.Live
{
    using System;

    /// <summary>
    /// An exception type used by the auth client.
    /// </summary>
    public class LiveAuthException : Exception
    {
        /// <summary>
        /// Constructs a new exception instance.
        /// </summary>
        public LiveAuthException()
        {
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// </summary>
        public LiveAuthException(string errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// <param name="innerException">The wrapped exception.</param>
        /// </summary>
        public LiveAuthException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructs a new exception instance.
        /// <param name="errorCode">Error code corresponds to the "error" parameter returned by the server.</param>
        /// <param name="message">Error message corresponds to the "errorDescription" parameter returned by the server.</param>
        /// <param name="requestState">The request state that is encoded in the login Url parameter.</param>
        /// </summary>
        internal LiveAuthException(string errorCode, string message, string requestState)
            : base(message)
        {
            this.ErrorCode = errorCode;
            this.State = requestState;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Gets the request state value that is encoded in the login Url parameter.
        /// </summary>
        public string State { get; internal set; }

        /// <summary>
        /// Customize the display string for the exception object.
        /// </summary>
        public override string ToString()
        {
            return this.ErrorCode + ": " + base.ToString();
        }
    }
}
