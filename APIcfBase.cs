using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Globalization;
using System.IO;
using static MetaRead.Constants;

namespace MetaRead
{
    [Serializable]
    public struct V8Header_Struct
    {
        public DateTime Time_Create;
        public DateTime Time_Modify;
        public int Zero;
    }

    public struct FAT_Item
    {
        public int Header_Start;
        public int Data_Start;
        public int ff; // всегда 7fffffff
    }

    public struct Catalog_Header
    {
        public int Start_Empty; // начало первого пустого блока
        public int Page_Size;   // размер страницы по умолчанию
        public int Version;     // версия
        public int Zero;        // всегда ноль?
    }

    public enum FileIsCatalog
    {
        iscatalog_unknown,
        iscatalog_true,
        iscatalog_false
    }

    public struct FileTime
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;

        public static implicit operator long(FileTime fileTime)
        {
            long returnedLong;
            // Convert 4 high-order bytes to a byte array
            byte[] highBytes = BitConverter.GetBytes(fileTime.dwHighDateTime);
            // Resize the array to 8 bytes (for a Long)
            Array.Resize(ref highBytes, 8);

            // Assign high-order bytes to first 4 bytes of Long
            returnedLong = BitConverter.ToInt64(highBytes, 0);
            // Shift high-order bytes into position
            returnedLong = returnedLong << 32;
            // Or with low-order bytes
            returnedLong = returnedLong | fileTime.dwLowDateTime;
            // Return long 
            return returnedLong;
        }
    }

    public static class FILETIMEExtensions
    {
        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME time)
        {
            ulong high = (ulong)time.dwHighDateTime;
            uint low = (uint)time.dwLowDateTime;
            long fileTime = (long)((high << 32) + low);
            try
            {
                return DateTime.FromFileTimeUtc(fileTime);
            }
            catch
            {
                return DateTime.FromFileTimeUtc(0xFFFFFFFF);
            }
        }
    }

    public class APIcfBase
    {
        public static readonly String str_cfu = ".cfu";
        public static readonly String str_cfe = ".cfe";
        public static readonly String str_cf = ".cf";
        public static readonly String str_epf = ".epf";
        public static readonly String str_erf = ".erf";

        public static readonly String str_backslash = "\\";


        public static FILETIME DateTimeToFILETIME(DateTime time)
        {

            FILETIME ft;
            var value = time.ToFileTimeUtc();
            ft.dwLowDateTime = (int)(value & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(value >> 32);
            return ft;

        }

        public static FILETIME ToFileTimeStructureUtc(DateTime dateTime)
        {
            var value = dateTime.ToFileTimeUtc();
            return new FILETIME
            {
                dwHighDateTime = unchecked((int)((value >> 32) & 0xFFFFFFFF)),
                dwLowDateTime = unchecked((int)(value & 0xFFFFFFFF))
            };
        }

        public static FILETIME ToFileTimeStructure(DateTime dateTime)
        {
            var value = dateTime.ToFileTime();
            return new FILETIME
            {
                dwHighDateTime = unchecked((int)((value >> 32) & 0xFFFFFFFF)),
                dwLowDateTime = unchecked((int)(value & 0xFFFFFFFF))
            };
        }


        public static bool IsValidValue(DateTime value)
        {
            var flag = true;
            try
            {
                value.ToFileTimeUtc();
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public static void V8timeToFileTime(Int64 v8t, ref FILETIME ft)
        {
            Int64 t = v8t;
            t -= DeltaData1C;
            t *= 1000;
            DateTime dateTime = new DateTime(t);
            ft = DateTimeToFILETIME(dateTime);
        }

        public static void FileTimeToV8time(FILETIME ft, ref Int64 v8t)
        {
            DateTime dateTime = ft.ToDateTime();
            v8t = dateTime.Ticks;
            v8t /= 1000;
            v8t += DeltaData1C;
        }

        public static void SetCurrentTime(Int64 v8t)
        {
            FileTimeToV8time(ToFileTimeStructure(DateTime.Now), ref v8t);
        }

        // string str = "1234abcd";
        // byte[] test_str1 = APIcfBase.GetBytes(str);
        // test_str1[0]  = 0x31
        // test_str1[1]  = 0x00
        // test_str1[2]  = 0x32
        // test_str1[3]  = 0x00
        // test_str1[4]  = 0x33
        // test_str1[5]  = 0x00
        // test_str1[6]  = 0x34
        // test_str1[7]  = 0x00
        // test_str1[8]  = 0x61
        // test_str1[9]  = 0x00
        // test_str1[10] = 0x62
        // test_str1[11] = 0x00
        // test_str1[12] = 0x63
        // test_str1[13] = 0x00
        // test_str1[14] = 0x64
        // test_str1[14] = 0x00
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static int hex_to_int(string hexstr)
        {
            return Convert.ToInt32(hexstr, 16);
        }

        public static string int_to_hex(int dec)
        {
            return Convert.ToString(dec, 16);
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // преобразует шестнадцатиричную восьмисимвольную строку в число
        public static int hex_to_int(byte[] hexstr)
        {

            string syym = GetString(hexstr);

            int res = 0;

            int sym;

            for (int i = 0; i < 8; i++)
            {
                //sym = hexstr[i];
                sym = syym[i];
                if (sym >= 'a')
                {
                    sym -= 'a' - '9' - 1;
                }
                else
                {
                    if (sym > '9')
                        sym -= 'A' - '9' - 1;
                }
                sym -= '0';
                res = (res << 4) | (sym & 0xf);

            }
            return res;
        }

        // преобразует число в шестнадцатиричную восьмисимвольную строку
        public static void int_to_hex(byte[] hexstr, int dec)
        {
            ;
        }

        // читает блок из потока каталога stream_from, собирая его по страницам
        public static Stream Read_Block(Stream stream_from, int start, Stream stream_to = null)
        {
            //char temp_buf[32]; - оригинал
            byte[] temp_buf = new byte[32];

            int len, curlen, pos, readlen;

            if (stream_to == null)
                stream_to = new MemoryStream();
            stream_to.Seek(0, SeekOrigin.Begin);

            if (start < 0 || start == 0x7fffffff || start > stream_from.Length)
                return stream_to;

            stream_from.Seek(0, SeekOrigin.Begin);
            stream_from.Read(temp_buf, 0, 31);

            len = hex_to_int(GetString(temp_buf).Substring(0, 2));
            if (len == 0)
                return stream_to;
            
            curlen = hex_to_int(GetString(temp_buf).Substring(0, 11));
            start  = hex_to_int(GetString(temp_buf).Substring(0, 20));

            readlen = Math.Min(len, curlen);
            stream_from.CopyTo(stream_to, readlen);

            pos = readlen;
            while (start != 0x7fffffff)
            {
                stream_from.Seek(start, SeekOrigin.Begin);
                stream_from.Read(temp_buf, 0, 31);

                curlen = hex_to_int(GetString(temp_buf).Substring(0, 11));
                start  = hex_to_int(GetString(temp_buf).Substring(0, 20));
                readlen = Math.Min(len - pos, curlen);
                stream_from.CopyTo(stream_to, readlen);
                pos += readlen;
            }
            return stream_to;
        }

        public static Catalog_Header ReadFromData(Stream Data)
        {
            Catalog_Header ch = new Catalog_Header();

            using (BinaryReader reader = new BinaryReader(Data, Encoding.ASCII))
            {
                
                ch.Start_Empty = reader.ReadInt32();
                ch.Page_Size   = reader.ReadInt32();
                ch.Version     = reader.ReadInt32();
                ch.Zero        = reader.ReadInt32();

            }
            return ch;
        }

        public static FAT_Item ReadFatItemFromData(Stream Data)
        {
            FAT_Item ch = new FAT_Item();

            using (BinaryReader reader = new BinaryReader(Data, Encoding.ASCII))
            {
                ch.Header_Start = reader.ReadInt32();
                ch.Data_Start   = reader.ReadInt32();
                ch.ff           = reader.ReadInt32();
            }
            return ch;
        }


    }
}
