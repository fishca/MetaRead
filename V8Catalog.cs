using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MetaRead
{
    public class V8Catalog
    {
        public V8File File; // файл, которым является каталог. Для корневого каталога NULL
        public Stream Data; // поток каталога. Если file не NULL (каталог не корневой), совпадает с file->data
        public Stream CFu; // поток файла cfu. Существует только при is_cfu == true
        public void Initialize() { }
        public V8File First; // первый файл в каталоге
        public V8File Last; // последний файл в каталоге
        //public std::map<String, v8file*> files; // Соответствие имен и файлов
        public SortedDictionary<String, V8File> Files; // Соответствие имен и файлов
        public int start_empty; // начало первого пустого блока
        public int page_size; // размер страницы по умолчанию
        public int version; // версия
        public bool zipped; // признак зазипованности файлов каталога
        public bool is_cfu; // признак файла cfu (файл запакован deflate'ом)
        public bool iscatalog;
        public bool iscatalogdefined;

        public bool is_fatmodified;
        public bool is_emptymodified;
        public bool is_modified;

        public void FreeBlock(int start) { }

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
        public void Flush() { }
        public void HalfClose() { }
        public void HalfOpen(String name) { }

    }
}
