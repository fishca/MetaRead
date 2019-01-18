using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.Structures;

namespace MetaRead
{
    /// <summary>
    /// Класс таблицы контейнера файлов (CONFIG, CONFIGSAVE, PARAMS, FILES, CONFICAS, CONFICASSAVE)
    /// </summary>
    public class TableFiles
    {
        private V8Table table;
        private SortedDictionary<string, TableFile> allfiles;

        private byte[] rec;
        private bool ready = false;

        private bool test_table()
        {
            if (table is null)
                return false;

            if (table.Get_numfields() < 6)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. В таблице меньше 6 полей"
                //     , L"Таблица", tab->getname()
                //     , L"Кол-во полей", tab->get_numfields());
                return false;
            }

            if (table.Get_numfields() > 7)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. В таблице больше 7 полей"
                //     , L"Таблица", tab->getname()
                //     , L"Кол-во полей", tab->get_numfields());
                return false;
            }

            if (table.Getfield(0).GetName().CompareTo("FILENAME") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Первое поле таблицы не FILENAME"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(0)->getname());
                return false;
            }

            if (table.Getfield(1).GetName().CompareTo("CREATION") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Второе поле таблицы не CREATION"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(1)->getname());
                return false;
            }
            if (table.Getfield(2).GetName().CompareTo("MODIFIED") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Третье поле таблицы не MODIFIED"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(2)->getname());
                return false;
            }

            if (table.Getfield(3).GetName().CompareTo("ATTRIBUTES") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Четвертое поле таблицы не ATTRIBUTES"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(3)->getname());
                return false;
            }

            if (table.Getfield(4).GetName().CompareTo("DATASIZE") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Пятое поле таблицы не DATASIZE"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(4)->getname());
                return false;
            }

            if (table.Getfield(5).GetName().CompareTo("BINARYDATA") != 0)
            {
                // error(L"Ошибка проверки таблицы контейнера файлов. Шестое поле таблицы не BINARYDATA"
                //     , L"Таблица", tab->getname()
                //     , L"Поле", tab->getfield(5)->getname());
                return false;
            }

            if (table.Get_numfields() > 6)
            {
                if (table.Getfield(6).GetName().CompareTo("PARTNO") != 0)
                {
                    // error(L"Ошибка проверки таблицы контейнера файлов. Седьмое поле таблицы не PARTNO"
                    //     , L"Таблица", tab->getname()
                    //     , L"Поле", tab->getfield(6)->getname());
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="t"></param>
        public TableFiles(V8Table t)
        {
            V8Field filename;
            V8Field f;
            V8Field partno;
            int start;
            int length;
            //char[] create;
            byte create;
            //char[] modify;
            byte modify;
            int i;
            int j;
            string s;
            table_rec ptr;
            table_rec tr = new table_rec();
            //std::vector<table_rec> allrec;
            List<table_rec> allrec = null;
            //std::map<String, int> maxpartnos;
            SortedDictionary<String, int> maxpartnos = null;
            
            //std::map<String, int>::iterator pmaxpartno;
            TableFile tf;
            //std::map<String, table_file*>::iterator pfilesmap;

            table = t;
            ready = test_table();
            if (!ready)
                return;
            //Container_file.temppath =  System.IO.Path.GetTempPath();

            //rec = new char[table.Get_recordlen()];
            rec = new byte[table.Get_recordlen()];

            filename = table.Getfield(0);

            f = table.Getfield(1);
            // create = (unsigned char*)(rec + f->getoffset());
            create = rec[f.Getoffset()];

            f = table.Getfield(1);
            modify = rec[f.Getoffset()];

            f = table.Getfield(5);
            start = rec[f.Getoffset()];

            length = start + 1;

            partno = (table.Get_numfields() > 6) ? table.Getfield(6) : null;

            for (i = 0; i < table.Get_phys_numrecords(); ++i)
            {
                table.Getrecord((uint)i, rec);
                if (rec != null)
                    continue;
                if (start == 0)
                    continue;
                if (length == 0)
                    continue;

                tr.Name = filename.Get_presentation(rec);

                if (string.IsNullOrEmpty(tr.Name))
                    continue;

                // TODO : Надо с этим разобраться
                // tr.Addr.Blob_start = start;
                // tr.Addr.Blob_length = length;

                if (partno != null)
                {
                    tr.Partno = Convert.ToInt32(partno.Get_presentation(rec, true));
                }
                else
                {
                    tr.Partno = 0;
                }

                // TODO : Надо с этим разобраться
                // time1CD_to_FileTime(&tr.ft_create, create);
                // time1CD_to_FileTime(&tr.ft_modify, modify);

                allrec.Add(tr);

                s = tr.Name.ToUpper();
                if (!maxpartnos.TryGetValue(s, out int val))
                {
                    maxpartnos[s] = tr.Partno;
                }
                else if (val < tr.Partno)
                {
                    val = tr.Partno;
                }
            }

            foreach (var item_maxpartnos in maxpartnos)
            {
                tf = new TableFile(table, item_maxpartnos.Key, (uint)item_maxpartnos.Value);
            }

            for (j = 0; j < allrec.Count; ++j)
            {
                ptr = allrec[j];
                if (allfiles.TryGetValue(ptr.Name.ToUpper(), out TableFile val))
                {
                    tf = val;
                    tf.addr[ptr.Partno] = ptr.Addr;
                    if (ptr.Partno == 0)
                    {
                        tf.Ft_create = ptr.Ft_create;
                        tf.Ft_modify = ptr.Ft_modify;
                        tf.Name = ptr.Name;
                    }
                }
                else
                {
                    tf = null;
                }
                

            }


        }

        public bool getready()
        {
            return ready;
        }

        public TableFile getfile(string name)
        {
            return (allfiles.TryGetValue(name.ToUpper(), out TableFile val)) ? val : null;
        }

        public V8Table gettable()
        {
            return table;
        }

        public SortedDictionary<String, TableFile> files
        {
            get { return allfiles; }
        }
        

    }
}
