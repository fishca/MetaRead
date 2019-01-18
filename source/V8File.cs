using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using static MetaRead.Constants;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    public class V8File
    {
        public String Name;
        public DateTime TimeCreate;
        public DateTime TimeModify;

        public Stream Data;
        public V8Catalog Parent;
        public FileIsCatalog ISCatalog;
        public V8Catalog Self; // указатель на каталог, если файл является каталогом

        public V8File Next;     // следующий файл в каталоге
        public V8File Previous; // предыдущий файл в каталоге

        public bool IsOpened;   // признак открытого файла (инициализирован поток data)

        public int StartData;   // начало блока данных файла в каталоге (0 означает, что файл в каталоге не записан)
        public int StartHeader; // начало блока заголовка файла в каталоге

        public bool IsDataModified;   // признак модифицированности данных файла (требуется запись в каталог при закрытии)
        public bool IsHeaderModified; // признак модифицированности заголовка файла (требуется запись в каталог при закрытии)

        public bool IsDestructed; // признак, что работает деструктор
        public bool Flushed;      // признак, что происходит сброс

        public bool SelfZipped; // Признак, что файл является запакованным независимо от признака zipped каталога

        public V8File()
        {
        }

        public V8File(V8Catalog _parent, String _name, V8File _previous, int _start_data, int _start_header, DateTime _time_create, DateTime _time_modify)
        {
            IsDestructed = false;
            Flushed = false;
            Parent = _parent;
            Name = _name;
            Previous = _previous;
            Next = null;
            Data = null;
            StartData = _start_data;
            StartHeader = _start_header;
            IsDataModified = (StartData != 0 ? true : false);
            IsHeaderModified = (StartHeader != 0 ? true : false);
            if (Previous != null)
                Previous.Next = this;
            else
                Parent.First = this;
            ISCatalog = FileIsCatalog.iscatalog_unknown;
            Self = null;
            IsOpened = false;
            TimeCreate = _time_create;
            TimeModify = _time_modify;
            SelfZipped = false;
            if (Parent != null)
                Parent.Files[Name.ToUpper()] = this;
        }


        public bool IsCatalog()
        {
            int _filelen = 0;
            int _startempty = -1;

            byte[] _t = new byte[32];
            byte[] _t4 = new byte[8];

            if (ISCatalog == FileIsCatalog.iscatalog_unknown)
            {
                if (!IsOpened)
                    if (!Open())
                        return false;

                _filelen = (int)Data.Length;

                if (_filelen == 16)
                {
                    Data.Seek(0, SeekOrigin.Begin);
                    Data.Read(_t, 0, 16);
                    if (!ByteArrayCompare(_t, _empty_catalog_template))
                    {
                        ISCatalog = FileIsCatalog.iscatalog_false;
                        return false;
                    }
                    else
                    {
                        ISCatalog = FileIsCatalog.iscatalog_true;
                        return true;
                    }


                }
                Data.Seek(0, SeekOrigin.Begin);
                Data.Read(_t4, 0, 4);

                if (BitConverter.ToInt32(_t4, 0) != 0x7fffffff)
                {
                    if (_startempty + 31 >= _filelen)
                    {
                        return false;
                    }
                    Data.Seek(_startempty, SeekOrigin.Begin);
                    Data.Read(_t, 0, 31);
                    if (_t[0] != 0xd || _t[1] != 0xa || _t[10] != 0x20 || _t[19] != 0x20 || _t[28] != 0x20 || _t[29] != 0xd || _t[30] != 0xa)
                    {
                        ISCatalog = FileIsCatalog.iscatalog_false;
                        return false;
                    }
                }
                if (_filelen < 31 + 16)
                {
                    ISCatalog = FileIsCatalog.iscatalog_false;
                    return false;
                }
                Data.Seek(16, SeekOrigin.Begin);
                Data.Read(_t, 0, 31);
                if (_t[0] != 0xd || _t[1] != 0xa || _t[10] != 0x20 || _t[19] != 0x20 || _t[28] != 0x20 || _t[29] != 0xd || _t[30] != 0xa)
                {
                    ISCatalog = FileIsCatalog.iscatalog_false;
                    return false;
                }
                ISCatalog = FileIsCatalog.iscatalog_true;
                return true;

            }
            return ISCatalog == FileIsCatalog.iscatalog_true;
        }

        public V8Catalog GetCatalog()
        {
            V8Catalog ret = null;

            if (IsCatalog())
            {
                if (Self != null)
                {
                    Self = new V8Catalog(this);
                }
                ret = Self;

            }
            else
                ret = null;

            return ret;
        }

        public int GetFileLength()
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;

            ret = (int)Data.Length;

            return ret;
        }

        public int Read(byte[] Buffer, int Start, int Length)
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;
            Data.Seek(Start, SeekOrigin.Begin);
            ret = Data.Read(Buffer, 0, Length);
            return ret;
        }

        public int Write(byte[] Buffer, int Start, int Length) // дозапись/перезапись частично
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;
            //setCurrentTime(&time_modify);
            IsHeaderModified = true;
            IsDataModified = true;
            Data.Seek(Start, SeekOrigin.Begin);
            Data.Write(Buffer, 0, Length);
            ret = (int)Data.Position;

            return ret;
        }

        public int Write(byte[] Buffer, int Length) // перезапись целиком
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;
            //setCurrentTime(&time_modify);
            IsHeaderModified = true;
            IsDataModified = true;
            if (Data.Length > Length)
            {
                // TODO: Надо проконтролировать
                // пока непонятно что с этим делать...
                //Data.Length = Length;
                Data.SetLength(Length);
            }
            Data.Seek(0, SeekOrigin.Begin);
            Data.Write(Buffer, 0, Length);
            ret = (int)Data.Position;

            return ret;
        }

        public int Write(Stream _Stream, int Start, int Length) // дозапись/перезапись частично
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;
            //setCurrentTime(&time_modify);
            IsHeaderModified = true;
            IsDataModified = true;
            if (Data.Length > Length)
            {
                // TODO: Надо проконтролировать
                // пока непонятно что с этим делать...
                //Data.Length = Length;
                Data.SetLength(Length);
            }
            Data.Seek(0, SeekOrigin.Begin);
            _Stream.CopyTo(Data, Length);
            //Data.Write(Buffer, 0, Length);
            ret = (int)Data.Position;

            return ret;
        }

        public int Write(Stream _Stream) // перезапись целиком
        {
            int ret = 0;

            if (!IsOpened)
                if (!Open())
                    return ret;
            //setCurrentTime(&time_modify);
            IsHeaderModified = true;
            IsDataModified = true;
            if (Data.Length > _Stream.Length)
            {
                // TODO: Надо проконтролировать
                // пока непонятно что с этим делать...
                //Data.Length = Length;

                Data.SetLength(_Stream.Length);
            }
            Data.Seek(0, SeekOrigin.Begin);
            //_Stream.CopyTo(Data, Length);
            _Stream.CopyTo(Data);
            //Data.Write(Buffer, 0, Length);
            ret = (int)Data.Position;

            return ret;
        }

        public String GetFileName()
        {
            return Name;
        }

        public String GetFullName()
        {
            if (Parent != null)
            {
                if (Parent.File != null)
                {
                    string fulln = Parent.File.GetFullName();
                    if (!string.IsNullOrEmpty(fulln))
                    {
                        fulln += "\\";
                        fulln += Name;
                        return fulln;
                    }
                }
            }
            return Name;
        }

        public void SetFileName(String _name)
        {
            Name = _name;
            IsHeaderModified = true;
        }

        public V8Catalog GetParentCatalog()
        {
            return Parent;
        }

        public void DeleteFile()
        {
            if (Parent != null)
            {
                if (Next != null)
                {
                    Next.Previous = Previous;
                }
                else
                {
                    Parent.Last = Previous;
                }

                if (Previous != null)
                {
                    Previous.Next = Next;
                }
                else
                {
                    Parent.First = Next;
                }

                Parent.is_fatmodified = true;
                Parent.FreeBlock(StartData);
                Parent.FreeBlock(StartHeader);
                Parent.Files.Remove(Name.ToUpper());
                Parent = null;
            }
            Data = null;
            if (Self != null)
            {
                Self.Data = null;
                Self = null;
            }
            ISCatalog = FileIsCatalog.iscatalog_false;
            Next = null;
            Previous = null;
            IsOpened = false;
            StartData = 0;
            StartHeader = 0;
            IsDataModified = false;
            IsHeaderModified = false;

        }

        public V8File GetNext()
        {
            return Next;
        }

        public bool Open()
        {
            if (Parent == null)
                return false;
            if (IsOpened)
            {
                return true;
            }
            Data = Parent.ReadDataBlock(StartData);
            IsOpened = true;
            return true;
        }

        public void Close()
        {
            int _t = 0;

            if (Parent == null) return;

            if (!IsOpened) return;

            if (Self != null)
            {
                if (Self.is_destructed)
                    Self = null;
            }
            Self = null;

            if (Parent.Data != null)
            {
                if (IsDataModified || IsHeaderModified)
                {
                    if (IsDataModified)
                    {
                        StartData = Parent.WriteDataBlock(Data, StartData, SelfZipped);
                    }
                    if (IsHeaderModified)
                    {
                        MemoryStream hs = new MemoryStream();
                        hs.Write(BitConverter.GetBytes(TimeCreate.Ticks), 0, 8);
                        hs.Write(BitConverter.GetBytes(TimeModify.Ticks), 0, 8);
                        hs.Write(BitConverter.GetBytes(_t), 0, 4);
                        hs.Write(Encoding.UTF8.GetBytes(Name), 0, Name.Length);
                        hs.Write(BitConverter.GetBytes(_t), 0, 4);

                        StartHeader = Parent.WriteBlock(hs, StartHeader, false);
                        hs.Dispose();
                    }

                }
            }
            Data.Dispose();
            Data = null;
            ISCatalog = FileIsCatalog.iscatalog_false;
            IsOpened = false;
            IsDataModified = false;
            IsHeaderModified = false;
        }

        public int WriteAndClose(Stream _Stream, int Length = -1) // перезапись целиком и закрытие файла (для экономии памяти не используется data файла)
        {
            int _t = 0;

            if (!IsOpened)
                if (!Open())
                    return 0;

            if (Parent == null)
                return 0;

            if (Self != null)
                Self = null;

            Data.Dispose();
            Data = null;

            if (Parent.Data != null)
            {
                StartData = Parent.WriteDataBlock(_Stream, StartData, SelfZipped, Length);
                MemoryStream hs = new MemoryStream();
                hs.Write(BitConverter.GetBytes(TimeCreate.Ticks), 0, 8);
                hs.Write(BitConverter.GetBytes(TimeModify.Ticks), 0, 8);
                hs.Write(BitConverter.GetBytes(_t), 0, 4);
                hs.Write(Encoding.UTF8.GetBytes(Name), 0, Name.Length);
                hs.Write(BitConverter.GetBytes(_t), 0, 4);

                StartHeader = Parent.WriteBlock(hs, StartHeader, false);
                hs.Dispose();
            }
            ISCatalog = FileIsCatalog.iscatalog_unknown;
            IsOpened = false;
            IsDataModified = false;
            IsHeaderModified = false;

            if (Length == -1)
                return (int)_Stream.Length;

            return Length;
        }

        public void GetTimeCreate(FILETIME ft)
        {
            V8timeToFileTime(TimeCreate.Ticks, ref ft);
        }

        public void GetTimeModify(FILETIME ft)
        {
            V8timeToFileTime(TimeModify.Ticks, ref ft);
        }

        public void SetTimeCreate(FILETIME ft)
        {
            long v8t = 0;
            FileTimeToV8time(ft, ref v8t);
            TimeCreate = new DateTime(v8t);
        }

        public void SetTimeModify(FILETIME ft)
        {
            long v8t = 0;
            FileTimeToV8time(ft, ref v8t);
            TimeModify = new DateTime(v8t);
        }

        public void SaveToFile(String FileName)
        {
            if (!IsOpened)
                if (!Open())
                    return;
            FileStream fs = new FileStream(FileName, FileMode.Create);
            Data.CopyTo(fs);
            fs.Close();
            fs.Dispose();
        }

        public void SaveToStream(Stream _Stream)
        {
            if (!IsOpened)
                if (!Open())
                    return;
            Data.CopyTo(_Stream);

        }

        public Stream GetData()
        {
            return Data;
        }

        public void Flush()
        {
            int _t  =  0;
            if (Flushed)
            {
                return;
            }
            if (Parent == null)
                return;
            if (!IsOpened)
                return;
            Flushed = true;
            if (Self != null)
                Self.Flush();

            if (Parent.Data != null)
            {
                if (IsDataModified || IsHeaderModified)
                {
                    if (IsDataModified)
                    {
                        StartData = Parent.WriteDataBlock(Data, StartData, SelfZipped);
                        IsDataModified = false;
                    }
                    if (IsHeaderModified)
                    {
                        MemoryStream hs = new MemoryStream();
                        hs.Write(BitConverter.GetBytes(TimeCreate.Ticks), 0, 8);
                        hs.Write(BitConverter.GetBytes(TimeModify.Ticks), 0, 8);
                        hs.Write(BitConverter.GetBytes(_t), 0, 4);
                        hs.Write(Encoding.UTF8.GetBytes(Name), 0, Name.Length);
                        hs.Write(BitConverter.GetBytes(_t), 0, 4);

                        StartHeader = Parent.WriteBlock(hs, StartHeader, false);
                        hs.Close();
                        hs.Dispose();
                        IsHeaderModified = false;
                    }
                }
            }
            Flushed = false;
        }


    }
}
