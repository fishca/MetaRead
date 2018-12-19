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

        public V8Catalog(V8File f) // создать каталог из файла
        { }

        public V8Catalog(String name) // создать каталог из физического файла (cf, epf, erf, hbk, cfu)
        { }

        public V8Catalog(String name, bool _zipped) // создать каталог из физического файла (cf, epf, erf, hbk, cfu)
        { }

        public bool IsCatalog()
        { return true; }

        public V8Catalog(Stream _Stream, bool _zipped, bool leave_stream = false) // создать каталог из потока
        { }

        public V8File GetFile(String FileName)
        { return null; }

        public V8File GetFirst()
        { return null; }

        public V8File CreateFile(String FileName, bool _selfzipped = false) // CreateFile в win64 определяется как CreateFileW, пришлось заменить на маленькую букву
        { return null; }

        public V8Catalog CreateCatalog(String FileName, bool _selfzipped = false)
        { return null; }

        public void DeleteFile(String FileName)
        { }

        public V8Catalog GetParentCatalog()
        { return null; }

        public V8File GetSelfFile()
        { return null; }

        public void SaveToDir(String DirName) { }
        public bool IsOpen() { return true; }

        public void Flush()
        {
            FAT_Item fi = new FAT_Item();
            

            if (flushed)
            {
                return;
            }
            flushed = true;

            V8File f = First;
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
                    fi.ff = 0x7fffffff;
                    f = First;
                    while (f != null)
                    {
                        fi.Header_Start = f.StartHeader;
                        fi.Data_Start = f.StartData;
                        fat = WriteFatItemToStream(fi);
                        f = f.Next;
                    }
                    WriteBlock(fat, 16, true);
                    is_fatmodified = false;
                }
                if (is_emptymodified)
                {
                    Data.Seek(0, SeekOrigin.Begin);
                }
            }

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
