using System;

namespace Minesweeper.Utils
{
    internal sealed class IdObfuscator
    {
        private readonly Feistel feistel;

        public IdObfuscator()
        {
            feistel = new Feistel();
        }

        public ulong Permute(ulong id)
        {
            return feistel.Permute(id);
        }

        public string PermuteToBase62(ulong id)
        {
            return Permute(id).ToBase62();
        }

        public string PermuteToBase62(byte[] data)
        {
            if (null == data)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length >= sizeof(ulong))
            {
                return Permute(BitConverter.ToUInt64(data, 0)).ToBase62();
            }

            byte[] complete = new byte[sizeof(ulong)];

            for (int i = 0; i < data.Length; ++i)
            {
                if (BitConverter.IsLittleEndian) // we have two ways of padding
                {
                    complete[i] = data[i];
                }
                else
                {
                    complete[complete.Length - i - 1] = data[data.Length - i - 1];
                }
            }

            return Permute(BitConverter.ToUInt64(complete, 0)).ToBase62();
        }

        internal sealed class Feistel
        {
            private double RoundFunction(ulong input)
            {
                return ((1369 * input + 150889) % 714025) / 714025.0;
            }

            public ulong Permute(ulong n)
            {
                ulong l1 = (n >> 32) & 4294967295L;
                ulong r1 = n & 4294967295L;
                ulong l2, r2;
                for (int i = 0; i < 3; i++)
                {
                    l2 = r1;
                    r2 = l1 ^ (ulong)(this.RoundFunction(r1) * 4294967295L);
                    l1 = l2;
                    r1 = r2;
                }
                return ((r1 << 32) + l1);
            }

            public uint Permute(uint n)
            {
                uint l1 = (n >> 16) & 65535;
                uint r1 = n & 65535;
                uint l2, r2;
                for (int i = 0; i < 3; i++)
                {
                    l2 = r1;
                    r2 = l1 ^ (uint)(RoundFunction(r1) * 65535);
                    l1 = l2;
                    r1 = r2;
                }
                return ((r1 << 16) + l1);
            }
        }
    }
}
