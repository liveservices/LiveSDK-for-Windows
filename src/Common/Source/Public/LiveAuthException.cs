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
