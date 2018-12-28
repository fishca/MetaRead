﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            if (parent is null)
                return "";
            if (type == mc.types->mt_config) return L"";
            j = parent->v_objpropv.size();
            return "";
        }

        public string fullpath(MetaContainer mc, bool english = false)
        {
            return "";
        }

    }

    public class Value1C_bool : Value1C
    {
        public bool v_bool;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_bool(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    public class Value1C_string : Value1C
    {
        public string v_string;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_string(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    public class Value1C_null : Value1C
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_null(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    public class Value1C_undef : Value1C
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_undef(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    public class Value1C_uid : Value1C
    {
        public Guid v_uid;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_uid(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    public class Value1C_enum : Value1C
    {
        public MetaValue v_enum; // Значение системного перечисления

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_parent"></param>
        public Value1C_enum(Value1C_obj _parent) : base(_parent)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

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
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }















    // public class Value1C<T> : Value1C
    // {
    //     public T v_value;
    //     public Value1C(Value1C_obj _parent) : base(_parent)
    //     {
    //     }
    // }


    //---------------------------------------------------------------------------
    // Сравнение свойств метаданных
    // struct MetaPropertyLess
    // {
    //     bool operator()(MetaProperty* const l, MetaProperty* const r) const
    // 	{
    // 	return l->name<r->name;
    // }
    // }

    public class MetaPropertyComparer : IComparer<MetaProperty>
    {
        public int Compare(MetaProperty l, MetaProperty r)
        {
            return String.Compare(l.Name, r.Name);
        }
    }




    //////////////////////////////////////////////////////////////
    public class Value1C_obj : Value1C
    {
        public MetaContainer owner;
        // std::map<MetaProperty*, Value1C*, MetaPropertyLess> v_objprop; //Объект - коллекция свойств
        public SortedDictionary<MetaProperty, Value1C>       v_objprop;   //Объект - коллекция свойств
        public List<SortedDictionary<MetaProperty, Value1C>> v_objpropv;  // Коллекция свойств в векторе
        public List<Value1C> v_objcol; //Объект (kv_obj или kv_metaobj???) - коллекция упорядоченных элементов
        public SortedDictionary<string, int> globalvars; // Коллекция глобальных переменных со значениями
        public void fillpropv()
        {
        }

        public static bool compare(MetaProperty p, Value1C v)
        {
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
            String s;
            String s2;

            if (!(type is null))
            {
                if ((type.Name.CompareTo("МногоязычнаяСтрока") == 0) || (type.Name.CompareTo("МногоязычнаяСтрокаВнутр") == 0))
                {
                    if (v_objcol.Count > 0)
                    {
                        vo = (Value1C_obj)v_objcol[0];
                        v = vo.ge
                    }
                }
            }
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }

    }

    //////////////////////////////////////////////////////////////

    public class Value1C_metaobj : Value1C_obj
    {
        public MetaObject v_metaobj;

        public SortedDictionary<MetaGeneratedType, GeneratedType> v_objgentypes; // коллекция генерируемых типов
        public List<PredefinedValue>                              v_prevalues;   // коллекция предопределенных элементов

        public Value1C_metaobj(Value1C_obj _parent, MetaContainer _owner) : base(_parent, _owner)
        {
        }

        public override string presentation(bool english = false)
        {
            return "";
        }

        public override string valuepresentation(bool english = false)
        {
            return "";
        }

        public override bool Export(string path, StreamWriter str, int indent, bool english = false)
        {
            return false;
        }

        public override bool IsEmpty()
        {
            return false;
        }

    }




}
