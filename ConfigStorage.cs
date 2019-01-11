using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.Structures;

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
            File    = _f;
            Name    = _name;
            Stream  = null;
            Rstream = null;
            Cat     = null;
            Packed  = table_file_packed.unknown;
            Dynno   = -3;
        }

        public bool open()
        {

            const int MAX_PATH = 260;
            Stream ts;
            string tn = "";
            char[] tempfile = new char[MAX_PATH];
            int i;
            V8Table t;
            table_blob_file addr; // TODO: возможно это должен быть МАССИВ !!!!!!!!!!!!!!!!!!!!!!!!
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
                if (packed == table_file_packed.tfp_yes)
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
                    t.ReadBlob(ts, addr[i].blob_start, addr[i].blob_length, false);

            }

            return true;
        }
        public bool ropen() { return true; } // raw open
        public void close() {  }
        public bool isPacked() { return true; }
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

        private String fdir;

        public ConfigStorageDirectory(String _dir) { }
        public override ConfigFile readfile(String path) { return new ConfigFile(); }
        public override bool writefile(String path, Stream str) { return true; }
        public override String presentation() { return " "; }
        public override void close(ConfigFile cf)  { }
        public override bool fileexists(String path) { return true; }

    }

    /// <summary>
    /// Класс адаптера контейнера конфигурации - cf (epf, erf, cfe) файл
    /// </summary>
    class ConfigStorageCFFile : ConfigStorage
    {
    
	    private String filename;
        private V8Catalog cat;

        public ConfigStorageCFFile(String fname) { }

        public override ConfigFile readfile(String path) { return new ConfigFile(); }
        public override bool writefile(String path, Stream str) { return true; }
        public override String presentation() { return " "; }
        public override void close(ConfigFile cf) { }
    	public override bool fileexists(String path) { return true; }

    }

    /// <summary>
    /// Базовый класс адаптера таблицы - контейнера конфигурации (CONFIG, CONFICAS, CONFIGSAVE, CONFICASSAVE)
    /// </summary>
    public class ConfigStorageTable : ConfigStorage
    {

        public ConfigStorageTable(Tools1CD _base = null) : base(){}

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

        private	String present;
    }


}
