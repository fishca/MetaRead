using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.Structures;
using static MetaRead.APIcfBase;
using static MetaRead.Constants;

namespace MetaRead
{
    /// <summary>
    /// Структура открытого файла адаптера контейнера конфигурации
    /// </summary>
    public class ConfigFile
    {
        public Stream str;
        public Object addin; // Не очень понятно что это такое...
        public ConfigFile()
        {
            str = null;
            addin = null;
        }
    }

    /// <summary>
    /// Структура файла контейнера файлов
    /// </summary>
    public class Container_file
    {
        private TableFile file;

        private string name;    // Приведенное имя (очищенное от динамического обновления)
        public String fname;    // Имя временого файла, содержащего stream
        public String rfname;   // Имя временого файла, содержащего rstream

        private Stream stream;  // stream (нераспакованный поток)
        private Stream rstream; // raw stream (нераспакованный поток)

        private V8Catalog cat;

        private table_file_packed packed;

        /// <summary>
        /// Номер (индекс) динамического обновления (0, 1 и т.д.). 
        /// 
        /// Если без динамического обновления, то -1, 
        /// если UID динамического обновления не найден, то -2. 
        /// 
        /// Для пропускаемых файлов -3.
        /// </summary>
        private int dynno;

        public TableFile File { get { return file; } set { file = value; } }

        public string FName { get { return fname; } set { fname = value; } }

        public string RFName { get { return rfname; } set { rfname = value; } }

        public string Name { get { return name; } set { name = value; } }

        public Stream Stream { get { return stream; } set { stream = value; } }

        public Stream Rstream { get { return rstream; } set { rstream = value; } }

        public V8Catalog Cat { get { return cat; } set { cat = value; } }

        public table_file_packed Packed { get { return packed; } set { packed = value; } }

