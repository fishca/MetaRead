using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using static MetaRead.Constants;

namespace MetaRead
{
    public struct V8Header_Struct
    {
        DateTime Time_Create;
        DateTime Time_Modify;
        int Zero;
    }

    public struct FAT_Item
    {
        int Header_Start;
        int Data_Start;
        int ff; // всегда 7fffffff
    }

    public struct Catalog_Header
    {
        int Start_Empty; // начало первого пустого блока
        int Page_Size;   // размер страницы по умолчанию
        int Version;     // версия
        int Zero;        // всегда ноль?
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


    }
}
