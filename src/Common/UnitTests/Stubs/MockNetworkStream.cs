namespace Microsoft.Live.UnitTest
{
    using System;
    using System.IO;

    public class MockNetworkStream : MemoryStream
    {
        public string Content { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Flush();
                this.Seek(0, 0);
                StreamReader sr = new StreamReader(this);
                this.Content = sr.ReadToEnd();
            }

            base.Dispose(disposing);
        }
    }
}