        public int Dynno { get { return dynno; } set { dynno = value; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_f"></param>
        /// <param name="_name"></param>
        public Container_file(TableFile _f, string _name)
        {
            File = _f;
            Name = _name;
            Stream = null;
            Rstream = null;
            Cat = null;
            Packed = table_file_packed.unknown;
            Dynno = -3;
        }

        public bool open()
        {

            const int MAX_PATH = 260;
            Stream ts;
            string tn = "";
            char[] tempfile = new char[MAX_PATH];
            int i;
            V8Table t;
            //table_blob_file addr; // TODO: возможно это должен быть МАССИВ !!!!!!!!!!!!!!!!!!!!!!!!
            table_blob_file[] addr; // TODO: возможно это должен быть МАССИВ !!!!!!!!!!!!!!!!!!!!!!!!
            uint maxpartno;

            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                return true;
            }
            t = file.T;
            addr = file.addr;
            maxpartno = file.maxpartno;

            if (maxpartno > 0)
            {
                tn = Path.GetTempFileName();
                stream = new FileStream(tn, FileMode.CreateNew);
                fname = tn;
            }
            else
            {
                stream = new MemoryStream();
            }

            if (packed == table_file_packed.unknown)
                packed = isPacked() ? table_file_packed.yes : table_file_packed.no;

            if (rstream != null)
            {
                if (packed == table_file_packed.yes)
                    ts = rstream;
                else
                {
                    stream = rstream;
                    stream.Seek(0, SeekOrigin.Begin);
                    return true;
                }
            }
            else
            {
                if (packed == table_file_packed.yes)
                {
                    if (maxpartno > 0)
                    {
                        //GetTempFileName(temppath, L"awa", 0, tempfile);
                        //tn = tempfile;
                        //ts = new TFileStream(tn, fmCreate);
                        tn = Path.GetTempFileName();
                        ts = new FileStream(tn, FileMode.CreateNew);
                        fname = tn;

                    }
                    else
                        ts = new MemoryStream();
                }
                else
                    ts = stream;

                for (i = 0; i <= maxpartno; ++i)
                    t.ReadBlob(ts, addr[i].Blob_start, addr[i].Blob_length, false);
            }
            if (packed == table_file_packed.yes)
            {
                ts.Seek(0, SeekOrigin.Begin);
                MemoryTributary out_stream = new MemoryTributary();
                Inflate((MemoryTributary)ts, out out_stream);
                out_stream.CopyTo(stream);
                if (rstream is null)
                {
                    ts.Dispose();
                    System.IO.File.Delete(tn);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        /// <summary>
        /// raw open
        /// </summary>
        /// <returns></returns>
        public bool ropen()
        {
            int i;
            V8Table t;
            table_blob_file[] addr;
            uint maxpartno;
            string tn;

            if (rstream != null)
            {
                rstream.Seek(0, SeekOrigin.Begin);
                return true;
            }

            t = file.T;
            addr = file.addr;
            maxpartno = file.maxpartno;
            if (packed == table_file_packed.unknown)
                packed = isPacked() ? table_file_packed.yes : table_file_packed.no;
            if (packed == table_file_packed.no && stream != null)
            {
                rstream = stream;
                rstream.Seek(0, SeekOrigin.Begin);
                return true;
            }

            if (maxpartno > 0)
            {
                tn = Path.GetTempFileName();
                rstream = new FileStream(tn, FileMode.CreateNew);
                rfname = tn;
            }
            else
            {
                rstream = new MemoryStream();
            }

            for (i = 0; i <= maxpartno; ++i)
                t.ReadBlob(rstream, addr[i].Blob_start, addr[i].Blob_length, false);

            rstream.Seek(0, SeekOrigin.Begin);

            return true;
        }

        public void close()
        {
            cat = null;
            if (stream != rstream)
            {
                stream.Dispose();
                rstream.Dispose();
            }
            else
                stream.Dispose();
            stream = null;
            rstream = null;
            if (!string.IsNullOrEmpty(fname))
                System.IO.File.Delete(fname);
            if (!string.IsNullOrEmpty(rfname))
                System.IO.File.Delete(rfname);
            fname = "";
            rfname = "";
        }

        public bool isPacked()
        {
            int i;
            string ext;

            if (name.CompareTo("DynamicallyUpdated") == 0)
                return false;
            if (name.CompareTo("SprScndInfo") == 0)
                return false;
            if (name.CompareTo("DBNamesVersion") == 0)
                return false;
            if (name.CompareTo("siVersions") == 0)
                return false;
            if (name.ToUpper().IndexOf("FO_VERSION") != -1)
                return false;
            if (name[1] == '{')
                return false;

            string delimiters = "\\/.,";

            i = name.LastIndexOfAny(delimiters.ToCharArray());
            if (i != -1)
            {
                ext = name.Substring(i + 1, name.Length - i).ToUpper();
                if (ext.CompareTo("INF") == 0)
                    return false;
                if (ext.CompareTo("PFL") == 0)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Базовый класс адаптеров контейнеров конфигурации
    /// </summary>
    public class ConfigStorage
    {

        public ConfigStorage() { }

        // Если файл не существует, возвращается NULL
        public virtual ConfigFile readfile(String path)
        {
            return new ConfigFile();
        }
        public virtual bool writefile(String path, Stream str) { return true; }
        public virtual String presentation() { return " "; }
        public virtual void close(ConfigFile cf) { }
        public virtual bool fileexists(String path) { return true; }
    }

    /// <summary>
    /// Класс адаптера контейнера конфигурации - Директория
    /// </summary>
    public class ConfigStorageDirectory : ConfigStorage
    {

        private string fdir;

        public ConfigStorageDirectory(string _dir)
        {
            fdir = _dir;
            if (fdir.LastIndexOf('\\') != fdir.Length - 1)
                fdir += '\\';
        }

        public override ConfigFile readfile(string path)
        {
            ConfigFile cf;
            string filename;

            filename = fdir + new StringBuilder(path).Replace('/', '\\').ToString();

            if (File.Exists(filename))
            {
                cf = new ConfigFile();
                try
                {
                    cf.str = new FileStream(filename, FileMode.CreateNew);
                    cf.addin = null;
                    return cf;
                }
                catch (Exception)
                {

                    return null;
                    //throw;
                }
            }
            else
                return null;
        }

        public override bool writefile(string path, Stream str)
        {
            string filename = fdir + new StringBuilder(path).Replace('/', '\\').ToString();

            FileStream f = new FileStream(filename, FileMode.CreateNew);
            str.CopyTo(f);
            f.Dispose();

            return true;
        }

        public override string presentation()
        {
            return fdir.Substring(1, fdir.Length - 1);
        }

        public override void close(ConfigFile cf)
        {
            cf.str.Dispose();
        }

        public override bool fileexists(string path)
        {
            string filename = fdir + new StringBuilder(path).Replace('/', '\\').ToString();
            return File.Exists(filename);
        }

    }

    /// <summary>
    /// Класс адаптера контейнера конфигурации - cf (epf, erf, cfe) файл
    /// </summary>
    public class ConfigStorageCFFile : ConfigStorage
    {

        private string filename;
        private V8Catalog cat;

        public ConfigStorageCFFile(string fname)
        {
            filename = fname;
            cat = new V8Catalog(filename);
        }

        public override ConfigFile readfile(string path)
        {
            V8Catalog c;
            V8File f;
            int i;

            ConfigFile cf;

            if (!cat.IsOpen())
                return null;

            string fname = new StringBuilder(path).Replace('/', '\\').ToString();
            c = cat;
            for (i = fname.IndexOf("\\"); i != -1; i = fname.IndexOf("\\"))
            {
                f = c.GetFile(fname.Substring(1, i - 1));
                if (f is null)
                    return null;
                c = f.GetCatalog();
                if (c is null)
                    return null;
                fname = fname.Substring(i + 1, fname.Length - i);
            }
            f = c.GetFile(fname);
            if (f is null)
                return null;
            if (f.Open())
                return null;
            cf = new ConfigFile();
            cf.str = f.GetData();
            cf.str.Seek(0, SeekOrigin.Begin);
            cf.addin = f;

            return cf;
        }

        public override bool writefile(string path, Stream str)
        {
            V8Catalog c;
            V8File f;
            int i;

            if (!cat.IsOpen())
                return false;

            string fname = new StringBuilder(path).Replace('/', '\\').ToString();
            c = cat;
            for (i = fname.IndexOf("\\"); i != -1; i = fname.IndexOf("\\"))
            {
                c = c.CreateCatalog(fname.Substring(1, i - 1));
                fname = fname.Substring(i + 1, fname.Length - 1);
            }
            f = c.CreateFile(fname);
            f.Write(str);

            return true;
        }

        public override string presentation()
        {
            return filename;
        }

        public override void close(ConfigFile cf)
        {
            V8File f;
            f = (V8File)cf.addin;
            f.Close();
        }

        public override bool fileexists(string path)
        {
            // По сути, проверяется существование только каталога (файла верхнего уровня)
            // Это неправильно для формата 8.0 с файлом каталогом metadata. Но метод fileexists используется только для внешних файлов,
            // поэтому такой проверки достаточно

            V8File f;
            int i;

            if (!cat.IsOpen())
                return false;

            string fname = new StringBuilder(path).Replace('/', '\\').ToString();
            i = fname.IndexOf("\\");
            if (i != -1)
            {
                fname = fname.Substring(1, i - 1);
            }
            f = cat.GetFile(fname);
            if (f is null)
                return false;
            return true;
        }

    }

    /// <summary>
    /// Базовый класс адаптера таблицы - контейнера конфигурации (CONFIG, CONFICAS, CONFIGSAVE, CONFICASSAVE)
    /// </summary>
    public class ConfigStorageTable : ConfigStorage
    {

        public ConfigStorageTable(Tools1CD _base = null) : base() { }

        public override ConfigFile readfile(String path) { return new ConfigFile(); }
        public override bool writefile(String path, Stream str) { return true; }
        public override void close(ConfigFile cf) { }
        /// <summary>
        /// сохранение конфигурации в файл
        /// </summary>
        /// <param name="_filename"></param>
        /// <returns></returns>
	    public bool save_config(String _filename) { return true; }
        public bool getready() { return ready; }
        public override bool fileexists(String path) { return true; }


        protected SortedDictionary<String, Container_file> files;

        protected bool ready = false;

        private Tools1CD base_; // установлена, если база принадлежит адаптеру конфигурации
    }
    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIG (конфигурации базы данных)
    /// </summary>
    public class ConfigStorageTableConfig : ConfigStorageTable
    {

        public ConfigStorageTableConfig(TableFiles tabf, Tools1CD _base = null) { }
        public override String presentation() { return " "; }

        private String present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGSAVE (основной конфигурации)
    /// </summary>
    public class ConfigStorageTableConfigSave : ConfigStorageTable
    {

        public ConfigStorageTableConfigSave(TableFiles tabc, TableFiles tabcs, Tools1CD _base = null) { }
        public override String presentation() { return " "; }

        private String present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGCAS (расширения конфигурации базы данных)
    /// </summary>
    public class ConfigStorageTableConfigCas : ConfigStorageTable
    {

        public ConfigStorageTableConfigCas(TableFiles tabc, String configver, Tools1CD _base = null) { }
        public override String presentation() { return " "; }

        private String present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGCASSAVE (расширения основной конфигурации)
    /// </summary>
    public class ConfigStorageTableConfigCasSave : ConfigStorageTable
    {
        public ConfigStorageTableConfigCasSave(TableFiles tabc, TableFiles tabcs, Guid uid, String configver, Tools1CD _base = null) { }
        public override String presentation() { return " "; }

        private String present;
    }


}
