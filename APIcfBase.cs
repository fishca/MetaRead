﻿using System;
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

        // шаблон заголовка блока
        public static readonly String _BLOCK_HEADER_TEMPLATE = "\r\n00000000 00000000 00000000 \r\n";
        public static readonly String _EMPTY_CATALOG_TEMPLATE = "FFFFFF7F020000000000";

        public static readonly Int32  LAST_BLOCK  = 0x7FFFFFFF;
        public static readonly UInt32 LAST_BLOCK2 = 0x7FFFFFFF;
        public static readonly UInt32 BLOCK_HEADER_LEN    = 32U;
        public static readonly Int32  BLOCK_HEADER_LEN2   = 32;
        public static readonly UInt32 CATALOG_HEADER_LEN  = 16U;
        public static readonly Int32  CATALOG_HEADER_LEN2 = 16;

        public static readonly Int64 EPOCH_START_WIN = 504911232000000;
        public static readonly Int32 HEX_INT_LEN = 2 * 2;


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

            //curlen = hex_to_int(GetString(temp_buf).Substring(0, 11));
            curlen = hex_to_int(GetString(temp_buf).Substring(11, 11)); // скорее всего должно быть так

            //start  = hex_to_int(GetString(temp_buf).Substring(0, 20));
            start = hex_to_int(GetString(temp_buf).Substring(20, 20));  // скорее всего должно быть так

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

        public static MemoryStream WriteFatItemToStream(FAT_Item fi)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.ASCII))
            {
                writer.Write(fi.Header_Start);
                writer.Write(fi.Data_Start);
                writer.Write(fi.ff);
            }
            return memoryStream;
        }

        #region InflateAndDeflate

        /// <summary>
        /// Распаковка
        /// </summary>
        /// <param name="compressedMemoryStream"></param>
        /// <param name="outBufStream"></param>
        /// <returns></returns>
        public static bool Inflate(MemoryTributary compressedMemoryStream, out MemoryTributary outBufStream)
        {
            bool result = true;

            outBufStream = new MemoryTributary();

            try
            {
                compressedMemoryStream.Position = 0;
                System.IO.Compression.DeflateStream decompressStream = new System.IO.Compression.DeflateStream(compressedMemoryStream, System.IO.Compression.CompressionMode.Decompress);
                decompressStream.CopyTo(outBufStream);
            }
            catch (Exception ex)
            {
                outBufStream = compressedMemoryStream;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Сжатие
        /// </summary>
        /// <param name="pDataStream"></param>
        /// <param name="outBufStream"></param>
        /// <returns></returns>
        public static bool Deflate(MemoryTributary pDataStream, out MemoryTributary outBufStream)
        {
            bool result = true;

            int DataSize = (int)pDataStream.Length;
            outBufStream = new MemoryTributary();

            pDataStream.Position = 0;
            try
            {
                MemoryTributary srcMemStream = pDataStream;
                {
                    using (MemoryTributary compressedMemStream = new MemoryTributary())
                    {
                        using (System.IO.Compression.DeflateStream strmDef = new System.IO.Compression.DeflateStream(compressedMemStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            srcMemStream.CopyTo(strmDef);
                        }

                        outBufStream = compressedMemStream;
                    }
                }
            }
            catch (Exception ex)
            {
                outBufStream = pDataStream;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Распаковка произвольных файлов
        /// </summary> 
        public void Inflate(string in_filename, string out_filename, bool enableNewCode = true)
        {
            if (!File.Exists(in_filename))
                throw new Exception("Input file not found!");

            using (FileStream fileReader = File.Open(in_filename, FileMode.Open))
            {
                MemoryTributary memOutBuffer;
                using (MemoryTributary memBuffer = new MemoryTributary())
                {
                    fileReader.CopyTo(memBuffer);

                    bool success = Inflate(memBuffer, out memOutBuffer);
                    if (!success)
                        throw new Exception("Inflate error!");

                    using (FileStream fileWriter = new FileStream(out_filename, FileMode.Create))
                    {
                        memOutBuffer.Position = 0;
                        memOutBuffer.CopyTo(fileWriter);
                    }
                    memOutBuffer.Close();
                }
            }
        }

        /// <summary>
        /// Сжатие произвольных файлов
        /// </summary>
        public void Deflate(string in_filename, string out_filename, bool enableNewCode = true)
        {
            if (!File.Exists(in_filename))
                throw new Exception("Input file not found!");

            using (FileStream fileReader = File.Open(in_filename, FileMode.Open))
            {
                MemoryTributary memOutBuffer;
                using (MemoryTributary memBuffer = new MemoryTributary())
                {
                    fileReader.CopyTo(memBuffer);

                    bool success = Deflate(memBuffer, out memOutBuffer);
                    if (!success)
                        throw new Exception("Deflate error!");

                    using (FileStream fileWriter = new FileStream(out_filename, FileMode.Create))
                    {
                        memOutBuffer.Position = 0;
                        memOutBuffer.CopyTo(fileWriter);
                    }
                    memOutBuffer.Close();
                }
            }
        }

        #endregion


        #region Service
        /// <summary>
        /// _httoi(Byte[] value) - преобразует массив Byte[] в целое значение 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 _httoi(Byte[] value)
        {
            UInt32 result = 0;

            string newByte = System.Text.Encoding.Default.GetString(value);
            result = UInt32.Parse(newByte, System.Globalization.NumberStyles.HexNumber);

            return result;
        }

        /// <summary>
        /// _intTo_BytesChar - преобразует целое значение в массив Byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Byte[] _intTo_BytesChar(UInt32 value)
        {
            string valueString = IntToHexString((int)value, 8).ToLower();
            Byte[] resultBytes = new Byte[8];

            for (int i = 0; i < valueString.Length; i++)
                resultBytes[i] = Convert.ToByte(valueString[i]);

            return resultBytes;
        }

        /// <summary>
        /// _inttoBytes - преобразует целое значение в массив Byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Byte[] _inttoBytes(UInt32 value)
        {
            string valueString = IntToHexString((int)value, 8).ToUpper();

            Byte[] resultBytes = new Byte[8];

            for (int i = 0; i < valueString.Length; i++)
            {
                switch (valueString[i])
                {
                    case 'A':
                        resultBytes[i] = 10;
                        break;
                    case 'B':
                        resultBytes[i] = 11;
                        break;
                    case 'C':
                        resultBytes[i] = 12;
                        break;
                    case 'D':
                        resultBytes[i] = 13;
                        break;
                    case 'E':
                        resultBytes[i] = 14;
                        break;
                    case 'F':
                        resultBytes[i] = 15;
                        break;
                    default:
                        resultBytes[i] = (Byte)(Convert.ToByte(valueString[i]) - 0x30);
                        break;
                }
            }

            return resultBytes;
        }

        /// <summary>
        /// IntToHexString - преобразование целого с определенной длиной в строку шестнадцатеричную
        /// </summary>
        /// <param name="n"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String IntToHexString(int n, int len)
        {
            Char[] ch = new Char[len--];
            for (int i = len; i >= 0; i--)
            {
                ch[len - i] = ByteToHexChar((Byte)((uint)(n >> 4 * i) & 15));
            }
            return new String(ch);
        }

        public static Int32 HexStringToInt(String instr)
        {
            Int32 Result = 0;

            Result = Convert.ToInt32(instr);

            return Result;
        }

        /// <summary>
        /// ByteToHexChar - перевод байта в шестнадцатеричный символ (a-f)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Char ByteToHexChar(Byte b)
        {
            if (b < 0 || b > 15)
                throw new Exception("IntToHexChar: input out of range for Hex value");
            return b < 10 ? (Char)(b + 48) : (Char)(b + 55);
        }

        /// <summary>
        /// ClearTempData - Очистка временного каталога
        /// </summary>
        /// <param name="tmpFolder"></param>
        /// <param name="_tmpFolder"></param>
        public static void ClearTempData(String tmpFolder = "", String _tmpFolder = "")
        {
            if (!String.IsNullOrEmpty(tmpFolder))
            {
                try
                {
                    Directory.Delete(_tmpFolder, true);
                }
                catch
                {
                }
            }

            String V8FormatsTmp = String.Format("{0}V8Formats{1}", Path.GetTempPath(), Path.DirectorySeparatorChar);
            if (Directory.Exists(V8FormatsTmp))
            {
                string[] foundDirectories = Directory.GetDirectories(V8FormatsTmp);
                foreach (string dirFullname in foundDirectories)
                {
                    try
                    {
                        DirectoryInfo tmpDir = new DirectoryInfo(dirFullname);
                        if (tmpDir.CreationTime < DateTime.Now.AddHours(-1))
                        {
                            tmpDir.Delete(true);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Определяет размер каталога
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Добавляем размер файлов
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Добавляем размер подкаталогов
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }
        #endregion


    }
}
