namespace Microsoft.Live.UnitTest
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Live;
    using Microsoft.Live.UnitTest.Stubs;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WEB
    using VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_PHONE
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
#error This platform needs to be handled.
#endif

    // TODO: Move this file to the Common\UnitTests\ directory and update the projects that reference it.

    [TestClass]
    public class ServerResponseReaderTest
    {
        [TestMethod]
        public void TestValidNonErrorJsonResponse()
        {
            var observer = new MockServerResponseReaderObserver();
            ServerResponseReader serverResponseReader = ServerResponseReader.Instance;

            const string jsonResponse = @"{ ""foo"": ""bar"" }";
            serverResponseReader.Read(jsonResponse, observer);

            Assert.IsTrue(
                observer.HasCalledOnSuccessResponse(),
                "The OnSuccessResponse should have been called on the observer.");

            Assert.AreEqual(
                jsonResponse, 
                observer.RawResult, 
                "The observer did not receive the correct raw json result.");

            IDictionary<string, object> result = observer.Result;
            Assert.AreEqual(
                "bar", 
                result["foo"],
                "The observer did not receive the correct json result. It was missing the correct foo property");

            Assert.IsFalse(
                observer.HasCalledOnErrorResponse(), 
                "The OnErrorResponse should NOT have been called on the observer.");

            Assert.IsFalse(
                observer.HasCalledOnInvalidJsonResponse(), 
                "The OnInvalidJsonResponse should NOT have been called on the observer.");
        }

        [TestMethod]
        public void TestValidErrorJsonResponse()
        {
            var observer = new MockServerResponseReaderObserver();
            ServerResponseReader serverResponseReader = ServerResponseReader.Instance;

            const string jsonResponse = @"{ ""error"": { ""code"": ""the code"", ""message"": ""the message"" } }";
            serverResponseReader.Read(jsonResponse, observer);

            Assert.IsTrue(
                observer.HasCalledOnErrorResponse(), 
                "The OnErrorResponse should have been called on the observer.");

            Assert.AreEqual(
                "the code",
                observer.Code,
                "The code value was not properly sent to the observer."
            );

            Assert.AreEqual(
                "the message",
                observer.Message,
                "The message value was not properly sent to the observer."
            );

            Assert.IsFalse(
                observer.HasCalledOnSuccessResponse(),
                "The OnSuccessResponse should NOT have been called on the observer.");

            Assert.IsFalse(
                observer.HasCalledOnInvalidJsonResponse(), 
                "The OnInvalidJsonResponse should NOT have been called on the observer.");
        }

        [TestMethod]
        public void TestInvalidJsonResponse()
        {
            var observer = new MockServerResponseReaderObserver();
            ServerResponseReader serverResponseReader = ServerResponseReader.Instance;

            const string jsonResponse = "INVALID JSON L:KA)(*#!:KAJ}{!#A";
            serverResponseReader.Read(jsonResponse, observer);

            Assert.IsTrue(
                observer.HasCalledOnInvalidJsonResponse(), 
                "The OnInvalidJsonResponse should NOT have been called on the observer.");

            Assert.IsNotNull(observer.Exception,
                "Expected to see a FormatException because of invalid json.");

            Assert.IsFalse(
                observer.HasCalledOnSuccessResponse(),
                "The OnSuccessResponse should NOT have been called on the observer.");

            Assert.IsFalse(
                observer.HasCalledOnErrorResponse(), 
                "The OnErrorResponse should NOT have been called on the observer.");
        }
    }
}
