using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    public static class Constants
    {
        //504911232000000 = ((365 * 4 + 1) * 100 - 3) * 4 * 24 * 60 * 60 * 10000
        public const Int64 DeltaData1C = 504911232000000;

        // массив для преобразования числа в шестнадцатиричную строку
        public const String _bufhex = "0123456789abcdef";

        // шаблон заголовка блока
        public const String _block_header_template = "\r\n00000000 00000000 00000000 \r\n";
        public static byte[] _empty_catalog_template = new byte[] { 0xff, 0xff, 0xff, 0x7f, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(a1, a2);
        }

        //public const String _empty_catalog_template = { 0xff, 0xff, 0xff, 0x7f, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //const char _empty_catalog_template[16] = { 0xff, 0xff, 0xff, 0x7f, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //char[] letters = { 'A', 'B', 'C' };
        //string alphabet = new string(letters);

    }
}
