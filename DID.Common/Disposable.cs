using System;
using System.Collections.Generic;
using System.Text;

namespace DID.Common
{
    public class Disposable : IDisposable
    {
        public Disposable()
        {
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void Dispose( bool disposing )
        {

        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
            }
        }

    }
}
