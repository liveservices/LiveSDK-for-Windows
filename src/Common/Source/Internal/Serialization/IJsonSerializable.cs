namespace Microsoft.Live.Serialization
{
    using System;

    /// <summary>
    ///     Represents an object that can be serialized into JSON.
    /// </summary>
    internal interface IJsonSerializable
    {
        /// <summary>
        ///     Converts the object to JSON.
        /// </summary>
        string ToJson();
    }
}