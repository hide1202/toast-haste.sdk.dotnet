using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Haste.ByteBuffer
{
    public interface IHandle
    {
        void Release();
    }

    public class ByteArray : IHandle
    {
        private readonly Pool<ByteArray> _pool;

        internal ByteArray(Pool<ByteArray> pool, int byteArrayLength)
        {
            _pool = pool;
            Array = new byte[byteArrayLength];
        }

        public static Func<Pool<ByteArray>, ByteArray> Creator(int byteArrayLength)
        {
            return pool => new ByteArray(pool, byteArrayLength);
        }

        internal byte[] Array { get; }

        public void Release()
        {
            _pool.Release(this);
        }
    }

    public class Pool<T> where T : class
    {
        private readonly Func<Pool<T>, T> _creator;
        private readonly ConcurrentStack<T> _pool;

        internal Pool(Func<Pool<T>, T> creator, int preCreateSize)
        {
            _creator = creator;
            _pool = new ConcurrentStack<T>();

            for (int i = 0; i < preCreateSize; i++)
            {
                _pool.Push(_creator(this));
            }
        }

        internal T Rent()
        {
            T buffer = null;
            var wait = new SpinWait();
            for (int i = 0; i < 3; i++)
            {
                if (_pool.TryPop(out buffer))
                    break;
                wait.SpinOnce();
            }

            return buffer ?? _creator(this);
        }

        internal void Release(T target)
        {
            _pool.Push(target);
        }
    }
}