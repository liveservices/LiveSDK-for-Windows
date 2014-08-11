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
    // Reference: http://tools.ietf.org/search/draft-jones-json-web-token-00
    //
    // JWT is made up of 3 parts: Envelope, Claims, Signature.
    // - Envelope - specifies the token type and signature algorithm used to produce 
    //       signature segment. This is in JSON format
    // - Claims - specifies claims made by the token. This is in JSON format
    // - Signature - Cryptographic signature use to maintain data integrity.
    // 
    // To produce a JWT token:
    // 1. Create Envelope segment in JSON format
    // 2. Create Claims segment in JSON format
    // 3. Create signature
    // 4. Base64url encode each part and append together separated by "." 

    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.IO;

    internal class JsonWebToken
    {
        #region Properties

        private static readonly DataContractJsonSerializer ClaimsJsonSerializer = new DataContractJsonSerializer(typeof(JsonWebTokenClaims));
        private static readonly DataContractJsonSerializer EnvelopeJsonSerializer = new DataContractJsonSerializer(typeof(JsonWebTokenEnvelope));
        private static readonly UTF8Encoding UTF8Encoder = new UTF8Encoding(true, true);
        private static readonly SHA256Managed SHA256Provider = new SHA256Managed();

        private string claimsTokenSegment;
        private string envelopeTokenSegment;

        /// <summary>
        /// Gets the JsonWebToken claims detail.
        /// </summary>
        public JsonWebTokenClaims Claims
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the JsonWebToken envelope data
        /// </summary>
        public JsonWebTokenEnvelope Envelope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the token signature.
        /// </summary>
        public string Signature
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if the token is already expired.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return this.Claims.Expiration < DateTime.UtcNow;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of JsonWebToken instance.
        /// </summary>
        public JsonWebToken(string token, object secrets)
        {
            var keyMap = secrets as IDictionary<int, string>;
            var key = secrets as string;

            Debug.Assert(keyMap != null || !string.IsNullOrEmpty(key));

            // Get the token segments & perform validation
            string[] tokenSegments = this.SplitToken(token);

            // Decode and deserialize the claims
            this.claimsTokenSegment = tokenSegments[1];
            this.Claims = this.GetClaimsFromTokenSegment(this.claimsTokenSegment);

            // Decode and deserialize the envelope
            this.envelopeTokenSegment = tokenSegments[0];
            this.Envelope = this.GetEnvelopeFromTokenSegment(this.envelopeTokenSegment);

            // Get the signature
            this.Signature = tokenSegments[2];

            if (keyMap != null)
            {
                // If a secret key map is provided, ensure that the token's KeyId exists in the key map.
                if (!keyMap.ContainsKey(this.Envelope.KeyId))
                {
                    throw new Exception(string.Format("Could not find key with id {0}", this.Envelope.KeyId));
                }

                key = keyMap[this.Envelope.KeyId];
            }

            // Validation
            this.ValidateEnvelope(this.Envelope);
            this.ValidateSignature(key);
        }

        #endregion

        #region Parsing Methods

        private JsonWebTokenClaims GetClaimsFromTokenSegment(string claimsTokenSegment)
        {
            byte[] claimsData = this.Base64UrlDecode(claimsTokenSegment);
            using (MemoryStream memoryStream = new MemoryStream(claimsData))
            {
                return ClaimsJsonSerializer.ReadObject(memoryStream) as JsonWebTokenClaims;
            }
        }

        private JsonWebTokenEnvelope GetEnvelopeFromTokenSegment(string envelopeTokenSegment)
        {
            byte[] envelopeData = this.Base64UrlDecode(envelopeTokenSegment);
            using (MemoryStream memoryStream = new MemoryStream(envelopeData))
            {
                return EnvelopeJsonSerializer.ReadObject(memoryStream) as JsonWebTokenEnvelope;
            }
        }

        private string[] SplitToken(string token)
        {
            // Expected token format: Envelope.Claims.Signature

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token is empty or null.");
            }

            string[] segments = token.Split('.');

            if (segments.Length != 3)
            {
                throw new Exception("Invalid token format. Expected Envelope.Claims.Signature");
            }

            if (string.IsNullOrEmpty(segments[0]))
            {
                throw new Exception("Invalid token format. Envelope must not be empty");
            }

            if (string.IsNullOrEmpty(segments[1]))
            {
                throw new Exception("Invalid token format. Claims must not be empty");
            }

            if (string.IsNullOrEmpty(segments[2]))
            {
                throw new Exception("Invalid token format. Signature must not be empty");
            }

            return segments;
        }

        #endregion

        #region Validation Methods

        private void ValidateEnvelope(JsonWebTokenEnvelope envelope)
        {
            if (envelope.Type != "JWT")
            {
                throw new Exception("Unsupported token type");
            }

            if (envelope.Algorithm != "HS256")
            {
                throw new Exception("Unsupported crypto algorithm");
            }
        }

        private void ValidateSignature(string key)
        {
            // Derive signing key, Signing key = SHA256(secret + "JWTSig")
            byte[] bytes = UTF8Encoder.GetBytes(key + "JWTSig");
            byte[] signingKey = SHA256Provider.ComputeHash(bytes);

            // To Validate:
            // 
            // 1. Take the bytes of the UTF-8 representation of the JWT Claim
            //  Segment and calculate an HMAC SHA-256 MAC on them using the
            //  shared key.
            //
            // 2. Base64url encode the previously generated HMAC as defined in this
            //  document.
            //
            // 3. If the JWT Crypto Segment and the previously calculated value
            //  exactly match in a character by character, case sensitive
            //  comparison, then one has confirmation that the key was used to
            //  generate the HMAC on the JWT and that the contents of the JWT
            //  Claim Segment have not be tampered with.
            //
            // 4. If the validation fails, the token MUST be rejected.

            // UFT-8 representation of the JWT envelope.claim segment
            byte[] input = UTF8Encoder.GetBytes(this.envelopeTokenSegment + "." + this.claimsTokenSegment);

            // calculate an HMAC SHA-256 MAC
            using (HMACSHA256 hashProvider = new HMACSHA256(signingKey))
            {
                byte[] myHashValue = hashProvider.ComputeHash(input);

                // Base64 url encode the hash
                string base64urlEncodedHash = this.Base64UrlEncode(myHashValue);

                // Now compare the two has values
                if (base64urlEncodedHash != this.Signature)
                {
                    throw new Exception("Signature does not match.");
                }
            }
        }

        #endregion

        #region Base64 Encode / Decode Functions
        // Reference: http://tools.ietf.org/search/draft-jones-json-web-token-00

        private byte[] Base64UrlDecode(string encodedSegment)
        {
            string s = encodedSegment;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        private string Base64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }
        #endregion
    }
}