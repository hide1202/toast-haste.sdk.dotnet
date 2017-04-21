using System;
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
        private readonly int _byteArrayLength;
        private ConcurrentStack<ByteBuffer> _pool;

        internal ByteArrayPool(int byteArrayLength)
        {
            _byteArrayLength = byteArrayLength;
            _pool = new ConcurrentStack<ByteBuffer>();

            for (int i = 0; i < InitialPoolSize; i++)
            {
                _pool.Push(new ByteBuffer(this, byteArrayLength));
            }
        }

        internal ByteBuffer Rent()
        {
            ByteBuffer buffer;
            if (!_pool.TryPop(out buffer))
                buffer = new ByteBuffer(this, _byteArrayLength);

            return buffer;
        }

        internal void Release(ByteBuffer target)
        {
            _pool.Push(target);
        }
    }
}