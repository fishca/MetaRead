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

        public Stream stream;  // stream (нераспакованный поток)
        public Stream rstream; // raw stream (нераспакованный поток)

        /// <summary>
        /// static wchar_t temppath[MAX_PATH];
        /// </summary>
        public string temppath;

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
            temppath = "";
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

            // TODO: Времянка
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
            if (!f.Open())
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

        public ConfigStorageTable(Tools1CD _base = null) : base()
        {
            this.base_ = _base;
        }

        public override ConfigFile readfile(String path)
        {
            Container_file tf = null;
            V8Catalog c;
            V8File f;
            int i;
            string fname;
            string r_name;
            ConfigFile cf;
            ConfigStorageTable_addin cfa;

            if (!ready)
                return null;

            fname = new StringBuilder(path).Replace('/', '\\').ToString();
            i = fname.IndexOf("\\");
            if (i != -1)
            {
                r_name = fname.Substring(1, i - 1);
                fname = fname.Substring(i + 1, fname.Length - i);
            }
            else
            {
                r_name = fname;
                fname = "";
            }

            tf = (files.TryGetValue(r_name.ToUpper(), out Container_file val)) ? val : null;
            tf.open();

            if (!string.IsNullOrEmpty(fname))
            {
                if (tf.Cat is null)
                {
                    tf.Cat = new V8Catalog(tf.Stream, false, true);
                }
                c = tf.Cat;
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
                if (!f.Open())
                    return null;
                cf = new ConfigFile();
                cfa = new ConfigStorageTable_addin();
                cfa.variant = ConfigStorageTableAddinVariant.cstav_v8file;
                cfa.f = f;
                cf.str = f.GetData();
                cf.str.Seek(0, SeekOrigin.Begin);
                cf.addin = cfa;
            }
            else
            {
                cf = new ConfigFile();
                cfa = new ConfigStorageTable_addin();
                cfa.variant = ConfigStorageTableAddinVariant.cstav_container_file;
                cfa.tf = tf;
                cf.str = tf.Stream;
                cf.str.Seek(0, SeekOrigin.Begin);
                cf.addin = cfa;
            }
            return cf;
        }

        public override bool writefile(String path, Stream str)
        {
            return false; // Запись в таблицы пока не поддерживается
        }

        public override void close(ConfigFile cf)
        {
            ConfigStorageTable_addin cfa;

            cfa = (ConfigStorageTable_addin)cf.addin;
            if (cfa.variant == ConfigStorageTableAddinVariant.cstav_container_file)
            {
                cfa.tf.close();
            }
            else if (cfa.variant == ConfigStorageTableAddinVariant.cstav_v8file)
            {
                cfa.f.Close();
            }
        }

        /// <summary>
        /// сохранение конфигурации в файл
        /// </summary>
        /// <param name="_filename"></param>
        /// <returns></returns>
        public bool save_config(String _filename)
        {
            V8Catalog c;
            V8File f;

            Container_file tf;
            int i, j, prevj, size;

            if (!ready)
                return false;

            size = files.Count;
            prevj = 101;

            if (File.Exists(_filename))
                File.Delete(_filename);
            c = new V8Catalog(_filename, false);

            i = 1;
            foreach (var item_files in files)
            {
                j = i * 100 / size;
                if (j != prevj)
                {
                    prevj = j;
                }
                tf = item_files.Value;
                if (!tf.ropen())
                {
                    f = c.CreateFile(tf.Name);
                    //f.SetTimeCreate(tf.File.Ft_create);
                    //f.SetTimeModify(tf.File.Ft_modify);
                    f.WriteAndClose(tf.Rstream);
                    tf.close();
                }
                ++i;
            }
            return true;
        }

        public bool getready()
        {
            return ready;
        }

        public override bool fileexists(String path)
        {
            // По сути, проверяется существование только каталога (файла записи верхнего уровня)
            // Это неправильно для формата 8.0 с файлом каталогом metadata. Но метод fileexists используется только для внешних файлов, поэтому такой проверки достаточно

            int i;
            string fname;
            if (!ready)
                return false;
            fname = new StringBuilder(path).Replace('/', '\\').ToString();
            i = fname.IndexOf("\\");
            if (i != -1)
            {
                fname = fname.Substring(1, i - 1);
            }
            return files.TryGetValue(fname.ToUpper(), out Container_file val) ? true : false;
        }

        protected SortedDictionary<String, Container_file> files;

        protected bool ready = false;

        private Tools1CD base_; // установлена, если база принадлежит адаптеру конфигурации
    }
    
    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIG (конфигурации базы данных)
    /// </summary>
    public class ConfigStorageTableConfig : ConfigStorageTable
    {

        public ConfigStorageTableConfig(TableFiles tabf, Tools1CD _base = null)
        {
            int m;
            string s;
            string name, ext;
            string spoint = ".";
            string sdynupdate = "_dynupdate_";
            int lsdynupdate = sdynupdate.Length;

            TableFile tf;
            //std::map<String, table_file*>::iterator pfilesmap;
            //std::map<String, container_file*>::iterator pfiles;
            TableFile _DynamicallyUpdated;
            Container_file DynamicallyUpdated;
            Tree tt;
            Tree ct;
            Guid[] dynup;
            int ndynup = 0;
            Guid g = EmptyUID;
            V8Table tab;
            int dynno;
            Container_file pcf;

            ready = tabf.getready();
            if (!ready)
                return;
            present = tabf.gettable().Getbase().Getfilename() + "\\CONFIG";
            tab = tabf.gettable();

            _DynamicallyUpdated = tabf.getfile("DynamicallyUpdated");
            dynup = null;

            if (_DynamicallyUpdated != null)
            {
                DynamicallyUpdated = new Container_file(_DynamicallyUpdated, _DynamicallyUpdated.Name);
                DynamicallyUpdated.open();
                s = tab.Getbase().Getfilename() + "\\" + tab.Getname() + "\\" + DynamicallyUpdated.Name;
                //tt = parse_1Cstream()
                tt = Tree.Parse_1Cstream(DynamicallyUpdated.Stream, "", s);
                if (tt is null)
                {
                    // error(L"Ошибка разбора файла DynamicallyUpdated"
                    //     , L"Путь", s);

                }
                else
                {
                    ct = tt.Get_First();
                    if (ct is null)
                    {
                        // error(L"Ошибка разбора файла DynamicallyUpdated"
                        //     , L"Путь", s);
                    }
                    else
                    {
                        ct = tt.Get_First();
                        if (ct is null)
                        {
                            // error(L"Ошибка разбора файла DynamicallyUpdated"
                            //     , L"Путь", s);
                        }
                        else
                        {
                            ndynup = Convert.ToInt32(ct.Get_Value());
                            if (ndynup > 0)
                            {
                                dynup = new Guid[ndynup];
                                for (m = 0; m < ndynup; ++m)
                                {
                                    ct = ct.Get_Next();
                                    string_to_GUID(ct.Get_Value(), ref dynup[m]);
                                }
                            }
                            else
                            {
                                ndynup = 0;
                            }

                        }
                    }
                }
            }

            int index = 0;

            foreach (var item_files in tabf.files)
            {
                tf = item_files.Value;
                if (tf == _DynamicallyUpdated)
                    continue;
                if (tf.addr[index].Blob_length == 0)
                    continue;
                s = tf.Name;

                m = s.LastIndexOfAny(spoint.ToCharArray());

                if (m != -1)
                {
                    name = s.Substring(1, m - 1);
                    ext = s.Substring(m + 1, s.Length - m);
                }
                else
                {
                    name = s;
                    ext = "";
                }

                if (ext.CompareTo("new") == 0)
                {
                    ext = "";
                    dynno = -2;
                }
                else
                {
                    dynno = -1;
                }


                m = name.IndexOf(sdynupdate);
                if (m != -1)
                {
                    s = name.Substring(m + lsdynupdate, name.Length - m - lsdynupdate + 1);
                    name = name.Substring(1, m - 1);
                    string_to_GUID(s, ref g);
                    if (dynup != null)
                    {
                        for (m = 0; m < ndynup; ++m)
                        {
                            if (g == dynup[m])
                                break;
                            if (m >= ndynup)
                                dynno = -2;
                            else
                                dynno = m;
                        }
                    }
                    else
                    {
                        dynno = -2;
                    }
                }
                if (string.IsNullOrEmpty(ext))
                {
                    name = name + spoint + ext;
                }
                s = name.ToUpper();

                if (files.TryGetValue(s, out Container_file val))
                {
                    pcf = val;
                    if (pcf.Dynno < dynno)
                    {
                        pcf.File = tf;
                        pcf.Dynno = dynno;
                    }
                }
                else
                {
                    pcf = new Container_file(tf, name);
                    files[s] = pcf;
                    pcf.Dynno = dynno;
                }
                ++index;
            }

        }

        public override String presentation()
        {
            return present;
        }

        private string present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGSAVE (основной конфигурации)
    /// </summary>
    public class ConfigStorageTableConfigSave : ConfigStorageTable
    {

        public ConfigStorageTableConfigSave(TableFiles tabc, TableFiles tabcs, Tools1CD _base = null)
        {
            int m;
            string s;
            string name, ext;
            TableFile tf;
            TableFile _DynamicallyUpdated;
            TableFile _deleted;
            Container_file pcf;
            Container_file DynamicallyUpdated;
            Container_file deleted;
            Tree tt;
            Tree ct;
            Guid[] dynup;
            int ndynup = 0;
            Guid g = EmptyUID;
            V8Table tab;
            int dynno;
            HashSet<string> del = null;

            ready = tabc.getready();
            if (!ready)
                return;
            ready = tabcs.getready();
            if (!ready)
                return;

            present = tabc.gettable().Getbase().Getfilename() + "\\CONFIGSAVE";

            tab = tabcs.gettable();
            _deleted = tabcs.getfile("deleted");

            if (_deleted != null)
            {
                deleted = new Container_file(_deleted, _deleted.Name);
                deleted.open();
                s = tab.Getbase().Getfilename() + "\\" + tab.Getname() + "\\" + deleted.Name;
                tt = Tree.Parse_1Cstream(deleted.stream, "", s);
                if (tt is null)
                {
                    // error(L"Ошибка разбора файла deleted"
                    //     , L"Путь", s);
                }
                else
                {
                    ct = tt.Get_First();
                    if (ct is null)
                    {
                        // error(L"Ошибка разбора файла DynamicallyUpdated"
                        //     , L"Путь", s);
                    }
                    else
                    {
                        for (m = Convert.ToInt32(ct.Get_Value()); m != 0; --m)
                        {
                            ct = ct.Get_Next();
                            if (ct is null)
                            {
                                // error(L"Ошибка разбора файла DynamicallyUpdated"
                                //     , L"Путь", s);
                                break;
                            }
                            del.Add(ct.Get_Value().ToUpper());
                            ct = ct.Get_Next();
                            if (ct is null)
                            {
                                // error(L"Ошибка разбора файла DynamicallyUpdated"
                                //     , L"Путь", s);
                                break;
                            }
                        }
                    }

                }
            }

            tab = tabc.gettable();
            _DynamicallyUpdated = tabc.getfile("DynamicallyUpdated");
            dynup = null;
            if (_DynamicallyUpdated != null)
            {
                DynamicallyUpdated = new Container_file(_DynamicallyUpdated, _DynamicallyUpdated.Name);
                DynamicallyUpdated.open();
                s = tab.Getbase().Getfilename() + "\\" + tab.Getname() + "\\" + DynamicallyUpdated.Name;
                tt = Tree.Parse_1Cstream(DynamicallyUpdated.stream, "", s);
                if (tt is null)
                {
                    // error(L"Ошибка разбора файла DynamicallyUpdated"
                    //     , L"Путь", s);
                }
                else
                {
                    ct = tt.Get_First();
                    if (ct is null)
                    {
                        // error(L"Ошибка разбора файла DynamicallyUpdated"
                        //     , L"Путь", s);
                    }
                    else
                    {
                        ct = tt.Get_First();
                        if (ct is null)
                        {
                            // error(L"Ошибка разбора файла DynamicallyUpdated"
                            //     , L"Путь", s);
                        }
                        else
                        {
                            ct = ct.Get_Next();
                            if (ct is null)
                            {
                                // error(L"Ошибка разбора файла DynamicallyUpdated"
                                //     , L"Путь", s);
                            }
                            else
                            {
                                if (ct.Get_Type() != Node_Type.nd_number)
                                {
                                    // error(L"Ошибка разбора файла DynamicallyUpdated"
                                    //     , L"Путь", s);
                                }
                                else
                                {
                                    ndynup = Convert.ToInt32(ct.Get_Value());
                                    if (ndynup > 0)
                                    {
                                        dynup = new Guid[ndynup];
                                        for (m = 0; m < ndynup; ++m)
                                        {
                                            ct = ct.Get_Next();
                                            string_to_GUID(ct.Get_Value(), ref dynup[m]);
                                        }
                                    }
                                    else ndynup = 0;
                                }

                            }
                        }
                    }
                }

            }

            m = ndynup + 2;

            int index = 0;
            foreach (var item_files in tabcs.files)
            {
                tf = item_files.Value;
                if (tf == _deleted)
                    continue;
                if (tf.addr[index].Blob_length == 0)
                    continue;

                name = tf.Name;
                s = name.ToUpper();

                if (files.TryGetValue(s, out Container_file val))
                {
                    // error(L"Ошибка чтения CONFIGSAVE. Повторяющееся имя файла"
                    // , L"Имя файла", s);
                }
                else
                {
                    pcf = new Container_file(tf, name);
                    files[s] = pcf;
                    pcf.Dynno = m;
                }
                ++index;
            }

            index = 0;
            foreach (var item_files in tabc.files)
            {
                tf = item_files.Value;
                if (tf == _DynamicallyUpdated)
                    continue;
                if (tf.addr[index].Blob_length == 0)
                {
                    continue;
                }
                s = tf.Name;

                if (del.Contains(s.ToUpper()))
                    continue;

                m = s.LastIndexOfAny(".".ToCharArray());
                if (m != -1)
                {
                    name = s.Substring(1, m - 1);
                    ext = s.Substring(m + 1, s.Length - m);
                }
                else
                {
                    name = s;
                    ext = "";
                }
                if (ext.CompareTo("new") == 0)
                {
                    ext = "";
                    dynno = ndynup + 1;
                }
                else
                {
                    dynno = -1;
                }
                m = name.IndexOf("_dynupdate_");
                if (m != -1)
                {
                    s = name.Substring(m + "_dynupdate_".Length, name.Length - m - "_dynupdate_".Length + 1);
                    name = name.Substring(1, m - 1);
                    string_to_GUID(s, ref g);
                    if (dynup != null)
                    {
                        for (m = 0; m < ndynup; ++m)
                        {
                            if (g == dynup[m])
                                break;
                        }

                        dynno = (m >= ndynup) ? -2 : m;
                    }
                    else
                    {
                        dynno = -2;
                    }
                }

                if (!string.IsNullOrEmpty(ext))
                {
                    name = name + "." + ext;
                }
                s = name.ToUpper();
                if (files.TryGetValue(s, out Container_file val))
                {
                    pcf = val;
                    if (pcf.Dynno < dynno)
                    {
                        pcf.File = tf;
                        pcf.Dynno = dynno;
                    }
                }
                else
                {
                    pcf = new Container_file(tf, name);
                    files[s] = pcf;
                    pcf.Dynno = dynno;
                }
                ++index;
            }
        }

        public override String presentation()
        {
            return present;
        }

        private String present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGCAS (расширения конфигурации базы данных)
    /// </summary>
    public class ConfigStorageTableConfigCas : ConfigStorageTable
    {

        public ConfigStorageTableConfigCas(TableFiles tabc, String configver, Tools1CD _base = null)
        {
            int m;
            String s, name, hashname;
            TableFile _configinfo;
            Container_file configinfo;
            Tree tt;
            Tree ct;
            TableFile tf;
            Container_file pcf;
            MemoryStream stream;

            ready = tabc.getready();
            if (!ready)
                return;

            present = tabc.gettable().Getbase().Getfilename() + "\\CONFIGCAS";
            s = present + "\\" + configver;

            _configinfo = tabc.getfile(configver);
            if (_configinfo is null)
            {
                // error(L"Ошибка поиска файла"
                //     , L"Путь", s
                //     , L"Имя", configver
                //     , L"Имя файла", L"configinfo");
                return;
            }

            configinfo = new Container_file(_configinfo, "configinfo");
            files["CONFIGINFO"] = configinfo;
            configinfo.open();
            tt = Tree.Parse_1Cstream(configinfo.stream, "", s);
            if (tt is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = tt.Get_First();
            if (ct is null)
            {
                //error(L"Ошибка разбора файла configinfo"
                //    , L"Путь", s);
                //delete tt;
                return;
            }
            ct = ct.Get_Next();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = ct.Get_Next();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = ct.Get_First();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }

            stream = new MemoryStream();
            for (m = Convert.ToInt32(ct.Get_Value()); m != 0; --m)
            {
                ct = ct.Get_Next();
                if (ct is null)
                {
                    // error(L"Ошибка разбора файла configinfo"
                    //     , L"Путь", s);
                    // delete tt;
                    // delete stream;
                    return;
                }
                if (ct.Get_Type() != Node_Type.nd_string)
                {
                    // error(L"Ошибка разбора файла configinfo"
                    //     , L"Путь", s);
                    // delete tt;
                    // delete stream;
                    return;
                }
                name = ct.Get_Value();

                ct = ct.Get_Next();
                if (ct is null)
                {
                    // error(L"Ошибка разбора файла configinfo"
                    //     , L"Путь", s);
                    // delete tt;
                    // delete stream;
                    return;
                }
                if (ct.Get_Type() != Node_Type.nd_binary2)
                {
                    // error(L"Ошибка разбора файла configinfo"
                    //     , L"Путь", s);
                    // delete tt;
                    // delete stream;
                    return;
                }
                stream.Seek(0, SeekOrigin.Begin);

                //base64_decode(ct->get_value(), stream);

                byte[] data = Convert.FromBase64String(ct.Get_Value());
                string decodedString = Encoding.UTF8.GetString(data);
                stream.Write(data, 0, data.Length);

                stream.Seek(0, SeekOrigin.Begin);

                hashname = hexstring(stream);

                tf = tabc.getfile(hashname);
                if (tf != null)
                {
                    pcf = new Container_file(tf, name);
                    files[name.ToUpper()] = pcf;
                }
                else
                {
                    // error(L"Ошибка поиска файла"
                    //     , L"Путь", s
                    //     , L"Имя", hashname
                    //     , L"Имя файла", name);
                    
                    stream.Dispose();
                    return;
                }
            }

            
            stream.Dispose();

        }

        public override String presentation()
        {
            return present;
        }

        private String present;
    }

    /// <summary>
    /// Класс адаптера таблицы - контейнера конфигурации CONFIGCASSAVE (расширения основной конфигурации)
    /// </summary>
    public class ConfigStorageTableConfigCasSave : ConfigStorageTable
    {
        public ConfigStorageTableConfigCasSave(TableFiles tabc, TableFiles tabcs, Guid uid, String configver, Tools1CD _base = null)
        {
            int m;
            String s, name, hashname;
            TableFile _configinfo;
            Container_file configinfo;
            Tree tt;
            Tree ct;
            TableFile tf;
            Container_file pcf;
            MemoryStream stream;
            string g;
            int gl;
            //std::map<String, table_file*>::iterator ptf;


            ready = tabc.getready();
            if (!ready)
                return;
            ready = tabcs.getready();
            if (!ready)
                return;

            configinfo = null;
            present = tabcs.gettable().Getbase().Getfilename() + "\\CONFIGCASSAVE";

            //g = GUID_to_string(uid) + L"__";
            g = uid.ToString() + "__";
            gl = g.Length;

            foreach (var item_files in tabcs.files)
            {
                if (item_files.Key.Substring(1, gl).CompareTo(g) == 0)
                {
                    tf = item_files.Value;
                    pcf = new Container_file(tf, tf.Name.Substring(gl + 1, tf.Name.Length - gl));
                    if (pcf.Name.CompareTo("configinfo") == 0)
                        configinfo = pcf;
                    files[pcf.Name.ToUpper()] = pcf;

                }
            }

            if (configinfo is null)
            {
                if (files.Count == 0)
                {
                    //error(L"Ошибка поиска файла"
                    //    , L"Путь", present
                    //    , L"Имя", g + L"configinfo"
                    //    , L"Имя файла", L"configinfo");
                    return;
                }

                s = tabc.gettable().Getbase().Getfilename() + "\\CONFIGCAS\\" + configver;
                _configinfo = tabc.getfile(configver);
                if (_configinfo is null)
                {
                    // error(L"Ошибка поиска файла"
                    //     , L"Путь", s
                    //     , L"Имя", configver
                    //     , L"Имя файла", L"configinfo");
                    return;
                }

                configinfo = new Container_file(_configinfo, "configinfo");
                files["CONFIGINFO"] = configinfo;
            }
            else
                s = present + "\\" + gl + "configinfo";

            configinfo.open();
            tt = Tree.Parse_1Cstream(configinfo.stream, "", s);
            if (tt is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = tt.Get_First();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = ct.Get_Next();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = ct.Get_Next();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }
            ct = ct.Get_First();
            if (ct is null)
            {
                // error(L"Ошибка разбора файла configinfo"
                //     , L"Путь", s);
                // delete tt;
                return;
            }

            stream = new MemoryStream();
            for (m = Convert.ToInt32(ct.Get_Value()); m != 0 ; --m)
            {
                ct = ct.Get_Next();
                if (ct is null)
                {
                    //error(L"Ошибка разбора файла configinfo"
                    //    , L"Путь", s);
                    //delete tt;
                    stream.Dispose();
                    return;
                }
                if (ct.Get_Type() != Node_Type.nd_string)
                {
                    // error(L"Ошибка разбора файла configinfo"
                    //     , L"Путь", s);
                    // delete tt;
                    stream.Dispose();
                    return;
                }
                name = ct.Get_Value();

                ct = ct.Get_Next();

                if (files.TryGetValue(name.ToUpper(), out Container_file val))
                {
                    continue;
                }

                if (ct is null)
                {
                    //error(L"Ошибка разбора файла configinfo"
                    //    , L"Путь", s);
                    //delete tt;
                    stream.Dispose();
                    return;
                }
                if (ct.Get_Type() != Node_Type.nd_binary2)
                {
                    //error(L"Ошибка разбора файла configinfo"
                    //    , L"Путь", s);
                    //delete tt;
                    stream.Dispose();
                    return;
                }
                stream.Seek(0, SeekOrigin.Begin);

                //base64_decode(ct->get_value(), stream);

                byte[] data = Convert.FromBase64String(ct.Get_Value());
                string decodedString = Encoding.UTF8.GetString(data);
                stream.Write(data, 0, data.Length);

                stream.Seek(0, SeekOrigin.Begin);
                hashname = hexstring(stream);

                tf = tabc.getfile(hashname);
                if (tf != null)
                {
                    pcf = new Container_file(tf, name);
                    files[name.ToUpper()] = pcf;
                }
                else
                {
                    // error(L"Ошибка поиска файла"
                    //     , L"Путь", s
                    //     , L"Имя", hashname
                    //     , L"Имя файла", name);
                    // delete tt;
                    stream.Dispose();
                    return;
                }
            }
            stream.Dispose();
        }

        public override String presentation()
        {
            return present;
        }

        private String present;
    }


}
