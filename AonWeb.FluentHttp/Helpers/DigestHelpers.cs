using System;

namespace AonWeb.FluentHttp.Helpers
{
    // this is based on code in netduino sha implementation
    public static class DigestHelpers
    {
        private static readonly uint[] Sha256Key = {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

       
        public static byte[] Sha256Hash(byte[] buffer)
        {
            // Initialize working parameters
            uint h0 = 0x6a09e667;
            uint h1 = 0xbb67ae85;
            uint h2 = 0x3c6ef372;
            uint h3 = 0xa54ff53a;
            uint h4 = 0x510e527f;
            uint h5 = 0x9b05688c;
            uint h6 = 0x1f83d9ab;
            uint h7 = 0x5be0cd19;
            uint blockstart = 0;
            
            var length = buffer.Length + 1;
            while ((length % 64) != 56)
                length++;
            
            var processed = new byte[length + 8];

            Array.Copy(buffer, processed, buffer.Length);

            processed[buffer.Length] = 0x80;

            processed[processed.Length - 4] = (byte)(((buffer.Length * 8) & 0xFF000000) >> 24);
            processed[processed.Length - 3] = (byte)(((buffer.Length * 8) & 0x00FF0000) >> 16);
            processed[processed.Length - 2] = (byte)(((buffer.Length * 8) & 0x0000FF00) >> 8);
            processed[processed.Length - 1] = (byte)(((buffer.Length * 8) & 0x000000FF));

            var wordblock = new uint[64];
            
            while (blockstart < processed.Length)
            {
                uint i;
                for (i = 0; i < 16; i++)
                    wordblock[i] = ToBigEndian(processed, blockstart + (i * 4));
                
                for (i = 16; i < 64; i++)
                {
                    var s0 = Rotate(wordblock[i - 15], 7) ^ Rotate(wordblock[i - 15], 18) ^ (wordblock[i - 15] >> 3);
                    var s1 = Rotate(wordblock[i - 2], 17) ^ Rotate(wordblock[i - 2], 19) ^ (wordblock[i - 2] >> 10);
                    wordblock[i] = wordblock[i - 16] + s0 + wordblock[i - 7] + s1;
                }
                
                var a = h0;
                var b = h1;
                var c = h2;
                var d = h3;
                var e = h4;
                var f = h5;
                var g = h6;
                var h = h7;
                
                for (i = 0; i < 64; i++)
                {
                    var t1 = h + (Rotate(e, 6) ^ Rotate(e, 11) ^ Rotate(e, 25)) + Choice(e, f, g) + Sha256Key[i] + wordblock[i];
                    var t2 = (Rotate(a, 2) ^ Rotate(a, 13) ^ Rotate(a, 22)) + Majority(a, b, c);
                    h = g;
                    g = f;
                    f = e;
                    e = d + t1;
                    d = c;
                    c = b;
                    b = a;
                    a = t1 + t2;
                }
                
                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += e;
                h5 += f;
                h6 += g;
                h7 += h;
                
                blockstart += 64;
            }
            
            var output = new byte[32];
            FromBigEndian(h0, ref output, 0);
            FromBigEndian(h1, ref output, 4);
            FromBigEndian(h2, ref output, 8);
            FromBigEndian(h3, ref output, 12);
            FromBigEndian(h4, ref output, 16);
            FromBigEndian(h5, ref output, 20);
            FromBigEndian(h6, ref output, 24);
            FromBigEndian(h7, ref output, 28);

            return output;
        }

        private static void FromBigEndian(uint input, ref byte[] output, int start)
        {
            output[start] = (byte)((input & 0xFF000000) >> 24);
            output[start + 1] = (byte)((input & 0x00FF0000) >> 16);
            output[start + 2] = (byte)((input & 0x0000FF00) >> 8);
            output[start + 3] = (byte)((input & 0x000000FF));
        }

        private static uint ToBigEndian(byte[] input, uint start)
        {
            uint r = 0;
            r |= (((uint)input[start]) << 24);
            r |= (((uint)input[start + 1]) << 16);
            r |= (((uint)input[start + 2]) << 8);
            r |= ((input[start + 3]));
            return r;
        }

        private static uint Rotate(uint x, int n)
        {
            return ((x >> n) | (x << (32 - n)));
        }

        private static uint Majority(uint x, uint y, uint z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        private static uint Choice(uint x, uint y, uint z)
        {
            return ((x & y) ^ (~x & z));
        }
    }
}