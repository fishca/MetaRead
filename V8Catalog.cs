using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    public class V8Catalog
    {
        public V8File File; // файл, которым является каталог. Для корневого каталога NULL
        public Stream Data; // поток каталога. Если file не NULL (каталог не корневой), совпадает с file->data
        public Stream CFu;  // поток файла cfu. Существует только при is_cfu == true
        
        public V8File First; // первый файл в каталоге
        public V8File Last;  // последний файл в каталоге
        
        //public std::map<String, v8file*> files; // Соответствие имен и файлов
        public SortedDictionary<String, V8File> Files; // Соответствие имен и файлов

        public int start_empty;       // начало первого пустого блока
        public int page_size;         // размер страницы по умолчанию
        public int version;           // версия
        public bool zipped;           // признак зазипованности файлов каталога
        public bool is_cfu;           // признак файла cfu (файл запакован deflate'ом)
        public bool iscatalog;        // это признак каталога
        public bool iscatalogdefined; // это признак определения каталога


        public bool is_fatmodified;    // признак модифицированности таблицы размещения
        public bool is_emptymodified;  // признак модифицированности таблицы свободных блоков
        public bool is_modified;       // признак модифицированности

        /// <summary>
        /// Инициализация объекта 
        /// </summary>
        public void Initialize()
        {
            is_destructed = false;
            Catalog_Header _ch = new Catalog_Header();
            int _temp = 0;
            string _name = "";
            FAT_Item _fi;
            //char* _temp_buf;
            //byte[] _temp_buf = new byte[32];

            MemoryStream _file_header = new MemoryStream();
            Stream _fat  = null;
            V8File _prev = null; 
            V8File _file = null;
            V8File f     = null;
            int _countfiles = 0;

            Data.Seek(0, SeekOrigin.Begin);

            //Data.Read(&_ch, 16);
            _ch = ReadFromData(Data);

            start_empty = _ch.Start_Empty;
            page_size   = _ch.Page_Size;
            version     = _ch.Version;

            First = null;

            _prev = null;

            try
            {
                if (Data.Length > 16)
                {
                    _fat = Read_Block(Data, 16);
                    _fat.Seek(0, SeekOrigin.Begin);
                    _countfiles = (int)_fat.Length / 12;
                    for (int i = 0; i < _countfiles; i++)
                    {
                        _fi = ReadFatItemFromData(Data);

                        Read_Block(Data, _fi.Header_Start, _file_header);

                        _file_header.Seek(0, SeekOrigin.Begin);

                        byte[] _temp_buf = new byte[_file_header.Length];

                        // TODO: Надо пристально проверять что здесь происходит
                        // _name = (wchar_t*)(_temp_buf + 20);
                        _name = GetString(_temp_buf)[20].ToString();
                        // TODO: Надо пристально проверять что здесь происходит
                        //_file = new v8file(this, _name, _prev, _fi.data_start, _fi.header_start, (__int64*)_temp_buf, (__int64*)(_temp_buf + 8));
                        _file = new V8File(this, _name, _prev, _fi.Data_Start, _fi.Header_Start, new DateTime(), new DateTime());
                        if (_prev == null)
                            First = _file;
                        _prev = _file;
                    }
                    _file_header.Close();
                    _fat.Close();
                }
            }
            catch (Exception)
            {
                f = First;
                while (f != null)
                {
                    f.Close();
                    f = f.Next;
                }
                while (First != null)
                {
                    First.Close();
                }
                iscatalog = false;
                iscatalogdefined = true;

                First = null;
                Last  = null;
                start_empty = 0;
                page_size   = 0;
                version     = 0;
                zipped = false;
            }
            Last = _prev;

            is_fatmodified   = false;
            is_emptymodified = false;
            is_modified      = false;
            is_destructed    = false;
            flushed          = false;
            leave_data       = false;
        }

        // TODO: не забыть доделать
        public void FreeBlock(int start)
        {
            //char temp_buf[32];
            byte[] temp_buf = new byte[32];

            int nextstart = 0;
            int prevempty = 0;

            if (start == 0)
                return;
            if (start == 0x7fffffff)
                return;
            
            prevempty   = start_empty;
            start_empty = start;

            do
            {
                Data.Seek(0, SeekOrigin.Begin);
                Data.Read(temp_buf, 0, 31);

                // TODO: Надо проконтролировать
                // nextstart = hex_to_int(&temp_buf[20]);
                //curlen = hex_to_int(GetString(temp_buf).Substring(20, 20));
                nextstart = hex_to_int(GetString(temp_buf).Substring(20, 20));

            } while (start != 0x7fffffff);
        }

        public int WriteBlock(Stream block, int start, bool use_page_size, int len = -1)  // возвращает адрес начала блока
        { return 0; }

        public int WriteDataBlock(Stream block, int start, bool _zipped = false, int len = -1) // возвращает адрес начала блока
        { return 0; }

        public Stream ReadDataBlock(int start)
        { return null; }

        public int GetNextBlock(int start)
        { return 0; }

        public bool is_destructed; // признак, что работает деструктор
        public bool flushed; // признак, что происходит сброс
        public bool leave_data; // признак, что не нужно удалять основной поток (data) при уничтожении объекта

        #region Конструкторы класса
        /// <summary>
        /// создать каталог из файла
        /// </summary>
        /// <param name="f"></param>
        public V8Catalog(V8File f) // создать каталог из файла
        {
            is_cfu = false;
            iscatalogdefined = false;
            File = f;

            File.Open();
            Data = File.Data;
            zipped = false;

            if (IsCatalog()) Initialize();
            else
            {
                First = null;
                Last = null;
                start_empty = 0;
                page_size = 0;
                version = 0;
                zipped = false;

                is_fatmodified = false;
                is_emptymodified = false;
                is_modified = false;
                is_destructed = false;
                flushed = false;
                leave_data = false;
            }

        }

        /// <summary>
        /// создать каталог из физического файла (cf, epf, erf, hbk, cfu)
        /// </summary>
        /// <param name="name"></param>
        public V8Catalog(String name) // создать каталог из физического файла (cf, epf, erf, hbk, cfu)
        {
            iscatalogdefined = false;

            String ext = Path.GetExtension(name).ToLower();
            if (ext == str_cfu)
            {
                is_cfu = true;
                zipped = false;
                //data = new MemoryStream();
                Data = new MemoryTributary();
                if (!System.IO.File.Exists(name))
                {
                    //data.WriteBuffer(_EMPTY_CATALOG_TEMPLATE, CATALOG_HEADER_LEN);
                    Data.Write(StringToByteArray(_EMPTY_CATALOG_TEMPLATE), 0, CATALOG_HEADER_LEN2);
                    CFu = new FileStream(name, FileMode.Create);
                }
                else
                {
                    CFu = new FileStream(name, FileMode.Append);
                    // Inflate((MemoryTributary)cfu, out data); TODO надо дорабатывать обязательно
                }
            }
            else
            {
                zipped = ext == str_cf || ext == str_epf || ext == str_erf || ext == str_cfe;
                is_cfu = false;

                if (!System.IO.File.Exists(name))
                {
                    FileStream data1 = new FileStream(name, FileMode.Create);
                    data1.Write(StringToByteArray(_EMPTY_CATALOG_TEMPLATE), 0, CATALOG_HEADER_LEN2);
                    //data1 = null;
                    data1.Dispose();
                }
                Data = new FileStream(name, FileMode.Append);
            }

            File = null;
            if (IsCatalog()) Initialize();
            else
            {
                First = null;
                Last = null;
                start_empty = 0;
                page_size = 0;
                version = 0;
                zipped = false;

                is_fatmodified = false;
                is_emptymodified = false;
                is_modified = false;
                is_destructed = false;
                flushed = false;
                leave_data = false;
            }

            CFu.Dispose();
            Data.Dispose();
        }

        public V8Catalog(String name, bool _zipped) // создать каталог из физического файла (cf, epf, erf, hbk, cfu)
        {
            iscatalogdefined = false;
            is_cfu = false;
            zipped = _zipped;

            if (!System.IO.File.Exists(name))
            {
                FileStream data = new FileStream(name, FileMode.Create);
                data.Write(StringToByteArray(_EMPTY_CATALOG_TEMPLATE), 0, CATALOG_HEADER_LEN2);
                data.Dispose();
            }
            Data = new FileStream(name, FileMode.Append);
            File = null;
            if (IsCatalog()) Initialize();
            else
            {
                First = null;
                Last = null;
                start_empty = 0;
                page_size = 0;
                version = 0;
                zipped = false;

                is_fatmodified = false;
                is_emptymodified = false;
                is_modified = false;
                is_destructed = false;
                flushed = false;
                leave_data = false;
            }

            Data.Dispose();

        }

        /// <summary>
        /// Создать каталог из потока
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="_zipped"></param>
        /// <param name="leave_stream"></param>
        public V8Catalog(Stream stream, bool _zipped, bool leave_stream = false)
        {

            is_cfu = false;
            iscatalogdefined = false;
            zipped = _zipped;
            Data = stream;
            File = null;

            if (Data.Length != 0)
                Data.Write(StringToByteArray(_EMPTY_CATALOG_TEMPLATE), 0, CATALOG_HEADER_LEN2);

            if (IsCatalog())
                Initialize();
            else
            {
                First = null;
                Last = null;
                start_empty = 0;
                page_size = 0;
                version = 0;
                zipped = false;

                is_fatmodified = false;
                is_emptymodified = false;
                is_modified = false;
                is_destructed = false;
                flushed = false;
            }
            leave_data = leave_stream;
        } // создать каталог из потока
        #endregion

        public bool IsCatalog()
        {
            Int64 _filelen;
            Int32 _startempty = (-1);
            //char _t[BLOCK_HEADER_LEN];
            Byte[] _t = new Byte[BLOCK_HEADER_LEN];

            if (iscatalogdefined)
            {

                return iscatalog;
            }
            iscatalogdefined = true;
            iscatalog = false;

            // эмпирический метод?
            _filelen = Data.Length;
            if (_filelen == CATALOG_HEADER_LEN2)
            {
                Data.Seek(0, SeekOrigin.Begin);
                Data.Read(_t, 0, CATALOG_HEADER_LEN2);
                //if (memcmp(_t, _EMPTY_CATALOG_TEMPLATE, CATALOG_HEADER_LEN) != 0)
                if (!_t.ToString().StartsWith(_EMPTY_CATALOG_TEMPLATE))
                {

                    return false;
                }
                else
                {
                    iscatalog = true;

                    return true;
                }
            }

            Data.Seek(0, SeekOrigin.Begin);
            //data->Read(&_startempty, 4); TODO: ХЗ что с этим делать
            if (_startempty != LAST_BLOCK)
            {
                if (_startempty + 31 >= _filelen)
                {

                    return false;
                }
                Data.Seek(0, SeekOrigin.Begin);
                Data.Read(_t, 0, 31);
                if (_t[0] != 0xd || _t[1] != 0xa || _t[10] != 0x20 || _t[19] != 0x20 || _t[28] != 0x20 || _t[29] != 0xd || _t[30] != 0xa)
                {

                    return false;
                }
            }
            if (_filelen < (BLOCK_HEADER_LEN - 1 + CATALOG_HEADER_LEN))
            {

                return false;
            }

            Data.Seek(CATALOG_HEADER_LEN, SeekOrigin.Begin);
            Data.Read(_t, 0, 31);
            if (_t[0] != 0xd || _t[1] != 0xa || _t[10] != 0x20 || _t[19] != 0x20 || _t[28] != 0x20 || _t[29] != 0xd || _t[30] != 0xa)
            {

                return false;
            }
            iscatalog = true;

            return true;
        }

        public V8File GetFile(String FileName)
        {
            V8File ret = null;

            foreach (KeyValuePair<String, V8File> kvp in Files)
            {
                if (kvp.Key.Equals(FileName))
                {
                    ret = kvp.Value;
                    break;
                }
                else
                    ret = null;
            }
            return ret;
        }

        public V8File GetFirst()
        {
            return First;
        }

        public V8File CreateFile(String FileName, bool _selfzipped = false) // CreateFile в win64 определяется как CreateFileW, пришлось заменить на маленькую букву
        {
            Int64 v8t = 0;
            V8File f;

            f = GetFile(FileName);
            if (f != null)
            {
                SetCurrentTime(v8t);
                f = new V8File(this, FileName, Last, 0, 0,  new DateTime(v8t), new DateTime(v8t));
                f.SelfZipped = _selfzipped;
                Last = f;
                is_fatmodified = true;
            }

            return f;
        }

        public V8Catalog CreateCatalog(String FileName, bool _selfzipped = false)
        {
            V8Catalog ret;

            V8File f = CreateFile(FileName, _selfzipped);
            if (f.GetFileLength() > 0)
            {
                if (f.IsCatalog()) ret = f.GetCatalog();
                else ret = null;
            }
            else
            {
                f.Write(Encoding.UTF8.GetBytes(_EMPTY_CATALOG_TEMPLATE), CATALOG_HEADER_LEN2);
                ret = f.GetCatalog();
            }

            return ret;
        }

        public void DeleteFile(String FileName)
        {
            V8File f = First;
            while (f != null)
            {
                //if (!f.name.CompareIC(FileName))
                if (String.Compare(f.Name, FileName, true) != 0)
                {
                    f.DeleteFile();
                    f = null;
                }

                if (f != null)
                    f = f.Next;
            }
        }

        public V8Catalog GetParentCatalog()
        {
            return File?.Parent;
        }

        public V8File GetSelfFile()
        {
            return File;
        }

        public void SaveToDir(String DirName)
        {
            V8File f = First;

            DirectoryInfo di = new DirectoryInfo(DirName);

            di.Create();

            if (!DirName.EndsWith("\\"))
                DirName += '\\';

            while (f != null)
            {
                if (f.IsCatalog())
                    f.GetCatalog().SaveToDir(DirName + f.Name);
                else
                    f.SaveToFile(DirName + f.Name);
                f.Close();
                f = f.Next;
            }

        }

        public bool IsOpen()
        {
            return IsCatalog();
        }

        public void Flush()
        {
            FAT_Item fi = new FAT_Item();
            V8File f;


            if (flushed)
            {
                return;
            }

            flushed = true;

            f = First;
            while (f != null)
            {
                f.Flush();
                f = f.Next;
            }

            if (Data != null)
            {
                if (is_fatmodified)
                {
                    MemoryStream fat = new MemoryStream();
                    fi.ff = (int)LAST_BLOCK2;
                    f = First;
                    while (f != null)
                    {
                        
                        fi.Header_Start = f.StartHeader;
                        fi.Data_Start   = f.StartData;
                        //fat.Write(fi, 0, 12); TODO: Подумать
                        f = f.Next;
                    }
                    WriteBlock(fat, CATALOG_HEADER_LEN2, true);
                    is_fatmodified = false;
                }

                if (is_emptymodified)
                {
                    Data.Seek(0, SeekOrigin.Begin);
                    // data.Write(start_empty, 0, 4); TODO: Подумать
                    is_emptymodified = false;
                }
                if (is_modified)
                {
                    version++;
                    Data.Seek(0, SeekOrigin.Begin);
                    // data.Write(version, 0, 4); TODO: Подумать
                }
            }

            if (File != null)
            {
                if (is_modified)
                {
                    File.IsDataModified = true;
                }
                File.Flush();
            }
            else
            {
                if (is_cfu)
                {
                    if (Data != null && CFu != null && is_modified)
                    {
                        Data.Seek(0, SeekOrigin.Begin);
                        CFu.Seek(0, SeekOrigin.Begin);

                        //ZDeflateStream(data, cfu);
                        // Deflate(data, out cfu); TODO: Додумать
                    }
                }
            }

            is_modified = false;
            flushed = false;

        }

        public void HalfClose()
        {
            Flush();
            if (is_cfu)
            {
                CFu.Close();
                CFu = null;
            }
            else
            {
                Data.Dispose();
                Data = null;
            }
        }

        public void HalfOpen(string name)
        {
            if (is_cfu)
                CFu = new FileStream(name, FileMode.OpenOrCreate);
            else
                Data = new FileStream(name, FileMode.OpenOrCreate);
        }

    }
}
