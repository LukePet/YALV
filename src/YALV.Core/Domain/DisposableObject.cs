using System;

namespace YALV.Core.Domain
{
    /// <summary>
    /// "Implementing a Dispose method"
    /// http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx
    /// </summary>
    public class DisposableObject
        : IDisposable
    {
        protected bool _disposed;

        public DisposableObject()
        {
        }

        public DisposableObject(Action action)
        {
            Disposed = action;
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);

            //Raise disposed event
            if (Disposed != null)
                Disposed();
        }

        protected void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    OnDispose();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }

        protected virtual void OnDispose() { }

        public event Action Disposed = delegate { };
    }
}
