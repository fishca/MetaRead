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

        /// <summary>
        /// 0x7FFFFFFF - Обозначение последней страницы
        /// </summary>
        public static readonly Int32 LAST_PAGE = Int32.MaxValue;

        public static readonly UInt32 LIVE_CASH = 5; // время жизни кешированных данных в минутах

        /// <summary>
        /// Длина строкового представления GUID-а
        /// </summary>
        public static readonly UInt32 GUID_LEN = 36;

        #region Сигнатура файла 1CD
        /*
        public static readonly char[] SIG_CON      = { '1', 'C', 'D', 'B', 'M', 'S', 'V', '8' };
        public static readonly char[] SIG_OBJ      = { '1', 'C', 'D', 'B', 'O', 'B', 'V', '8' };
        public static readonly char[] SIG_MOXCEL   = { 'M', 'O', 'X', 'C', 'E', 'L', '0' };
        public static readonly char[] SIG_SKD      = { '0', '0', '0', '0', '1', '0', '0', '0' };
        public static readonly char[] SIG_TABDESCR = { '{', '0', '\"', '0' };
        */

        public static readonly String SIG_CON = "1CDBMSV8";
        public static readonly String SIG_OBJ = "1CDBOBV8";

        #endregion

        #region Размеры страницы в файле
        public static readonly UInt32 PAGE_SIZE_4K  = 4096;  // 0x1000
        public static readonly UInt32 PAGE_SIZE_8K  = 8192;  // 0x2000
        public static readonly UInt32 PAGE_SIZE_16K = 16384; // 0x4000
        public static readonly UInt32 PAGE_SIZE_32K = 32768; // 0x8000
        public static readonly UInt32 PAGE_SIZE_64K = 65536; // 0x10000;

        public static readonly UInt32 PAGE4K  = 0x1000;
        public static readonly UInt32 PAGE8K  = 0x2000;
        public static readonly UInt32 PAGE16K = 0x4000;
        public static readonly UInt32 PAGE32K = 0x8000;
        public static readonly UInt32 PAGE64K = 0x10000;


        public static readonly UInt32 DEFAULT_PAGE_SIZE = PAGE_SIZE_4K;
        #endregion

        public static readonly UInt32 DATAHASH_FIELD_LENGTH = 20;

        public static readonly Int16 indexpage_is_root = 1; // Установленный флаг означает, что страница является корневой
        public static readonly Int16 indexpage_is_leaf = 2; // Установленный флаг означает, что страница является листом, иначе веткой

        // Стили преобразования bynary16 в GUID
        //
        // Исходное значение
        // 00112233445566778899aabbccddeeff
        //
        // 1С style
        // ccddeeff-aabb-8899-0011-223344556677
        //
        // MS style
        // 33221100-5544-7766-8899-aabbccddeeff
        //


        public static readonly String EMPTY_GUID = "00000000-0000-0000-0000-000000000000";

        public static readonly String SNAPSHOT_VER1 = "3721ca9e-a272-496b-a093-206ea7629574";
        public static readonly String SNAPSHOT_VER2 = "6fdfdf78-062a-46bb-8402-62145ae55764";

        public static readonly String NODE_GENERAL = "9cd510cd-abfc-11d4-9434-004095e12fc7";

        public static Guid EmptyUID = new Guid();

        public static readonly UInt32 maxStringLength = 4096;


        //public const String _empty_catalog_template = { 0xff, 0xff, 0xff, 0x7f, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //const char _empty_catalog_template[16] = { 0xff, 0xff, 0xff, 0x7f, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //char[] letters = { 'A', 'B', 'C' };
        //string alphabet = new string(letters);

    }
}
