using System.Collections.Concurrent;

namespace Haste.ByteBuffer
{
    public interface IHandle
    {
        void Release();
    }

    public class ByteBuffer : IHandle
    {
        private readonly ByteArrayPool _pool;

        internal ByteBuffer(ByteArrayPool pool, int byteArrayLength)
        {
            _pool = pool;
            ByteArray = new byte[byteArrayLength];
        }

        internal byte[] ByteArray { get; }

        public void Release()
        {
            _pool.Release(this);
        }
    }

    public class ByteArrayPool
    {
        private const int InitialPoolSize = 1024;

        private ConcurrentStack<ByteBuffer> _pool;

        internal ByteArrayPool(int byteArrayLength)
        {
            _pool = new ConcurrentStack<ByteBuffer>();

            for (int i = 0; i < InitialPoolSize; i++)
            {
                _pool.Push(new ByteBuffer(this, byteArrayLength));
            }
        }

        internal ByteBuffer Rent()
        {
            ByteBuffer buffer;
            return _pool.TryPop(out buffer) ? buffer : null;
        }

        internal void Release(ByteBuffer target)
        {
            _pool.Push(target);
        }
    }
}