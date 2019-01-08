using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    //////////////////// Базовый класс значения 1С //////////////////////////////////////////
    //
    public class Value1C
    {
        public static string indentstring;

        public Value1C_obj parent;
        public int index; // Индекс в родителе (-1 - еще не определен, от 0 до (parent->v_objpropv.size() - 1) - именованое свойство, от parent->v_objpropv.size() - элемент коллекции)
        public MetaType type; // Тип значения 1С
        public KindOfValue1C kind; // Вид хранимого значения

        public Value1C(Value1C_obj _parent)
        {
            type = null;
            kind = KindOfValue1C.kv_unknown;
            index = -1;
        }

        public virtual string presentation(bool english = false)
        {
            return "";
        }

        public virtual string valuepresentation(bool english = false)
        {
            return "";
        }

        public virtual bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public string path(MetaContainer mc, bool english = false)
        {
            return "";
        }

        public string fullpath(MetaContainer mc, bool english = false)
        {
            return "";
        }

    }

    /// <summary>
    /// Значение 1С типа Булево
    /// </summary>
    public class Value1C_bool : Value1C
    {
        public bool v_bool;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_bool(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_bool;
            type = MetaTypeSet.mt_bool;
        }

        public override string presentation(bool english = false)
        {
            if (english)
                return v_bool ? "Yes" : "No";
        	else
                return v_bool ? "Да" : "Нет";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            string s;
            if (english)
                s = v_bool ? "Yes" : "No";
        	else
                s = v_bool ? "Да" : "Нет";
            str.Write(s);

            return true;
        }

        public override bool IsEmpty()
        {
            if (!(this is null))
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа Строка
    /// </summary>
    public class Value1C_string : Value1C
    {
        public string v_string;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_string(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_string;
            type = MetaTypeSet.mt_string;
        }

        public override string presentation(bool english = false)
        {
            return v_string;
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            StringBuilder sb = new StringBuilder(v_string);
            sb.Replace("\"", "\"\"");
            str.Write("\"");
            str.Write(sb.ToString());
            str.Write("\"");

            return true;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(v_string);
        }
    }

    /// <summary>
    /// Значение 1С типа Число
    /// </summary>
    public class Value1C_number : Value1C
    {
        public string v_string;
        public int v_number;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_number(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_number;
            type = MetaTypeSet.mt_number;
        }

        public override string presentation(bool english = false)
        {
            return v_string;
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            str.Write(v_string);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа Число с плавающей запятой
    /// </summary>
    public class Value1C_number_exp : Value1C
    {
        public string v_string;
        public decimal v_number;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_number_exp(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_number_exp;
            type = MetaTypeSet.mt_number;
        }

        public override string presentation(bool english = false)
        {
            return v_string;
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            str.Write(v_string);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа Дата
    /// </summary>
    public class Value1C_date : Value1C
    {
        // static char emptydate[7];
        public static string emptydate;
        // unsigned char v_date[7];
        public string v_date;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_date(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_date;
            type = MetaTypeSet.mt_date;
            v_date = emptydate;
        }

        public override string presentation(bool english = false)
        {
            return date_to_string(v_date);
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            string s = "'";
            s += date_to_string(v_date);
            s += "'";
            str.Write(s);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа NULL
    /// </summary>
    public class Value1C_null : Value1C
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_null(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_null;
            type = MetaTypeSet.mt_null;
        }

        public override string presentation(bool english = false)
        {
            return "<Null>";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            string s = "NULL";
            str.Write(s);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа Неопределено
    /// </summary>
    public class Value1C_undef : Value1C
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_undef(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_undef;
            type = MetaTypeSet.mt_undef;
        }

        public override string presentation(bool english = false)
        {
            return english ? "<Undefined>" : "<Неопределено>";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            string s = english ? "<Undefined>" : "<Неопределено>";
            str.Write(s);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Значение 1С типа Тип
    /// </summary>
    public class Value1C_type : Value1C
    {
        public MetaType v_type;
        public Guid v_uid;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_type(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_type;
            type = MetaTypeSet.mt_type;
            v_type = null;
            v_uid = new Guid();
        }

        public override string presentation(bool english = false)
        {
            if (v_type != null)
                return v_type.GetName(english);
            if (english)
                return "<??? Unknown type>";
        	else
                return "<??? Неизвестный тип>";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_type == null)
                return false;
            string s = english ? v_type.EName : v_type.Name;
            str.Write(s);
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return (v_type == null);
        }
    }
    
    /// <summary>
    /// Значение 1С типа УникальныйИдентификатор
    /// </summary>
    public class Value1C_uid : Value1C
    {
        public Guid v_uid;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_uid(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_uid;
            type = MetaTypeSet.mt_uid;
            v_uid = new Guid();
        }

        public override string presentation(bool english = false)
        {
            return v_uid.ToString();
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            str.Write(v_uid.ToString());
            return true;
        }

        public override bool IsEmpty()
        {
            if (this != null)
                return true;
            return v_uid == new Guid();
        }
    }

    /// <summary>
    /// Значение 1С - системное перечисление
    /// </summary>
    public class Value1C_enum : Value1C
    {
        public MetaValue v_enum; // Значение системного перечисления

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_enum(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_enum;
            v_enum = null;
        }

        public override string presentation(bool english = false)
        {
            return v_enum.GetName(english);
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_enum == null)
                return false;
            string s = english ? v_enum.EName : v_enum.Name;
            str.Write(s);

            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_enum == null);
        }
    }

    /// <summary>
    /// Значение 1С - стандартный реквизит
    /// </summary>
    public class Value1C_stdattr : Value1C
    {
        public MetaValue v_enum; // Значение системного перечисления
        public MetaObject v_metaobj; // Объект метаданных
        public MetaStandartAttribute v_stdattr;
        public int v_value;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_stdattr(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_stdattr;
            type = MetaTypeSet.mt_standart_attribute;
            v_metaobj = null;
            v_stdattr = null;
        }

        public override string presentation(bool english = false)
        {
            if (v_stdattr != null)
            {
                if (v_stdattr.Count)
                    return v_stdattr.GetName(english) + (v_value + 1);
                else
                    return v_stdattr.GetName(english);
            }
            return english ? "Unknown_standard_attribute" : "Неизвестный_стандартный_реквизит";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_stdattr == null)
                return false;
            str.Write(v_stdattr.GetName(english));
            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_metaobj != null && v_stdattr != null);
        }
    }

    /// <summary>
    /// Значение 1С - стандартная табличная часть
    /// </summary>
    public class Value1C_stdtabsec : Value1C
    {
        public MetaObject v_metaobj; // Объект метаданных
        public MetaStandartTabularSection v_stdtabsec;
        public int v_value;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_stdtabsec(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_stdtabsec;
            type = MetaTypeSet.mt_standart_tabular_section;
            v_metaobj = null;
            v_stdtabsec = null;
        }

        public override string presentation(bool english = false)
        {
            if (v_stdtabsec != null)
                return v_stdtabsec.GetName(english);

            return english ? "Unknown_standard_tabular_section" : "Неизвестная_стандартная_табличная_часть";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_stdtabsec == null)
                return false;
            str.Write(v_stdtabsec.GetName(english));
            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_metaobj != null && v_stdtabsec != null);
        }
    }

    public class Value1C_binary : Value1C
    {
        public ExternalFileFormat v_binformat; // Формат двоичных данных
        public MemoryStream v_binary;    // Двоичные данные

        public Value1C_binary(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_binary;
            v_binary = null;
        }

        public override string presentation(bool english = false)
        {
            
            if (v_binformat == ExternalFileFormat.eff_string)
                if (v_binary != null)
                {
                    v_binary.Seek(0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(v_binary, true);
                    string s = reader.ReadLine();
                    reader.Dispose();
                    return s;
                }
            if (english)
                switch (v_binformat)
                {
                    case ExternalFileFormat.eff_servalue:
                        return "SerializedValue";
                    case ExternalFileFormat.eff_text:
                        return "SourceCode";
                    case ExternalFileFormat.eff_tabdoc:
                        return "SpreadsheetDocument";
                    case ExternalFileFormat.eff_binary:
                        return "BinaryData";
                    case ExternalFileFormat.eff_activedoc:
                        return "ActiveDocument";
                    case ExternalFileFormat.eff_htmldoc:
                        return "HTMLDocument";
                    case ExternalFileFormat.eff_textdoc:
                        return "TextDocument";
                    case ExternalFileFormat.eff_geo:
                        return "GeographicalSchema";
                    case ExternalFileFormat.eff_kd:
                        return "DataCompositionSchema";
                    case ExternalFileFormat.eff_mkd:
                        return "DataCompositionAppearanceTemplate";
                    case ExternalFileFormat.eff_graf:
                        return "GraphicalSchema";
                    case ExternalFileFormat.eff_xml:
                        return "XML";
                    case ExternalFileFormat.eff_wsdl:
                        return "WSDL";
                    case ExternalFileFormat.eff_picture:
                        return "Picture";
                    case ExternalFileFormat.eff_string:
                        return "String";
                    default:
                        return "??? Unknown binary format";
                }
            switch (v_binformat)
            {
                case ExternalFileFormat.eff_servalue:
                    return "СериализованноеЗначение";
                case ExternalFileFormat.eff_text:
                    return "ИсходныйТекст";
                case ExternalFileFormat.eff_tabdoc:
                    return "ТабличныйДокумент";
                case ExternalFileFormat.eff_binary:
                    return "ДвоичныеДанные";
                case ExternalFileFormat.eff_activedoc:
                    return "ActiveДокумент";
                case ExternalFileFormat.eff_htmldoc:
                    return "HTMLДокумент";
                case ExternalFileFormat.eff_textdoc:
                    return "ТекстовыйДокумент";
                case ExternalFileFormat.eff_geo:
                    return "ГеографическаяСхема";
                case ExternalFileFormat.eff_kd:
                    return "СхемаКомпоновкиДанных";
                case ExternalFileFormat.eff_mkd:
                    return "МакетОформленияКомпоновкиДанных";
                case ExternalFileFormat.eff_graf:
                    return "ГрафическаяСхема";
                case ExternalFileFormat.eff_xml:
                    return "XML";
                case ExternalFileFormat.eff_wsdl:
                    return "WSDL";
                case ExternalFileFormat.eff_picture:
                    return "Картинка";
                case ExternalFileFormat.eff_string:
                    return "Строка";
                default:
                    return "??? Неизвестный формат двоичных данных";
            }
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_binformat == ExternalFileFormat.eff_string)
            {
                string s = presentation(english);
                StringBuilder sb = new StringBuilder(s);
                sb.Replace("\"", "\"\"");
                str.Write("\"");
                str.Write(sb.ToString());
                str.Write("\"");
            }
            else
                str.Write("<binary>");

            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            if (v_binary == null)
                return true;
            if (v_binary.Length == 0)
                return true;
            return false;
        }

        public string get_file_extension()
        {
            switch (v_binformat)
            {
                case ExternalFileFormat.eff_servalue:
                    return "err";
                case ExternalFileFormat.eff_tabdoc:
                    return "mxl";
                case ExternalFileFormat.eff_geo:
                    return "geo";
                case ExternalFileFormat.eff_kd:
                case ExternalFileFormat.eff_mkd:
                    return "bin";
                case ExternalFileFormat.eff_graf:
                    return "grs";
                case ExternalFileFormat.eff_xml:
                    return "xml";
                case ExternalFileFormat.eff_wsdl:
                    return "wsdl";
                case ExternalFileFormat.eff_picture:
                    file_format ff = get_file_format(v_binary);
                    switch (ff)
                    {
                        case file_format.ff_unknown:
                            break;
                        case file_format.ff_gif:
                            return "gif";
                        case file_format.ff_pcx:
                            return "pcx";
                        case file_format.ff_bmp:
                            return "bmp";
                        case file_format.ff_jpg:
                            return "jpg";
                        case file_format.ff_png:
                            return "png";
                        case file_format.ff_tiff:
                            return "tiff";
                        case file_format.ff_ico:
                            return "ico";
                        case file_format.ff_wmf:
                            return "wmf";
                        case file_format.ff_emf:
                            return "emf";
                        case file_format.ff_zip:
                            return "zip";
                    }
                    break;
                default:
                    break;
            }
            return "bin";
        }


    }


    //---------------------------------------------------------------------------
    // Сравнение свойств метаданных
    // struct MetaPropertyLess
    // {
    //     bool operator()(MetaProperty* const l, MetaProperty* const r) const
    // 	{
    // 	return l->name<r->name;
    // }
    // }


    /// <summary>
    /// Сравнение свойств метаданных
    /// </summary>
    public class MetaPropertyComparer : IComparer<MetaProperty>
    {
        public int Compare(MetaProperty l, MetaProperty r)
        {
            return String.Compare(l.Name, r.Name);
        }
    }


    //////////////////////////////////////////////////////////////


    /// <summary>
    /// Значение 1С объектного типа
    /// </summary>
    public class Value1C_obj : Value1C
    {
        public MetaContainer owner;
        // std::map<MetaProperty*, Value1C*, MetaPropertyLess> v_objprop; //Объект - коллекция свойств
        public SortedDictionary<MetaProperty, Value1C>       v_objprop;   //Объект - коллекция свойств
        //public List<SortedDictionary<MetaProperty, Value1C>> v_objpropv;  // Коллекция свойств в векторе
        public List<KeyValuePair<MetaProperty, Value1C>>     v_objpropv;  // Коллекция свойств в векторе
        public List<Value1C>                                 v_objcol;    //Объект (kv_obj или kv_metaobj???) - коллекция упорядоченных элементов
        public SortedDictionary<string, int>                 globalvars;  // Коллекция глобальных переменных со значениями

        // TODO : Очень пристально надо разбираться с этим
        // Не до конца понятна механика происходящего здесь
        public void fillpropv()
        {
            int i = 0;
            int count = 0;

            if (v_objpropv.Count > 0)
                v_objpropv.Clear();
            v_objpropv.Capacity = v_objprop.Count;


            SortedDictionary<MetaProperty, Value1C> pi = new SortedDictionary<MetaProperty, Value1C>(new MetaPropertyComparer());

            foreach (var item_pi in v_objprop)
            {
                v_objpropv.Add(item_pi);
                // if(pi->second) pi->second->index = count;
            }
            for (i = 0; i < v_objcol.Count; ++i, count++)
            {
                if (v_objcol[i] != null)
                    v_objcol[i].index = count;

            }

        }

        public static bool compare(MetaProperty p, Value1C v)
        {
            switch (p.defaultvaluetype)
            {
                case DefaultValueType.dvt_novalue:
                    return false;
                case DefaultValueType.dvt_bool:
                    if (v.kind != KindOfValue1C.kv_bool)
                        return false;
                    if (((Value1C_bool)v).v_bool != p.dv_union_type.dv_bool)
                        return false;
                    break;
                case DefaultValueType.dvt_number:
                    if (v.kind != KindOfValue1C.kv_number)
                        return false;
                    if (((Value1C_number)v).v_number != p.dv_union_type.dv_number)
                        return false;
                    break;
                case DefaultValueType.dvt_string:
                    if (v.kind != KindOfValue1C.kv_string)
                        return false;
                    if (((Value1C_string)v).v_string.CompareTo(p.dv_union_type.dv_string) != 0)
                        return false;
                    break;
                case DefaultValueType.dvt_date:
                    if (v.kind != KindOfValue1C.kv_date)
                        return false;
                    // TODO : Необходимо подумать как реализовать
                    // if (memcmp(((Value1C_date*)v)->v_date, p->dv_date, 7) != 0) return false;
                    break;
                case DefaultValueType.dvt_undef:
                    if (v.kind != KindOfValue1C.kv_undef)
                        return false;
                    break;
                case DefaultValueType.dvt_null:
                    if (v.kind != KindOfValue1C.kv_null)
                        return false;
                    break;
                case DefaultValueType.dvt_type:
                    if (v.kind != KindOfValue1C.kv_type)
                        return false;
                    if (((Value1C_type)v).v_type != p.dv_union_type.dv_type)
                        return false;
                    break;
                case DefaultValueType.dvt_enum:
                    if (v.kind != KindOfValue1C.kv_enum)
                        return false;
                    if (((Value1C_enum)v).v_enum != p.dv_union_type.dv_enum)
                        return false;
                    break;
                default:
                    break;
            }
            return true;
        }

        public Value1C_obj(Value1C_obj _parent, MetaContainer _owner) : base(_parent) 
        {
            owner = _owner;
            kind = KindOfValue1C.kv_obj;
        }

        public override string presentation(bool english = false)
        {
            Value1C_obj vo;
            Value1C v;
            Value1C_enum ve;
            Value1C_number vn;
            Value1C v2;
            uint i;
            String s = "";
            String s2 = "";

            if (!(type is null))
            {
                if ((type.Name.CompareTo("МногоязычнаяСтрока") == 0) || (type.Name.CompareTo("МногоязычнаяСтрокаВнутр") == 0))
                {
                    if (v_objcol.Count > 0)
                    {
                        vo = (Value1C_obj)v_objcol[0];
                        v = vo.getproperty("Строка");
                        if (v != null)
                            return v.presentation(english);
                    }
                }
                else if (type.Name.CompareTo("Реквизит") == 0)
                {
                    v = getproperty("Реквизит");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ТабличнаяЧасть") == 0)
                {
                    v = getproperty("ТабличнаяЧасть");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("СтрокаНаЯзыке") == 0 || type.Name.CompareTo("СтрокаНаЯзыкеВнутр") == 0)
                {
                    v = getproperty("КодЯзыка");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ВариантHTMLДокумента") == 0)
                {
                    v = getproperty("КодЯзыка");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("КлючИЗначение") == 0)
                {
                    v = getproperty("Ключ");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ЭлементПрав") == 0)
                {
                    v = getproperty("Право");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОграничениеДоступаКДанным") == 0)
                {
                    v = getproperty("Право");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ПраваНаОбъект") == 0)
                {
                    v = getproperty("Объект");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОбъектМетаданныхСсылка") == 0)
                {
                    v = getproperty("ОбъектМетаданных");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОписаниеСтандартногоРеквизита") == 0)
                {
                    v = getproperty("СтандартныйРеквизит");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОписаниеСтандартнойТабличнойЧасти") == 0)
                {
                    v = getproperty("СтандартнаяТабличнаяЧасть");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОписаниеТипа") == 0)
                {
                    v = getproperty("Тип");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ФайлКартинки") == 0)
                {
                    v = getproperty("ИмяФайла");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ГруппаКоманд") == 0)
                {
                    v = getproperty("Группа");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОписаниеТипов") == 0)
                {
                    for (i = 0; i < v_objcol.Count; ++i)
                    {
                        v = v_objcol[(int)i];
                        if (v != null)
                        {
                            s += (s.Length == 0 ? "" : ", ");
                            s += v.presentation(english);
                        }
                    }
                    return s;
                }
                else if (type.Name.CompareTo("РасширенноеИмяXML") == 0)
                {
                    v = getproperty("URIПространстваИмен");
                    v2 = getproperty("ЛокальноеИмя");
                    if (v != null && v2 != null) 
                    {
                        s = "{";
                        s += v.presentation(english);
                        s = "}";
                        s += v2.presentation(english);
                        return s;
                    }
                }
                else if (type.Name.CompareTo("СочетаниеКлавиш") == 0)
                {
                    v = getproperty("Модификатор");
                    v2 = getproperty("Клавиша");
                    if (v != null && v2 != null)
                    {
                        s += v2.presentation(english);
                        if (s.Length == 2)
                            if (s[1] == '_')
                                s = s.Substring(2, 1);

                        return v.presentation(english) + s;
                    }
                }
                else if (type.Name.CompareTo("ЭлементСоставаОбщегоРеквизита") == 0)
                {
                    v = getproperty("Метаданные");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ЭлементСоставаФункциональнойОпции") == 0)
                {
                    v = getproperty("Объект");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("Ссылка") == 0)
                {
                    v = getproperty("Ссылка");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("Ссылка") == 0)
                {
                    v = getproperty("Ссылка");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ПредопределенноеЗначение") == 0 || type.Name.CompareTo("ПредопределенноеЗначение2") == 0)
                {
                    v = getproperty("Тип");
                    v2 = getproperty("Значение");
                    if (v != null)
                        s = v.presentation(english);
                    else
                        s = "";

                    if (v2 != null)
                        s2 = v2.presentation(english);
                    else
                        s2 = "";

                    if (string.IsNullOrEmpty(s))
                    {
                        if (english)
                            s = "<Empty type>";
                        else
                            s = "<Пустой тип>";
                    }
                    if (string.IsNullOrEmpty(s2))
                    {
                        if (english)
                            s2 = "<Empty ref>";
                        else
                            s2 = "<Пустая ссылка>";
                    }
                    return s + "." + s2;
                }
                else if (type.Name.CompareTo("Цвет") == 0 || type.Name.CompareTo("ЦветВнутр") == 0)
                {
                    ve = (Value1C_enum)getproperty("Вид");
                    if (ve != null)
                        if (ve.kind == KindOfValue1C.kv_enum)
                        {
                            if (ve.v_enum.GetName(false).CompareTo("АвтоЦвет") == 0)
                                return english ? "Auto" : "Авто";
                            v2 = getproperty("Значение");
                            if (v2 != null)
                                return v2.presentation(english);
                            return ve.presentation(english);
                        }
                }
                else if (type.Name.CompareTo("Шрифт") == 0 || type.Name.CompareTo("ШрифтВнутр") == 0)
                {
                    ve = (Value1C_enum)getproperty("Вид");
                    if (ve != null)
                        if (ve.kind == KindOfValue1C.kv_enum)
                        {
                            if (ve.v_enum.GetName(false).CompareTo("АвтоШрифт") == 0)
                                return english ? "Auto" : "Авто";
                            if (ve.v_enum.GetName(false).CompareTo("ЭлементСтиля") == 0)
                            {
                                v = getproperty("Значение");
                                if (v != null)
                                    return v.presentation(english);
                            }
                        }
                    v = getproperty("Имя");
                    if (v != null)
                    {
                        s = v.presentation(english);
                        vn = (Value1C_number)getproperty("Размер");
                        if (vn != null)
                            if (vn.kind == KindOfValue1C.kv_number)
                            {
                                s += ",";
                                s += (vn.v_number / 10);
                            }
                        return s;
                    }
                    v = getproperty("Значение");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ЭлементСоставаПланаОбмена") == 0)
                {
                    v = getproperty("Метаданные");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ЗначениеРоли") == 0)
                {
                    v = getproperty("Роль");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ОграничениеПолей") == 0)
                {
                    v = getproperty("Поля");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("СтандартнаяПанель") == 0)
                {
                    v = getproperty("СтандартнаяПанель");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("ПоложениеИнтерфейса") == 0)
                {
                    v = getproperty("Положение");
                    if (v != null)
                        return v.presentation(english);
                }
                else if (type.Name.CompareTo("МетаСсылка") == 0)
                {
                    v = getproperty("Объект");
                    if (v != null)
                        s = v.presentation(english);
                    else
                        s = "";

                    v = getproperty("СтандартнаяТабличнаяЧасть");
                    if (v != null)
                    {
                        s += ".";
                        s += v.presentation(english);
                    }
                    return s;
                }
                else if (type.Name.CompareTo("ЭлементИнтерфейса") == 0)
                {
                    v = getproperty("Вид");
                    if (v != null)
                        if (v.kind == KindOfValue1C.kv_enum)
                        {
                            if ( ((Value1C_enum)v).v_enum.Name.CompareTo("Группа") == 0 )
                                return v.presentation(english);

                            v = getproperty("СтандартнаяПанель");
                            s = v.presentation(english);
                            if (string.IsNullOrEmpty(s))
                            {
                                return english ? "<StandardPanel>" : "<СтандартнаяПанель>";
                            }
                            return s;
                        }
                }
                else if (type.Name.CompareTo("ПолеСОграничением") == 0)
                {
                    v = getproperty("Реквизит");
                    if (v != null)
                        s = v.presentation(english);
                    else
                        s = "";

                    // TODO : Необходимо тщательно перепроверить !!!!!!!!!!!!!!!!!
                    if (s.IndexOf(".") == 0)
                    {
                        v = getproperty("ТабличнаяЧасть");
                        if (v != null)
                        {
                            s2 = v.presentation(english);
                            if (!string.IsNullOrEmpty(s2))
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    s2 += ".";
                                    s2 += s;
                                }
                                return s2;
                            }
                        }
                    }
                    return s;
                }
                else if (type.Name.CompareTo("ПоляСОграничением") == 0)
                {
                    if (v_objcol.Count == 0)
                        return english ? "<Other fields>" : "<Прочие поля>";
                    for (i = 0; i < v_objcol.Count; ++i)
                    {
                        v = v_objcol[(int)i];
                        if (v != null)
                        {
                            s += s.Length == 0 ? "" : ", ";
                            s += v.presentation(english);
                        }
                    }
                    return s;
                }
                v = getproperty("Имя");
                if (v != null)
                {
                    return v.presentation(english);
                }
                v = getproperty("НомерПараметра");
                if (v != null)
                {
                    return v.presentation(english);
                }
            }
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            Value1C v = null;
            if (type == null)
                return "";
            if (type.Name.CompareTo("СтрокаНаЯзыке") == 0 || type.Name.CompareTo("СтрокаНаЯзыкеВнутр") == 0)
            {
                v = getproperty("Строка");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("КлючИЗначение") == 0)
            {
                v = getproperty("Значение");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("ЭлементПрав") == 0)
            {
                v = getproperty("Доступность");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("СвязьПараметраВыбора") == 0)
            {
                v = getproperty("Реквизит");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("КолонкаДереваЗначений") == 0)
            {
                v = getproperty("ТипЗначения");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("КолонкаТаблицыЗначений") == 0)
            {
                v = getproperty("ТипЗначения");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("ЭлементСоставаПланаОбмена") == 0)
            {
                v = getproperty("АвтоРегистрация");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("ЗначениеРоли") == 0)
            {
                v = getproperty("Значение");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("ПараметрВыбора") == 0)
            {
                v = getproperty("Значение");
                if (v != null)
                    return v.presentation(english);
            }
            else if (type.Name.CompareTo("ОграничениеПолей") == 0)
            {
                v = getproperty("Ограничение");
                if (v != null)
                    return v.presentation(english);
            }
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            string _is;
            string npath;
            string _npath;
            string pname;
            string fname = "";
            StreamWriter strp; // свойства
            StreamWriter strc; // коллекция
            FileStream fs;
            FileStream fsb;
            int i, j;
            Value1C v;
            MetaProperty p;
            bool needtype;
            MetaType ct;
            ExportType exporttype;
            Value1C_binary vb;
            bool isfirst;         // Признак, что файл "свой", т.е. выводятся на первом уровне, а не вложенно.

            if (indent < 0)
            {
                isfirst = true;
                indent = 0;
            }
            else
            {
                isfirst = (str != null) ? false : true;
            }

            // Свойства
            if (v_objpropv.Count != 0)
            {
                if (str != null)
                {
                    if (!isfirst)
                    {
                        indent++;
                        if (kind == KindOfValue1C.kv_obj || kind == KindOfValue1C.kv_extobj)
                            str.Write("{\r\n");
                    }
                    strp = str;
                }
                else
                {
                    fs = new FileStream(path + (english ? "\\!Properties" : "\\!Свойства"), FileMode.CreateNew);
                    fs.Write(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
                    strp = new StreamWriter(fs, Encoding.UTF8, 1024);
                }

                // _is = new string()
                //  is = String(indentstring, indent);
                // TODO: С этим надо разбираться подробно
                _is = indentstring;
                for (i = 0; i < v_objpropv.Count; ++i)
                {
                    v = v_objpropv[i].Value;
                    if (v == null)
                        continue;
                    if (v.IsEmpty())
                        continue;
                    p = v_objpropv[i].Key;
                    if (compare(p, v))
                        continue;
                    pname = p.GetName(english);
                    exporttype = p.Exporttype;
                    if (exporttype == ExportType.et_default)
                        if (v.type != null)
                            exporttype = v.type.ExportType;
                    if (exporttype == ExportType.et_default)
                        if (v.kind == KindOfValue1C.kv_binary)
                            if (((Value1C_binary)v).v_binformat != ExternalFileFormat.eff_servalue)
                                exporttype = ExportType.et_file;
                    needtype = false;
                    if (v.type != null)
                    {
                        if (p.Types.Count != 1)
                            needtype = true;
                        else if (p.Types[0] == MetaTypeSet.mt_typedescrinternal && v.type == MetaTypeSet.mt_type) ;
                        else if (p.Types[0] != v.type)
                            needtype = true;
                    }
                    if (exporttype == ExportType.et_catalog)
                    {
                        npath = path + "\\" + pname;
                        System.IO.Directory.CreateDirectory(npath);
                        if (v.kind == KindOfValue1C.kv_obj || v.kind == KindOfValue1C.kv_metaobj || v.kind == KindOfValue1C.kv_extobj)
                        {
                            // TODO : Надо разбираться с этим обязательно!!!!!!!!!!!!!!!!!!!!!!
                            // ((Value1C_obj*)v)->owner->ExportThread((Value1C_obj*)v, npath, english);
                        }
                        else
                            v.Export(npath, null, 0, english);
                        if (needtype || str != null)
                        {
                            strp.Write(_is);
                            strp.Write(pname);
                            strp.Write(" = (");
                            strp.Write(v.type.GetName(english));
                            strp.Write(")@\"");
                            strp.Write(pname);
                            strp.Write("\"\r\n");

                        }

                    }
                    if (exporttype == ExportType.et_file)
                    {
                        if (v.kind == KindOfValue1C.kv_binary)
                        {
                            vb = (Value1C_binary)v;
                            if (type.GetName(false).CompareTo("ШаблонОграничений") == 0)
                            {
                                fname = ((Value1C_string)(getproperty("Имя"))).v_string + ".rt";
                            }
                            else
                            {
                                if (str != null && vb.v_binformat == ExternalFileFormat.eff_picture)
                                {
                                    if (vb.parent.type.Name.CompareTo("ФайлКартинки") == 0)
                                        fname = pname + "." + ((Value1C_string)vb.parent.getproperty("ИмяФайла")).v_string;
                                    else
                                    {
                                        fname = "";
                                        // TODO: Надо обязательно разбираться дополнительно
                                        //fname = String::IntToHex(_crc32(vb->v_binary), 8) + L"." + vb->get_file_extension();
                                    }
                                }
                            }
                            npath = path + "\\" + fname;
                            fsb = new FileStream(npath, FileMode.CreateNew);
                            //fsb.CopyTo
                            vb.v_binary.CopyTo(fsb);
                            fsb.Dispose();
                        }
                        else
                        {
                            if (type.GetName(false).CompareTo("ШаблонОграничений") == 0)
                                fname = ((Value1C_string)(getproperty("Имя"))).v_string + ".rt";
                            else
                            {
                                fname = pname;
                                if (v.kind == KindOfValue1C.kv_string)
                                    fname += ".bsl"; // Костыль для модулей управляемых форм
                            }
                            npath = path + "\\" + fname;
                            fsb = new FileStream(npath, FileMode.CreateNew);
                            fsb.Write(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
                            strc = new StreamWriter(fsb, Encoding.UTF8, 1024);
                            if (v.kind == KindOfValue1C.kv_string)
                                strc.Write(((Value1C_string)v).v_string);
                            else
                                v.Export(path, strc, 0, english);
                            strc.Dispose();
                            fsb.Dispose();
                        }
                        strp.Write(_is);
                        strp.Write(pname);
                        strp.Write(" = ");
                        if (needtype)
                        {
                            strp.Write("(");
                            strp.Write(v.type.GetName(english));
                            strp.Write(")");
                        }
                        strp.Write("%\"");
                        strp.Write(fname);
                        strp.Write("\"\r\n");
                    }
                    else
                    {
                        strp.Write(_is);
                        strp.Write(pname);
                        strp.Write(" = ");
                        if (needtype)
                        {
                            strp.Write("(");
                            strp.Write(v.type.GetName(english));
                            strp.Write(")");
                        }
                        v.Export(path, strp, indent, english);
                        strp.Write("\r\n");

                    }

                }
                if (str != null)
                {
                    if (!isfirst)
                    {
                        indent--;
                        // str->Write(String(indentstring, indent));
                        // TODO : Может быть совсем не то что должно быть...
                        str.Write(_is);
                        str.Write("}");
                    }
                }
                else
                {
                    strp.Dispose();
                    //fs.Dispose();
                }
            }

            // Коллекция
            _is = "";
            if (v_objcol.Count != 0)
            {
                ct = null;
                if (type != null)
                    if (type.CollectionTypes.Count() == 1)
                        ct = type.CollectionTypes[0];
                if (!isfirst)
                {
                    if (v_objpropv.Count != 0)
                    {
                        str.Write("\r\n");
                        // str->Write(String(indentstring, indent));
                        str.Write(_is);
                    }
                    indent++;
                    str.Write("[\r\n");
                    // TODO: Пока не очень понятно что это такое
                    _is = "============================================================================";
                    for (i = 0; i < v_objcol.Count; ++i)
                    {
                        v = v_objcol[i];
                        if (v == null)
                            continue; // нехорошо, но пока приходится
                        //if(v->isempty()) continue; // Неизвестно пока, можно ли так жестко
                        str.Write(_is);
                        needtype = false;
                        if (v.type != null)
                        {
                            if (ct != null)
                                needtype = true;
                            else if (v.type != ct)
                                needtype = true;
                        }
                        if (needtype)
                        {
                            str.Write("(");
                            str.Write(v.type.GetName(english));
                            str.Write(")");
                        }
                        v.Export(path, str, indent, english);
                        str.Write("\r\n");
                    }
                    indent--;
                    //str->Write(String(indentstring, indent));
                    str.Write(_is);
                    str.Write("]");
                }
                else
                {
                    fs = new FileStream(path + (english ? "\\!Order" : "\\!Порядок"), FileMode.CreateNew);
                    fs.Write(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
                    strc = new StreamWriter(fs, Encoding.UTF8, 1024);
                    for (i = 0; i < v_objcol.Count; ++i)
                    {
                        v = v_objcol[i];
                        pname = v.presentation(english);
                        if (string.IsNullOrEmpty(pname))
                            pname = "[" + i + "]";
                        npath = path + "\\" + pname;
                        Directory.CreateDirectory(npath);
                        needtype = false;
                        if (v.type != null)
                        {
                            if (ct == null)
                                needtype = true;
                            else if (v.type != ct)
                                needtype = true;
                        }

                        if (v.kind == KindOfValue1C.kv_obj || v.kind == KindOfValue1C.kv_metaobj || v.kind == KindOfValue1C.kv_extobj)
                        {
                            // TODO : Необходимо реализовывать
                            //((Value1C_obj*)v)->owner->ExportThread((Value1C_obj*)v, npath, english);
                        }
                        else
                            v.Export(npath, null, 0, english);

                        if (needtype)
                        {
                            strc.Write("(");
                            strc.Write(v.type.GetName(english));
                            strc.Write(")");
                        }
                        strc.Write(pname);
                        strc.Write("\r\n");
                    }
                    strc.Dispose();
                    fs.Dispose();
                }
            }

            // j = thrs.size();
            // 
            // for (i = 0; i < j; ++i)
            // {
            //     thrs[i]->WaitFor();
            //     delete thrs[i];
            // }

            return true;
        }

        public override bool IsEmpty()
        {
            int i;
            Value1C v;
            MetaProperty p;

            if (this == null)
                return true;

            for (i = 0; i < v_objpropv.Count; ++i)
            {
                v = v_objpropv[i].Value;
                if (v != null)
                    continue;
                if (!v.IsEmpty())
                {
                    p = v_objpropv[i].Key;
                    if (!compare(p, v))
                        return false;
                }
            }

            for (i = 0; i < v_objcol.Capacity; ++i)
            {
                v = v_objcol[i];
                if (v != null)
                    continue;
                if (!v.IsEmpty())
                    return false;
            }
            return true;
        }

        public virtual Value1C getproperty(MetaProperty mp)
        {
            if (v_objprop.TryGetValue(mp, out Value1C val))
                return val;
            else
                return null;
        }

        public virtual Value1C getproperty(string prop)
        {
            if (type == null)
                return null;
            MetaProperty pn = type.GetProperty(prop);
            if (pn != null)
                return getproperty(pn);
            return null;
        }

        public Class getclass(bool immediately = false)
        {
            int i = 0;
            Class cl = null;

            for (Value1C_obj v = this; v.parent != null; v = v.parent)
            {
                if (v.index >= 0)
                {
                    i = v.index;
                    if (i < v.parent.v_objpropv.Capacity)
                    {
                        cl = v.parent.v_objpropv[i].Key._class;
                        if (immediately)
                            return cl;
                        if (cl != null)
                            return cl;
                    }
                }
            }
            return null;
        }

    }

    //////////////////////////////////////////////////////////////
    /// <summary>
    /// Значение 1С - объект метаданных
    /// </summary>
    public class Value1C_metaobj : Value1C_obj
    {
        public MetaObject v_metaobj;

        public SortedDictionary<MetaGeneratedType, GeneratedType> v_objgentypes; // коллекция генерируемых типов
        public List<PredefinedValue>                              v_prevalues;   // коллекция предопределенных элементов

        public Value1C_metaobj(Value1C_obj _parent, MetaContainer _owner) : base(_parent, _owner)
        {
            kind = KindOfValue1C.kv_metaobj;
            v_metaobj = null;
        }

        public override string presentation(bool english = false)
        {
            if (v_metaobj != null)
                return v_metaobj.GetName(english);
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            if (type is null)
                return "";

            if (type.Name.CompareTo("ОбъектМетаданных: Реквизит") == 0)
            {
                Value1C v = getproperty("Тип");
                if (v != null)
                    return v.presentation(english);
            }
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            StreamWriter strp = null;
            bool res;

            if (str != null)
            {
                indent++;
                str.Write("{\r\n");
                strp = str;
            }
            else
            {
                FileStream fs = new FileStream(path + (english ? "\\!Properties" : "\\!Свойства"), FileMode.CreateNew);
                fs.Write(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
                strp = new StreamWriter(fs, Encoding.UTF8, 1024);
            }

            // is = String(indentstring, indent);
            string is1 = ""+ indentstring;
            strp.Write(is1);

            strp.Write(english ? "!Meta = {\r\n" : "!Мета = {\r\n");

            // is2 = String(indentstring, indent + 1);
            string is2 = "" + indentstring;

            //is3 = String(indentstring, indent + 2);
            string is3 = "" + indentstring;

            strp.Write(is2);
            if (english)
                strp.Write("ID = ");
            else
                strp.Write("ИД = ");

            //strp.Write(GUID_to_string(v_metaobj.UID));
            strp.Write(v_metaobj.UID.ToString());

            strp.Write("\r\n");
            if (v_objgentypes.Count != 0)
            {
                strp.Write(is2);
                if (english)
                    strp.Write("GeneratedTypes = {\r\n");
                else
                    strp.Write("ГенерирумыеТипы = {\r\n");

                foreach (var item_v_objgentypes in v_objgentypes)
                {
                    strp.Write(is3);
                    strp.Write(item_v_objgentypes.Key.GetName(english));
                    strp.Write(" = ");
                    strp.Write(item_v_objgentypes.Value.typeuid.ToString());
                    strp.Write(", ");
                    strp.Write(item_v_objgentypes.Value.valueuid.ToString());
                    strp.Write("\r\n");
                }
                strp.Write(is2);
                strp.Write("}\r\n");

            }
            strp.Write(is1);
            strp.Write("}\r\n");

            //res = Value1C_obj.Export(path, strp, indent - 1, english);
            res = base.Export(path, strp, indent - 1, english);

            if (str != null)
            {
                strp.Dispose();
            }

            return res;
        }

        public override bool IsEmpty()
        {
            if (this is null)
                return true;
            return false;
        }

    }

    /// <summary>
    /// Значение 1С - ссылка на объект метаданных
    /// </summary>
    public class Value1C_refobj : Value1C
    {
        public MetaObject v_metaobj;
        public Guid v_uid;

        public Value1C_refobj(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_refobj;
            v_metaobj = null;
            v_uid = new Guid();
        }
        public override string presentation(bool english = false)
        {
            if (v_metaobj != null)
                return v_metaobj.GetFullName(english);

            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_metaobj == null)
                return false;
            str.Write(v_metaobj.GetFullName(english));
            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_metaobj != null);
        }

    }


    /// <summary>
    /// Значение 1С - ссылка на предопределенное значение
    /// </summary>
    public class Value1C_refpre : Value1C
    {
        public PredefinedValue v_prevalue; // Предопределенное значение (kv_refpre)

        public Value1C_refpre(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_refpre;
            v_prevalue = null;
        }

        public override string presentation(bool english = false)
        {
            if (v_prevalue != null)
                return v_prevalue.name;

            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_prevalue == null)
                return false;
            str.Write(v_prevalue.getfullname(english));
            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_prevalue != null);
        }

    }

    /// <summary>
    /// Значение 1С - право
    /// </summary>
    public class Value1C_right : Value1C
    {
        public MetaRight v_right; // Право

        public Value1C_right(Value1C_obj _parent) : base(_parent)
        {
            kind = KindOfValue1C.kv_right;
            v_right = null;
        }

        public override string presentation(bool english = false)
        {
            if (v_right != null)
                return v_right.GetName(english);

            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            if (v_right == null)
                return false;
            str.Write(v_right.GetName(english));
            return true;
        }

        public override bool IsEmpty()
        {
            if (this == null)
                return true;
            return (v_right != null);
        }

    }

    /// <summary>
    /// Значение 1С - внешний объект
    /// </summary>
    public class Value1C_extobj : Value1C_obj
    {
        public bool opened; // признак, открыт ли внешний объект (т.е. распознан, или нет)
        public string path; // путь внешнего файла
        Guid metauid;

        public Value1C_extobj(Value1C_obj _parent, MetaContainer _owner, string _path, MetaType _type, Guid _metauid) : base(_parent,_owner)
        {
            path = _path;
            type = _type;
            metauid = _metauid;
            kind = KindOfValue1C.kv_extobj;
            opened = false;
        }

        public void open()
        {
            // TODO : После реализации MetaContainer
        }

        public void close()
        {
        }

        public override Value1C getproperty(MetaProperty mp)
        {
            open();
            return base.getproperty(mp);
        }

        public override Value1C getproperty(string prop)
        {
            open();
            return base.getproperty(prop);
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return true;
        }

        public override bool IsEmpty()
        {
            if (!opened)
                return false;
            return base.IsEmpty();
        }

    }


}
