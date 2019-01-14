using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static MetaRead.APIcfBase;

namespace MetaRead
{

    [StructLayout(LayoutKind.Explicit)]
    public struct tUnion
    {
        [FieldOffset(0)]
        public bool dv_bool;
        [FieldOffset(100)]
        public int dv_number;
        [FieldOffset(200)]
        public string dv_string;
        [FieldOffset(300)]
        public char[] dv_date;
        [FieldOffset(400)]
        public MetaType dv_type;
        [FieldOffset(500)]
        public MetaValue dv_enum;
    }

    //Свойство метаданных
    // 
    public class MetaProperty : MetaBase
    {
        public List<MetaType> ftypes;
        public List<String> fstypes;
        public MetaType owner;
        public bool fpredefined;
        public ExportType fexporttype;
        public Class f_class;

        public DefaultValueType defaultvaluetype;

        /* пока непонятно на что заменить
            union
            {
              bool dv_bool;
              int dv_number;
              String* dv_string;
              char dv_date[7];
              MetaType* dv_type;
              MetaValue* dv_enum;
            }
        */

        //[StructLayout(LayoutKind.Explicit)]
        //public struct tUnion
        //{
        //    [FieldOffset(0)]
        //    public bool dv_bool;
        //    [FieldOffset(0)]
        //    public int dv_number;
        //    [FieldOffset(0)]
        //    public string dv_string;
        //    [FieldOffset(0)]
        //    public char[] dv_date;
        //    [FieldOffset(0)]
        //    public MetaType dv_type;
        //    [FieldOffset(0)]
        //    public MetaValue dv_enum;
        //}
        public tUnion dv_union_type;


        public MetaProperty(MetaType _owner, string _name, string _ename) : base(_name, _ename)
        {
            owner = _owner;
        }
        public MetaProperty(MetaType _owner, Tree tr)
        {
            Tree tt;
            Tree t;
            int num;
            Guid guid = new Guid();

            owner = _owner;

            tt = tr.Get_First();
            Name = tt.Get_Value();

            tt = tt.Get_Next();
            EName = tt.Get_Value();

            tt = tt.Get_Next();
            fpredefined = tt.Get_Value().CompareTo("1") == 0 ? true : false;

            tt = tt.Get_Next();
            fexporttype = (ExportType)Convert.ToUInt32(tt.Get_Value());

            tt = tt.Get_Next();
            string_to_GUID(tt.Get_Value(), ref guid);

            f_class = Class.GetClass(guid);

            // Типы
            tt = tt.Get_Next();
            t = tt.Get_First();
            num = Convert.ToInt32(t.Get_Value());
            for (int i = 0; i < num; ++i)
            {
                t = t.Get_Next();
                fstypes.Add(t.Get_Value());
            }
            defaultvaluetype = DefaultValueType.dvt_novalue;
        }

        public void FillTypes()
        {
            if (ftypes.Count != 0)
                ftypes.Clear();
            for (int i = 0; i < ftypes.Count; i++)
            {
                // TODO: обязательно проверить преобразование из INT in String
                ftypes.Add(owner.typeSet.GetTypeByName(i.ToString()));
            }
        }

        public List<MetaType> Types
        {
            get { return ftypes; }
        }

        public MetaType GetOwner()
        {
            return owner;
        }

        public bool Predefined
        {
            get
            {
                return fpredefined;
            }
        }

        public ExportType Exporttype
        {
            get { return fexporttype; }
        }

        public Class _class
        {
            get { return f_class; }
        }


    }

    //Сравнение свойств метаданных
    // 
    public static class MetaPropertyLess
    {
        public static bool Less(MetaProperty l, MetaProperty r)
        {
            return String.Compare(l.Name, r.Name) < 0 ? true : false;
        }
    }

}
