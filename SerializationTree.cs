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
    public struct UTreeNode1
    {
        [FieldOffset(0)]
        public int num1;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(0)]
        public Guid uid1;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(0)]
        public MetaValue val1;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(0)]
        public MetaProperty prop1;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(0)]
        public MetaGeneratedType gentype;                // генерируемый тип (type == stt_gentype)
        [FieldOffset(0)]
        public ContainerVer vercon1;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(0)]
        public Version1C ver1C1;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(0)]
        public SerializationTreeClassType classtype;     // вид коллекции классов ((type == stt_classcol) 
        [FieldOffset(0)]
        public ClassParameter classpar1;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode2
    {
        [FieldOffset(0)]
        public int num2;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(0)]
        public Guid uid2;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(0)]
        public MetaValue val2;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(0)]
        public MetaProperty prop2;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(0)]
        public ContainerVer vercon2;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(0)]
        public Version1C ver1C2;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(0)]
        public ClassParameter classpar2;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }


    //---------------------------------------------------------------------------
    // Переменная дерева сериализации
    public class SerializationTreeVar
    {
        public String fname;
        public bool fcolcount;
        public bool fisglobal;
        public bool fisfix;
        public int ffixvalue;
        public List<VarValidValue> fvalidvalues;

        public SerializationTreeVar(Tree tr)
        {

        }

        public String Name
        {
            get { return fname; }
        }

        public bool ColCount
        {
            get { return fcolcount; }
        }

        public bool IsGlobal
        {
            get { return fisglobal; }
        }

        public bool IsFix
        {
            get { return fisfix; }
        }

        public int FixValue
        {
            get { return ffixvalue; }
        }

        public List<VarValidValue> validvalues
        {
            get { return fvalidvalues; }
        }

    }

    //---------------------------------------------------------------------------
    // Узел дерева сериализации
    public class SerializationTreeNode
    {
        SerializationTreeNode next; // следующий на этом уровне
        SerializationTreeNode first; // первый на подчиненном уровне
        SerializationTreeNode parent; // родитель
        uint index; // индекс на текущем уровне
        MetaType owner;

        SerializationTreeNodeType type;
        SerializationTreeCondition condition; // (type == stt_cond)

        SerializationTreeValueType typeval1; // Тип значения 1 (type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol)
        string str1; // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_var ИЛИ type == stt_elcol) И typeval1 = stv_string ИЛИ typeval1 = stv_var ИЛИ typeval1 = stv_globalvar)

        UTreeNode1 uTreeNode1;
        UTreeNode2 UTreeNode2;

    }

}
