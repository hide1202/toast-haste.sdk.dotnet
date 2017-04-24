/*
* Copyright 2016 NHN Entertainment Corp.
*
* NHN Entertainment Corp. licenses this file to you under the Apache License,
* version 2.0 (the "License"); you may not use this file except in compliance
* with the License. You may obtain a copy of the License at:
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Haste.Pool
{
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
