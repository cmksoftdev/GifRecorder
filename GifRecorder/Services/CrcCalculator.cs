using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifRecorder.Services
{
    public class CrcCalculator
    {
        long[] pTable = new long[256];

        //Für Standard CRC32:
        //(Wert kann verändert werden)
        long Poly = 0xEDB88320;

        public CrcCalculator()
        {
            long CRC;
            int i, j;

            for (i = 0; i < 256; i++)
            {
                CRC = i;

                for (j = 0; j < 8; j++)
                {
                    if ((CRC & 0x1) == 1)
                    {
                        CRC = (CRC >> 1) ^ Poly;
                    }
                    else
                    {
                        CRC = (CRC >> 1);
                    }
                }
                pTable[i] = CRC;
            }

        }

        public uint GetCRC32(byte[] input)
        {
            long StreamLength, CRC;

            StreamLength = input.Length;

            CRC = 0xFFFFFFFF;
            for (int i = 0; i < input.Length; i++)
            {
                CRC = ((CRC & 0xFFFFFF00) / 0x100) & 0xFFFFFF ^ pTable[input[i] ^ CRC & 0xFF];
            }
            CRC = (-(CRC)) - 1; // !(CRC)

            return (uint)CRC;
        }

    }
}
