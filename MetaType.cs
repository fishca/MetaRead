﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.APIcfBase;


namespace MetaRead
{
    /// <summary>
    /// Тип метаданных
    /// </summary>
    public class MetaType : MetaBase
    {
        private void Init()
        {
            fhasuid = fuid != Constants.EmptyUID;
            //fserializationtree = null;
            gentypeRef = null;
            fserialization_ver = 0;
            fimageindex = -1;
            fprenameindex = 999;
            fpreidindex = 999;
            fmeta = false;
            fexporttype = ExportType.et_default;
            fdefaultclass = null;
            //fserializationtree = null;
        }

        public MetaTypeSet typeSet; // набор типов, которому принадлежит этот тип
        public Guid fuid;                  // УИД типа
        public bool fhasuid;               // Признак наличия УИД
        public string fmetaname;
        public string femetaname;
        public string fgentypeprefix;
        public string fegentypeprefix;

        public int fimageindex;         // индекс картинки
        public uint fserialization_ver; // Вариант сериализации
        public uint fprenameindex;      // Индекс колонки имя предопределенного
        public uint fpreidindex;        // ИндексКолонкиИДПредопределенного

        //public std::vector<MetaProperty*> fproperties; // Свойства

        //std::map<Key, Value> → SortedDictionary<TKey, TValue>
        //std::unordered_map<Key, Value> → Dictionary<TKey, TValue>

        public List<MetaProperty> fproperties; // Свойства

        public SortedDictionary<String, MetaProperty> fproperties_by_name; // Соответствие имен (русских и английских) свойствам

        public List<MetaValue> fvalues;  // Предопределенные значения типа

        public SortedDictionary<int, MetaValue> fvalues_by_value; // Соответствие числовых значений предопределенным значениям

        public SortedDictionary<String, MetaValue> fvalues_by_name; // Соответствие имен (русских и английских) предопределенным значениям

        public List<MetaType> fcollectiontypes; // Типы элементов коллекции
        public List<String> fscollectiontypes; // Имена типов элементов коллекции

        public List<MetaGeneratedType> fgeneratedtypes; // Генерируемые типы

        public bool fmeta; // Признак объекта метаданных

        public ExportType fexporttype;

        public Class fdefaultclass;

        // Дерево сериализации
        public List<SerializationTreeVar> fserializationvars;
        public SerializationTreeNode fserializationtree; // Если NULL - дерева сериализации нет
        public List<ExternalFile> fexternalfiles;

        public MetaType(MetaTypeSet _typeSet, string _name, string _ename, string _metaname, string _emetaname, string _uid) : base(_name, _ename)
        {
            fmetaname = _metaname;
            femetaname = _emetaname;
            typeSet = _typeSet;
            string_to_GUID(_uid, ref fuid);
            Init();
        }

        public MetaType(MetaTypeSet _typeSet, string _name, string _ename, string _metaname, string _emetaname, Guid _uid) : base(_name, _ename)
        {
            fmetaname = _metaname;
            femetaname = _emetaname;
            typeSet = _typeSet;
            fuid = _uid;
            Init();
        }

        public MetaType(MetaTypeSet _typeSet, Tree tr)
        {
            Tree tt;
            Tree t;
            int num, i;
            MetaGeneratedType gt;
            Guid guid = new Guid();

            gentypeRef = null;

            tt = tr.Get_First();
            Name = tt.Get_Value();
            tt = tt.Get_Next();
            EName = tt.Get_Value();
            tt = tt.Get_Next();
            fhasuid = string_to_GUID(tt.Get_Value(), ref fuid);

            tt = tt.Get_Next();
            fserialization_ver = Convert.ToUInt32(tt.Get_Value());

            tt = tt.Get_Next();
            fmetaname = tt.Get_Value();

            tt = tt.Get_Next();
            femetaname = tt.Get_Value();

            tt = tt.Get_Next();
            fimageindex = Convert.ToInt32(tt.Get_Value());

            tt = tt.Get_Next();
            fprenameindex = Convert.ToUInt32(tt.Get_Value());

            tt = tt.Get_Next();
            fpreidindex = Convert.ToUInt32(tt.Get_Value());

            tt = tt.Get_Next();
            fmeta = tt.Get_Value().CompareTo("1") == 0 ? true : false;

            tt = tt.Get_Next();
            fexporttype = (ExportType)(Convert.ToInt32(tt.Get_Value()));

            tt = tt.Get_Next();
            string_to_GUID(tt.Get_Value(), ref guid);
            fdefaultclass = Class.GetClass(guid);

            // Свойства
            tt = tt.Get_Next();
            t = tt.Get_First();
            num = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                MetaProperty mp = new MetaProperty(this, t);
                fproperties.Add(mp);
                fproperties_by_name[mp.Name.ToUpper()] = mp;
                fproperties_by_name[mp.EName.ToUpper()] = mp;
            }

