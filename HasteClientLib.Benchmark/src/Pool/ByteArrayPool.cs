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

namespace Haste.Pool
{
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
}