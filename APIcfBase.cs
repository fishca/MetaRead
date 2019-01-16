using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using System.Globalization;
using System.IO;
using static MetaRead.Constants;
using static MetaRead.Structures;

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

    public enum file_format
    {
        ff_unknown, // неизвестный
        ff_gif,     // GIF
        ff_utf8,    // UTF-8
        ff_pcx,     // PCX
        ff_bmp,
        ff_jpg,
        ff_png,
        ff_tiff,
        ff_ico,
        ff_wmf,
        ff_emf,
        ff_zip
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode1
    {
        [FieldOffset(0)] public int num1;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(100)] public Guid uid1;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(200)] public MetaValue val1;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(300)] public MetaProperty prop1;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(400)] public MetaGeneratedType gentype;                // генерируемый тип (type == stt_gentype)
        [FieldOffset(500)] public ContainerVer vercon1;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(600)] public Version1C ver1C1;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(700)] public SerializationTreeClassType classtype;     // вид коллекции классов ((type == stt_classcol) 
        [FieldOffset(800)] public ClassParameter classpar1;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode2
    {
        [FieldOffset(0)] public int num2;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(100)] public Guid uid2;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(200)] public MetaValue val2;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(300)] public MetaProperty prop2;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(400)] public ContainerVer vercon2;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(500)] public Version1C ver1C2;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(600)] public ClassParameter classpar2;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode3
    {
        [FieldOffset(0)] public int num1;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(100)] public Guid uid1;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(200)] public MetaValue val1;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(300)] public MetaProperty prop1;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(400)] public ContainerVer vercon1;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(500)] public Version1C ver1C1;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(600)] public ClassParameter classpar1;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode4
    {
        [FieldOffset(0)] public int num2;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(100)] public Guid uid2;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(200)] public MetaValue val2;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(300)] public MetaProperty prop2;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(400)] public ContainerVer vercon2;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(500)] public Version1C ver1C2;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(600)] public ClassParameter classpar2;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
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

        public static readonly char[] SIG_GIF87   = { 'G', 'I', 'F', '8', '7', 'a' }; // версия 87 года
        public static readonly char[] SIG_GIF89   = { 'G', 'I', 'F', '8', '9', 'a' }; // версия 89 года
        public static readonly byte[] SIG_UTF8    = { 0xEF, 0xBB, 0xBF };
        public static readonly byte[] SIG_PCX25   = { 0x0a, 0x00, 0x01 }; // версия 2.5
        public static readonly byte[] SIG_PCX28P  = { 0x0a, 0x02, 0x01 }; // версия 2.8  с информацией о палитре
        public static readonly byte[] SIG_PCX28   = { 0x0a, 0x03, 0x01 }; // Версия 2.8 без информации о палитре
        public static readonly byte[] SIG_PCX30   = { 0x0a, 0x05, 0x01 }; // Версия 3.0
        public static readonly byte[] SIG_BMP     = { 0x42, 0x4d };
        public static readonly byte[] SIG_JPG     = { 0xff, 0xd8, 0xff };
        public static readonly byte[] SIG_PNG     = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 };
        public static readonly byte[] SIG_BIGTIFF = { 0x4D, 0x4D, 0x00, 0x2B };
        public static readonly byte[] SIG_TIFFBE  = { 0x4D, 0x4D, 0x00, 0x2A };
        public static readonly byte[] SIG_TIFFLE  = { 0x49, 0x49, 0x2A, 0x00 };
        public static readonly byte[] SIG_ICO     = { 0x00, 0x00, 0x01, 0x00 };
        public static readonly byte[] SIG_WMFOLD  = { 0x01, 0x00, 0x09, 0x00, 0x00, 0x03 };
        public static readonly byte[] SIG_WMF     = { 0xD7, 0xCD, 0xC6, 0x9A, 0x00, 0x00 };
        public static readonly byte[] SIG_EMF     = { 0x01, 0x00, 0x00, 0x00 };
        public static readonly char[] SIG_ZIP     = { 'P', 'K' };


        public static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFILETIME(DateTime time)
        {

            System.Runtime.InteropServices.ComTypes.FILETIME ft;
            var value = time.ToFileTimeUtc();
            ft.dwLowDateTime = (int)(value & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(value >> 32);
            return ft;

        }

        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFileTimeStructureUtc(DateTime dateTime)
        {
            var value = dateTime.ToFileTimeUtc();
            return new System.Runtime.InteropServices.ComTypes.FILETIME
            {
                dwHighDateTime = unchecked((int)((value >> 32) & 0xFFFFFFFF)),
                dwLowDateTime = unchecked((int)(value & 0xFFFFFFFF))
            };
        }

        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFileTimeStructure(DateTime dateTime)
        {
            var value = dateTime.ToFileTime();
            return new System.Runtime.InteropServices.ComTypes.FILETIME
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

        public static void V8timeToFileTime(Int64 v8t, ref System.Runtime.InteropServices.ComTypes.FILETIME ft)
        {
            Int64 t = v8t;
            t -= DeltaData1C;
            t *= 1000;
            DateTime dateTime = new DateTime(t);
            ft = DateTimeToFILETIME(dateTime);
        }

        public static void FileTimeToV8time(System.Runtime.InteropServices.ComTypes.FILETIME ft, ref Int64 v8t)
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
            //char[] chars = new char[bytes.Length];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string GetHexString(byte[] bytes)
        {
            string tmp = "";
            foreach (var item_bytes in bytes)
            {
                tmp += Convert.ToString(item_bytes, 16);
            }
            return tmp;
        }

        public static int GetIntFromArray(byte[] bytes, bool rev = true)
        {
            if (rev)
                Array.Reverse(bytes);

            return Convert.ToInt32(GetHexString(bytes), 16);
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

            //for (int i = 0; i < 8; i++)
            for (int i = 0; i < 8; ++i)
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

        public enum BlockHeader : Int32
        {
            doc_len   = 2,
            block_len = 11,
            nextblock = 20
        }

        #region Оригинал процедуры
        //////          //---------------------------------------------------------------------------
        //////          // читает блок из потока каталога stream_from, собирая его по страницам
        //////          TStream* read_block(TStream* stream_from, int start, TStream* stream_to = nullptr)
        //////          {
        //////              stBlockHeader block_header;
        //////          
        //////              if (!stream_to)
        //////              {
        //////                  stream_to = new TMemoryStream;
        //////              }
        //////          
        //////              stream_to->Seek(0, soFromBeginning);
        //////              stream_to->SetSize(0);
        //////          
        //////              if (start < 0
        //////                      || start == LAST_BLOCK
        //////                      || start > stream_from->GetSize())
        //////              {
        //////                  return stream_to;
        //////              }
        //////          
        //////              stream_from->Seek(start, soFromBeginning);
        //////              stream_from->Read(&block_header, stBlockHeader::size());
        //////          
        //////              int32_t len = block_header.get_data_size();
        //////          
        //////              if (len == 0)
        //////              {
        //////                  return stream_to;
        //////              }
        //////          
        //////              int32_t curlen = block_header.get_page_size();
        //////              start = block_header.get_next_page_addr();
        //////          
        //////              int32_t readlen = std::min(len, curlen);
        //////              stream_to->CopyFrom(stream_from, readlen);
        //////          
        //////              int32_t pos = readlen;
        //////          
        //////              while (start != LAST_BLOCK)
        //////              {
        //////                  stream_from->Seek(start, soFromBeginning);
        //////                  stream_from->Read(&block_header, block_header.size());
        //////          
        //////                  curlen = block_header.get_page_size();
        //////          
        //////                  start = block_header.get_next_page_addr();
        //////          
        //////                  readlen = std::min(len - pos, curlen);
        //////                  stream_to->CopyFrom(stream_from, readlen);
        //////                  pos += readlen;
        //////              }
        //////          
        //////              return stream_to;
        //////          
        //////          }
        #endregion

        // читает блок из потока каталога stream_from, собирая его по страницам
        public static Stream Read_Block(Stream stream_from, int start, Stream stream_to = null)
        {
            //char temp_buf[32]; - оригинал
            byte[] temp_buf = new byte[32];

            stBlockHeader block_header = new stBlockHeader();
            
            int len, curlen, pos, readlen;

            if (stream_to == null)
            {
                stream_to = new MemoryStream();
            }

            stream_to.Seek(0, SeekOrigin.Begin);
            stream_to.SetLength(0);

            if (start < 0 || start == 0x7fffffff || start > stream_from.Length)
            {
                return stream_to;
            }
                

            // спозиционироваться надо на start
            
            stream_from.Seek(start, SeekOrigin.Begin);
            block_header = ReadBlockHeaderFromData(stream_from);

            len = block_header.get_data_size();

            if (len == 0)
                return stream_to;

            curlen = block_header.get_page_size();

            start = block_header.get_next_page_addr();

            readlen = Math.Min(len, curlen);
            byte[] tmp_buf = new byte[readlen];

            ((MemoryStream)stream_to).Capacity = readlen;
            stream_from.CopyTo(stream_to, readlen);

            pos = readlen;
            while (start != 0x7fffffff)
            {

                stream_from.Seek(start, SeekOrigin.Begin);

                block_header = ReadBlockHeaderFromData(stream_from);
                curlen = block_header.get_page_size();
                start = block_header.get_next_page_addr();

                readlen = Math.Min(len - pos, curlen);
                if (readlen != 0)
                    stream_from.CopyTo(stream_to, readlen);
                
                pos += readlen;
            }
            return stream_to;
        }

        public static Catalog_Header ReadFromData(Stream Data)
        {
            Catalog_Header ch = new Catalog_Header();

            // using (BinaryReader reader = new BinaryReader(Data, Encoding.ASCII))
            // {
            //     
            //     ch.Start_Empty = reader.ReadInt32();
            //     ch.Page_Size   = reader.ReadInt32();
            //     ch.Version     = reader.ReadInt32();
            //     ch.Zero        = reader.ReadInt32();
            // 
            // }
            BinaryReader reader = new BinaryReader(Data, Encoding.ASCII);

            ch.Start_Empty = reader.ReadInt32();
            ch.Page_Size   = reader.ReadInt32();
            ch.Version     = reader.ReadInt32();
            ch.Zero        = reader.ReadInt32();

            return ch;
        }

        public static stBlockHeader ReadBlockHeaderFromData(Stream Data, int offset = 0)
        {
            stBlockHeader bh = new stBlockHeader();
            BinaryReader reader = new BinaryReader(Data, Encoding.ASCII);

            //byte[] ttt = reader.ReadBytes(offset);
            if (offset != 0)
                Data.Seek(offset, SeekOrigin.Begin);

            bh.EOL_0D             = (char)reader.ReadByte();
            bh.EOL_0A             = (char)reader.ReadByte();
            bh.data_size_hex      = reader.ReadChars(8);
            bh.space1             = (char)reader.ReadByte();
            bh.page_size_hex      = reader.ReadChars(8);
            bh.space2             = (char)reader.ReadByte();
            bh.next_page_addr_hex = reader.ReadChars(8);
            bh.space3             = (char)reader.ReadByte();
            bh.EOL2_0D            = (char)reader.ReadByte();
            bh.EOL2_0A            = (char)reader.ReadByte();

            return bh;
        }

        public static FAT_Item ReadFatItemFromData(Stream Data)
        {
            FAT_Item ch = new FAT_Item();

            // using (BinaryReader reader = new BinaryReader(Data, Encoding.ASCII))
            // {
            //     ch.Header_Start = reader.ReadInt32();
            //     ch.Data_Start   = reader.ReadInt32();
            //     ch.ff           = reader.ReadInt32();
            // }
            BinaryReader reader = new BinaryReader(Data, Encoding.ASCII);

            ch.Header_Start = reader.ReadInt32();
            ch.Data_Start   = reader.ReadInt32();
            ch.ff           = reader.ReadInt32();

            return ch;
        }

        public static MemoryStream WriteFatItemToStream(FAT_Item fi)
        {
            MemoryStream memoryStream = new MemoryStream();

            //using (BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.ASCII))
            //{
            //    writer.Write(fi.Header_Start);
            //    writer.Write(fi.Data_Start);
            //    writer.Write(fi.ff);
            //}
            BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.ASCII);

            writer.Write(fi.Header_Start);
            writer.Write(fi.Data_Start);
            writer.Write(fi.ff);

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

        public static file_format get_file_format(Stream s)
        {
            byte[] buf = new byte[32];
            int len;

            s.Seek(0, SeekOrigin.Begin);
            len = s.Read(buf, 0, 32);

            if (len < 2)
                return file_format.ff_unknown;

            if (ByteArrayCompare(buf, SIG_BMP)) return file_format.ff_bmp;

            if (ByteArrayCompare(buf, GetBytes(SIG_ZIP.ToString()))) return file_format.ff_zip;

            if (len < 3) return file_format.ff_unknown;

            if (ByteArrayCompare(buf, SIG_JPG))    return file_format.ff_jpg;
            if (ByteArrayCompare(buf, SIG_UTF8))   return file_format.ff_utf8;
            if (ByteArrayCompare(buf, SIG_PCX25))  return file_format.ff_pcx;
            if (ByteArrayCompare(buf, SIG_PCX28P)) return file_format.ff_pcx;
            if (ByteArrayCompare(buf, SIG_PCX28))  return file_format.ff_pcx;
            if (ByteArrayCompare(buf, SIG_PCX30))  return file_format.ff_pcx;

            if (len < 4) return file_format.ff_unknown;

            if (ByteArrayCompare(buf, SIG_BIGTIFF)) return file_format.ff_tiff;
            if (ByteArrayCompare(buf, SIG_TIFFBE))  return file_format.ff_tiff;
            if (ByteArrayCompare(buf, SIG_TIFFLE))  return file_format.ff_tiff;
            if (ByteArrayCompare(buf, SIG_ICO))     return file_format.ff_ico;
            if (ByteArrayCompare(buf, SIG_EMF))     return file_format.ff_emf;

            if (len < 6) return file_format.ff_unknown;

            if (ByteArrayCompare(buf, GetBytes(SIG_GIF87.ToString())))  return file_format.ff_gif;
            if (ByteArrayCompare(buf, GetBytes(SIG_GIF87.ToString())))  return file_format.ff_gif;

            if (ByteArrayCompare(buf, SIG_WMFOLD)) return file_format.ff_wmf;
            if (ByteArrayCompare(buf, SIG_WMF))    return file_format.ff_wmf;

            if (len < 16) return file_format.ff_unknown;

            if (ByteArrayCompare(buf, SIG_PNG)) return file_format.ff_png;

            return file_format.ff_unknown;
        }

        public static bool string_to_GUID(string str, ref Guid guid)
        {
            if (str == "")
                return false;
            guid = new Guid(str);
            return true;
        }
        
        public static Version1C stringtover1C(string s)
        {
            if (string.IsNullOrEmpty(s)) return Version1C.v1C_min;
            if (s == "8.0")    return Version1C.v1C_8_0;
            if (s == "8.1")    return Version1C.v1C_8_1;
            if (s == "8.2")    return Version1C.v1C_8_2;
            if (s == "8.2.14") return Version1C.v1C_8_2_14;
            if (s == "8.3.1")  return Version1C.v1C_8_3_1;
            if (s == "8.3.2")  return Version1C.v1C_8_3_2;
            if (s == "8.3.3")  return Version1C.v1C_8_3_3;
            if (s == "8.3.4")  return Version1C.v1C_8_3_4;
            if (s == "8.3.5")  return Version1C.v1C_8_3_5;
            if (s == "8.3.6")  return Version1C.v1C_8_3_6;
            return Version1C.v1C_min;
        }

        public static string date_to_string(DateTime dt)
        {
            return "";
        }

        public static string date_to_string(string dt)
        {
            return dt;
        }

        public static ObjTab ByteArrayToObjtab(byte[] src)
        {
            ObjTab Res = new ObjTab(0, new UInt32[1023]);

            Res.Numblocks = BitConverter.ToInt32(src, 0);
            Array.Copy(src, 4, Res.Blocks, 0, Res.Numblocks);

            return Res;
        }

        /// <summary>
        /// Структура страницы размещения уровня 1 версий от 8.3.8 
        /// </summary>
        public struct ObjTab838
        {
            private UInt32[] blocks; // реальное количество блоков зависит от размера страницы (pagesize)
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        public static ObjTab838 ByteArrayToObjtab838(byte[] src)
        {
            ObjTab838 Res = new ObjTab838();

            Res.Blocks = new UInt32[1023];
            Array.Clear(Res.Blocks, 0, Res.Blocks.Length);
            Array.Copy(src, 0, Res.Blocks, 0, src.Length);

            return Res;
        }

        /// <summary>
        /// структура заголовочной страницы файла данных или файла свободных страниц 
        /// </summary>
        public struct V8Obj
        {
            private char[] sig; // сигнатура SIG_OBJ
            private UInt32 len; // длина файла
            private _Version version;
            private UInt32[] blocks;

            public char[] Sig { get { return sig; } set { sig = value; } }
            public uint Len { get { return len; } set { len = value; } }
            public _Version Version { get { return version; } set { version = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        public static V8Obj ByteArrayToV8ob(byte[] src)
        {
            // public char[] sig; // сигнатура SIG_OBJ
            // public UInt32 len; // длина файла
            // public _version version;
            // public UInt32[] blocks; // 1018

            //V8ob Res = new V8ob();
            V8Obj Res = new V8Obj();

            Res.Sig = Encoding.UTF8.GetChars(src, 0, 8);
            Res.Len = BitConverter.ToUInt32(src, 8);
            //Res.Version.Version_1 = BitConverter.ToUInt32(src, 12);

            _Version VV = new _Version(0, 0, 0);
            VV.Version_1 = BitConverter.ToUInt32(src, 12);
            VV.Version_2 = BitConverter.ToUInt32(src, 16);
            VV.Version_3 = BitConverter.ToUInt32(src, 20);


            Res.Version = VV;
            //Res.Version.Version_1 = 1;
            //Res.Version.Version_2 = BitConverter.ToUInt32(src, 16);
            //Res.Version.Version_3 = BitConverter.ToUInt32(src, 20);
            Res.Blocks = new UInt32[1018];
            Array.Clear(Res.Blocks, 0, Res.Blocks.Length);
            Array.Copy(src, 24, Res.Blocks, 0, src.Length - 24);

            return Res;
        }

        public static V838ObjData ByteArrayTov838ob(byte[] src)
        {
            // public char[] sig;       // сигнатура 0x1C 0xFD (1C File Data?)  sig[2];
            // public Int16 fatlevel;   // уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            // public _version version;
            // public UInt64 len;       // длина файла
            // public UInt32[] blocks;  // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)  blocks[1];

            V838ObjData Res = new V838ObjData();

            //Res.sig = Encoding.UTF8.GetChars(src, 0, 2);

            Res.Sig = new byte[2];
            Array.Copy(src, 0, Res.Sig, 0, 2);

            Res.Fatlevel = BitConverter.ToInt16(src, 2);

            _Version VV = new _Version(0, 0, 0);


            VV.Version_1 = BitConverter.ToUInt32(src, 4);
            VV.Version_2 = BitConverter.ToUInt32(src, 8);
            VV.Version_3 = BitConverter.ToUInt32(src, 12);

            Res.Version = VV;

            Res.Len = BitConverter.ToUInt64(src, 16);
            //Res.blocks = new UInt32[16378];
            Res.Blocks = new UInt32[1];
            Array.Clear(Res.Blocks, 0, Res.Blocks.Length);
            //Array.Copy(src, 24, Res.blocks, 0, src.Length - 20);
            Array.Copy(src, 24, Res.Blocks, 0, 1);

            return Res;

        }

        public static V838ObjFree ByteArrayTov838ob_free(byte[] src)
        {
            // public char[] sig;     // сигнатура 0x1C 0xFF (1C File Free?)
            // public Int16 fatlevel; // 0x0000 пока! но может ... уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            // public UInt32 version;        // ??? предположительно...
            // public UInt32[] blocks;       // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)

            V838ObjFree Res = new V838ObjFree();

            //Res.sig = Encoding.UTF8.GetChars(src, 0, 2);
            Res.Sig = new byte[2];
            Array.Copy(src, 0, Res.Sig, 0, 2);
            Res.FatLevel = BitConverter.ToInt16(src, 2);
            Res.Version = BitConverter.ToUInt32(src, 4);
            //Res.blocks = new UInt32[16378];
            Res.Blocks = new UInt32[1];
            Array.Clear(Res.Blocks, 0, Res.Blocks.Length);
            //Array.Copy(src, 8, Res.blocks, 0, src.Length - 4);
            Array.Copy(src, 8, Res.Blocks, 0, 1);

            return Res;

        }

        public static bool string1C_to_date(string str, ref string bytedate)
        {
            return true;
        }

        public static void LoadValidValues(Tree tr, List<VarValidValue> validvalues, bool haveglobal = false)
        {
            Tree tt;
            int i, count;
            VarValidValue vvv = new VarValidValue();
            String s;

            tt = tr.Get_First();
            //count = tt.Get_Value().ToIntDef(0);
            count = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < count; ++i)
            {
                tt = tt.Get_Next();
                vvv.value = Convert.ToInt32(tt.Get_Value());
                tt = tt.Get_Next();
                s = tt.Get_Value();
                if (string.IsNullOrEmpty(s))
                    vvv.ver1C = Version1C.v1C_min;
                else
                {
                    vvv.ver1C = stringtover1C(s);
                    if (vvv.ver1C == Version1C.v1C_min)
                    {
                        //error(L"Ошибка загрузки статических типов. Некорректное значение версии 1C в допустимых значениях переменной дерева сериализации"
                        //    //, L"Переменная", fname
                        //    , L"Значение", s);
                    }
                }
                if (haveglobal)
                {
                    tt = tt.Get_Next();
                    vvv.globalvalue = Convert.ToInt32(tt.Get_Value());
                }
                validvalues.Add(vvv);
            }

        }

        public static string hexstring(Stream str)
        {
            int i;
            string s = "";
            string hexdecode = "0123456789abcdef";

            byte c;
            byte b;

            byte[] d = null;
            while (str.Read(d, 0, 1) != 0)
            {
                c = (byte)(d[0] >> 4);
                b = (byte)hexdecode[c];
                s += b;
                c = (byte)(d[0] & 0xf);
                b = (byte)hexdecode[c];
                s += b;
            }
            return s;
        }
    }
}