            // Элементы коллекции
            tt = tt.Get_Next();
            t = tt.Get_First();
            num = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                fscollectiontypes.Add(t.Get_Value());
            }

            // Значения
            tt = tt.Get_Next();
            t = tt.Get_First();
            num = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                MetaValue mv = new MetaValue(this, t);
                fvalues.Add(mv);
                fvalues_by_name[mv.Name.ToUpper()] = mv;
                fvalues_by_name[mv.EName.ToUpper()] = mv;
                fvalues_by_value[mv.Value] = mv;
            }

            // Генерируемые типы
            tt = tt.Get_Next();
            t = tt.Get_First();
            fgentypeprefix = t.Get_Value();
            t = t.Get_Next();
            fegentypeprefix = t.Get_Value();
            t = t.Get_Next();
            num = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                gt = new MetaGeneratedType(t);
                fgeneratedtypes.Add(gt);
                if (gt.Name.CompareTo("Ссылка") == 0)
                    gentypeRef = gt;
            }

            //fserializationtree = null;

            typeSet.Add(this);
        }

        public void setSerializationTree(Tree tr)
        {
            Tree t;
            int num, i;

            // Переменные дерева сериализации
            t = tr.Get_First();
            num = Convert.ToInt32( t.Get_Value() );
            for (i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                fserializationvars.Add(new SerializationTreeVar(t));
            }

            // Дерево сериализации
            tr = tr.Get_Next();
            t = tr.Get_First();
            fserializationtree = SerializationTreeNode. SerializationTree(this, t, NULL);

            // Внешние файлы
            tr = tr->get_next();
            t = tr->get_first();
            num = t->get_value().ToIntDef(0);
            for (i = 0; i < num; ++i)
            {
                t = t->get_next();
                fexternalfiles.push_back(new ExternalFile(this, t));
            }
        }

        public MetaGeneratedType gentypeRef; // генерируемый тип Ссылка
 
        public Guid uid
        {
            get { return fuid; }
        }
        public String Metaname
        {
            get { return fmetaname; }
        }
        public String Emetaname
        {
            get { return femetaname; }
        }
        public String GenTypePrefix
        {
            get { return fgentypeprefix; }
        }
        public String EgenTypePrefix
        {
            get { return fegentypeprefix; }
        }

        public bool HasUid
        {
            get { return fhasuid; }
        }
        public uint Serialization_Ver
        {
            get { return fserialization_ver; }
        }
        public int ImageIndex
        {
            get { return fimageindex; }
        }
        public uint PreNameIndex
        {
            get { return fprenameindex; }
        }
        public uint PreIdIndex
        {
            get { return fpreidindex; }
        }
        public List<MetaProperty> Properties
        {
            get { return fproperties; }
        }
        public List<MetaValue> Values
        {
            get { return fvalues; }
        }

        public List<MetaType> CollectionTypes
        {
            get { return fcollectiontypes; }
        }

        public List<MetaGeneratedType> GeneratedTypes
        {
            get { return fgeneratedtypes; }
        }

        public MetaTypeSet TypeSet
        {
            get { return typeSet; }
        }

        public List<SerializationTreeVar> SerializationVars
        {
            get { return fserializationvars; }
        }

        public SerializationTreeNode SerializationTree
        {
            get { return fserializationtree; }
        }

        public List<ExternalFile> ExternalFiles
        {
            get { return fexternalfiles; }
        }

        public bool Meta
        {
            get { return fmeta; }
        }

        public ExportType ExportType
        {
            get { return fexporttype; }
        }

        public Class DefaultClass
        {
            get { return fdefaultclass; }
        }

        public MetaProperty GetProperty(string n)
        {
            if (!fproperties_by_name.TryGetValue(n.ToUpper(), out MetaProperty val))
                return null;
            else
                return val;
        }

        public MetaProperty GetProperty(int index)
        {
            return fproperties[index];
        }

        public MetaValue GetValue(string n)
        {
            if (!fvalues_by_name.TryGetValue(n.ToUpper(), out MetaValue val))
                return null;
            else
                return val;
        }

        public MetaValue GetValue(int value)
        {
            if (!fvalues_by_value.TryGetValue(value, out MetaValue val))
                return null;
            else
                return val;
        }

        public int NumberOfProperties()
        {
            return fproperties.Count;
        }

        // Заполнить типы элементов коллекции по их именам (по fscollectiontypes заполнить fcollectiontypes)
        public void FillCollectionTypes()
        {
            if (fcollectiontypes.Count != 0)
                fcollectiontypes.Clear();

            for (int i = 0; i < fcollectiontypes.Count; i++)
            {
                fcollectiontypes.Add(typeSet.GetTypeByName(Convert.ToString(i)));
            }
        } 

        public String GetMetaName(bool english = false)
        {
            return english ? femetaname : fmetaname;
        }


    }
}