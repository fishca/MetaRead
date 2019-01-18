using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace MetaRead
{

    public class Tree
    {
        public static event EventHandler EventError;
        public static string MsgError { get; set; }
        public static void ShowMessage(string _MsgError)
        {
            MsgError = _MsgError;
            EventError?.Invoke(null, null);  // запускаем подписчиков на событие        
        }

        public string Value;  // Значение в дереве

        public Node_Type type;

        public int num_subnode; // количество подчиненных

        public Tree parent;     // +1

        public Tree next;       // 0
        public Tree prev;       // 0

        public Tree first;      // -1
        public Tree last;       // -1

        public uint index;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_type"></param>
        /// <param name="_parent"></param>
        public Tree(string _value, Node_Type _type, Tree _parent)
        {
            Value  = _value;
            type   = _type;
            parent = _parent;

            num_subnode = 0;
            index = 0;

            if (parent != null)
            {
                parent.num_subnode++;

                prev = parent.last;

                if (prev != null)
                {
                    prev.next = this;
                    index = prev.index + 1;
                }
                else
                {
                    parent.first = this;
                }

                parent.last = this;
            }
            else
            {
                prev = null;
            }

            next = null;
            first = null;
            last = null;
        }

        public Tree AddChild(string _value = "", Node_Type _type = Node_Type.nd_empty)
        {
            return new Tree(_value, _type, this);
        }

        public Tree AddNode()
        {
            return new Tree("", Node_Type.nd_empty, parent);
        }

        public string Get_Value()
        {
            return Value;
        }

        public Node_Type Get_Type()
        {
            return type;
        }

        public void Set_Value(string v, Node_Type t)
        {
            Value = v;
            type = t;
        }

        public Tree Get_Subnode(int _index)
        {
            if (_index >= num_subnode)
                return null;
            Tree t = first;
            while (_index > 0)
            {
                t = t.next;
                --_index;

            }
            return t;
        }

        public Tree Get_Subnode(string node_name)
        {
            Tree t = first;
            while (t != null)
            {
                if (t.Value == node_name)
                    return t;
                t = t.next;
            }
            return null;
        }

        public Tree Get_Next()
        {
            return next;
        }

        public Tree Get_Parent()
        {
            return parent;
        }

        public Tree Get_First()
        {
            return first;
        }

        public Tree this[int _index]
        {
            get
            {
                if (this != null)
                    return this;
                Tree ret = first;
                while (_index > 0)
                {
                    if (ret != null)
                        ret = ret.next;
                    --_index;
                }
                return ret;
            }
            
        }

        public void OutText(ref string text)
        {
            Node_Type lt = Node_Type.nd_unknown;

            if (num_subnode != 0)
            {
                if (text.Length != 0)
                    text += Environment.NewLine;

                text += "{";
                Tree t = first;
                while (t != null)
                {
                    t.OutText(ref text);
                    lt = t.type;
                    t = t.next;
                    if (t != null)
                        text += ",";
                }
                if (lt == Node_Type.nd_list)
                    text += Environment.NewLine;
                text += "}";
            }
            else
            {
                switch (type)
                {
                    case Node_Type.nd_string:
                        text += "\"";

                        //text += string.Re Replace(value, L"\"", L"\"\"", _ReplaceAll);
                        text += this.Value.Replace("\"", "\"\"");
                        text += "\"";
                        break;
                    case Node_Type.nd_number:
                    case Node_Type.nd_number_exp:
                    case Node_Type.nd_guid:
                    case Node_Type.nd_list:
                    case Node_Type.nd_binary:
                    case Node_Type.nd_binary2:
                    case Node_Type.nd_link:
                    case Node_Type.nd_binary_d:
                        text += this.Value;
                        break;
                    default:
                        //if(err) err->AddError(L"Ошибка вывода потока. Пустой или неизвестный узел.");
                        break;
                }

            }


        }

        public string Path()
        {
            string p = "";

            if (this != null)
                return ":??";
            for (Tree t = this; t.parent != null; t = t.parent)
            {
                p = ":" + t.index + p;
            }
            return p;
        }

        public static Node_Type ClassificationValue(string Value)
        {
            string exp_number     = "^-?\\d+$";
            string exp_number_exp = "^-?\\d+(\\.?\\d*)?((e|E)-?\\d+)?$";
            string exp_guid       = "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
            string exp_binary     = "^#base64:[0-9a-zA-Z\\+=\\r\\n\\/]*$";
            string exp_binary2    = "^[0-9a-zA-Z\\+=\\r\\n\\/]+$";
            string exp_link       = "^[0-9]+:[0-9a-fA-F]{32}$";
            string exp_binary_d   = "^#data:[0-9a-zA-Z\\+=\\r\\n\\/]*$";

            if (Value.Length == 0) return Node_Type.nd_empty;

            Match m = Regex.Match(Value, exp_number, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_number;

            m = Regex.Match(Value, exp_number_exp, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_number_exp;

            m = Regex.Match(Value, exp_guid, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_guid;

            m = Regex.Match(Value, exp_binary, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_binary;

            m = Regex.Match(Value, exp_link, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_link;

            m = Regex.Match(Value, exp_binary2, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_binary2;

            m = Regex.Match(Value, exp_binary_d, RegexOptions.IgnoreCase);
            if (m.Success) return Node_Type.nd_binary_d;

            return Node_Type.nd_unknown;
        }

        public static Tree Parse_1Cstream(Stream str, string err, string path)
        {
            StringBuilder __curvalue__ = new StringBuilder("");

            string curvalue = "";

            _state state = _state.s_value;

            int i = 0;

            Char sym = '0';

            int _sym = 0;

            Node_Type nt = Node_Type.nd_unknown;

            Tree ret = new Tree("", Node_Type.nd_list, null);

            Tree t = ret;

            //StreamReader reader = new StreamReader(str, true);
            StreamReader reader = new StreamReader(str, Encoding.ASCII, true); // т.к. файл в кодировке win1251

            for (i = 1, _sym = reader.Read(); _sym >= 0; i++, _sym = reader.Read())
            {
                
                sym = (Char)_sym;
                //if(i % 0x100000 == 0) if(err) err->Status(String(i/0x100000) + L" MB");
                switch (state)
                {
                    case _state.s_value:
                        switch (sym)
                        {
                            case ' ':  // space
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case '"':
                                __curvalue__.Clear();
                                state = _state.s_string;
                                break;
                            case '{':
                                t = new Tree("", Node_Type.nd_list, t);
                                break;
                            case '}':
                                if (t.Get_First() != null)
                                    t.AddChild("", Node_Type.nd_empty);
                                t = t.Get_Parent();
                                //if (t != null)
                                if (t is null)
                                {
                                    // Ошибка формата потока. Лишняя закрывающая скобка }.
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }");
                                    ret = null;
                                    return null;
                                }
                                state = _state.s_delimitier;
                                break;
                            case ',':
                                t.AddChild("", Node_Type.nd_empty);
                                break;
                            default:
                                __curvalue__.Clear();
                                __curvalue__.Append(sym);
                                state = _state.s_nonstring;
                                break;

                        }
                        break;
                    case _state.s_delimitier:
                        switch (sym)
                        {
                            case ' ':  // space
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case ',':
                                state = _state.s_value;
                                break;
                            case '}':
                                t = t.Get_Parent();
                                //if (t != null)
                                if (t is null)
                                {
                                    // Ошибка формата потока. Лишняя закрывающая скобка }.
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }");
                                    ret = null;
                                    return null;
                                }
                                //state = _state.s_delimitier;
                                break;
                            default:
                                // Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.
                                ShowMessage("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.");
                                __curvalue__.Clear();
                                __curvalue__.Append(sym);
                                ret = null;
                                return null;
                        }
                        break;
                    case _state.s_string:
                        if (sym == '"')
                        {
                            state = _state.s_quote_or_endstring;
                        }
                        else
                        {
                            __curvalue__.Append(sym);
                        }
                        break;
                    case _state.s_quote_or_endstring:
                        if (sym == '"')
                        {
                            __curvalue__.Append(sym);
                            state = _state.s_string;
                        }
                        else
                        {
                            t.AddChild(__curvalue__.ToString(), Node_Type.nd_string);
                            switch (sym)
                            {
                                case ' ':
                                case '\t':
                                case '\r':
                                case '\n':
                                    state = _state.s_delimitier;
                                    break;
                                case ',':
                                    state = _state.s_value;
                                    break;
                                case '}':
                                    t = t.Get_Parent();
                                    //if (t != null)
                                    if (t is null)
                                    {
                                        // Ошибка формата потока. Лишняя закрывающая скобка }.
                                        ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }.");
                                        ret = null;
                                        return null;
                                    }
                                    state = _state.s_delimitier;
                                    break;
                                default:
                                    // Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.
                                    ShowMessage("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.");
                                    ret = null;
                                    return null;

                            }
                        }
                        break;
                    case _state.s_nonstring:
                        switch (sym)
                        {
                            case ',':
                                curvalue = __curvalue__.ToString();
                                nt = ClassificationValue(curvalue);
                                if (nt == Node_Type.nd_unknown)
                                {
                                    // Ошибка формата потока. Неизвестный тип значения.
                                    ShowMessage("Ошибка формата потока. Неизвестный тип значения.");
                                }
                                t.AddChild(curvalue, nt);
                                state = _state.s_value;
                                break;
                            case '}':
                                curvalue = __curvalue__.ToString();
                                nt = ClassificationValue(curvalue);
                                if (nt == Node_Type.nd_unknown)
                                {
                                    // Ошибка формата потока. Неизвестный тип значения.
                                    ShowMessage("Ошибка формата потока. Неизвестный тип значения.");
                                }
                                t.AddChild(curvalue, nt);
                                t = t.Get_Parent();
                                //if (t != null)
                                if (t is null)
                                {
                                    // Ошибка формата потока. Лишняя закрывающая скобка }.
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }.");
                                    ret = null;
                                    return null;
                                }
                                state = _state.s_delimitier;
                                break;
                            default:
                                __curvalue__.Append(sym);
                                break;
                        }
                        break;
                    default:
                        if (1 != 1)
                        {
                            // Ошибка формата потока. Неизвестный режим разбора.
                            ShowMessage("Ошибка формата потока. Неизвестный режим разбора.");
                        }
                        ret = null;
                        return null;
                }
            } // end of for

            if (state == _state.s_nonstring)
            {
                curvalue = __curvalue__.ToString();
                nt = ClassificationValue(curvalue);
                if (nt == Node_Type.nd_unknown)
                {
                    // Ошибка формата потока. Неизвестный тип значения.
                    ShowMessage("Ошибка формата потока. Неизвестный тип значения.");
                }
                t.AddChild(curvalue, nt);
            }
            if (state == _state.s_quote_or_endstring)
            {
                t.AddChild(__curvalue__.ToString(), Node_Type.nd_string);
            }
            if (state != _state.s_delimitier)
            {
                ret = null;
                return null;
            }
            if (t != ret)
            {
                // Ошибка формата потока. Не хватает закрывающих скобок } в конце текста разбора. 
                ShowMessage("Ошибка формата потока. Не хватает закрывающих скобок } в конце текста разбора. ");
                ret = null;
                return null;
            }
            return ret;
        } // end of parce of tree

        /// <summary>
        /// Парсинг скобочного дерева 1С
        /// </summary>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Tree Parse_1Ctext(String text, String path)
        {

            StringBuilder __curvalue__ = new StringBuilder("");

            String curvalue = "";
            Tree ret = new Tree("", Node_Type.nd_list, null);
            Tree t = ret;
            int len = text.Length;
            int i = 0;
            char sym = '0';
            Node_Type nt = Node_Type.nd_unknown;

            _state state = _state.s_value;

            //for (i = 0; i <= len-1; i++)
            for (i = 1; i < len; i++)
            {
                sym = text[i];

                if (String.IsNullOrEmpty(sym.ToString())) break;

                switch (state)
                {
                    case _state.s_value:
                        switch (sym)
                        {
                            case ' ': // space
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case '"':

                                __curvalue__.Clear();
                                state = _state.s_string;
                                break;

                            case '{':

                                t = new Tree("", Node_Type.nd_list, t);
                                break;

                            case '}':

                                if (t.Get_First() != null)
                                    t.AddChild("", Node_Type.nd_empty);

                                t = t.Get_Parent();

                                if (t == null)
                                {
                                    //if (msreg) msreg->AddError("Ошибка формата потока. Лишняя закрывающая скобка }.", "Позиция", i, "Путь", path);
                                    //delete ret;
                                    //String msreg = $"Ошибка формата потока. Лишняя закрывающая скобка. Позиция: { i }, Путь: {path}";
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }. Позиция " + i + ", Путь " + path);
                                    ret = null;
                                    return null;
                                }
                                state = _state.s_delimitier;
                                break;

                            case ',':

                                t.AddChild("", Node_Type.nd_empty);
                                break;

                            default:

                                __curvalue__.Clear();
                                __curvalue__.Append(sym);
                                state = _state.s_nonstring;

                                break;
                        }
                        break;
                    case _state.s_delimitier:
                        switch (sym)
                        {
                            case ' ': // space
                            case '\t':
                            case '\r':
                            case '\n':
                                break;
                            case ',':
                                state = _state.s_value;
                                break;
                            case '}':
                                t = t.Get_Parent();
                                if (t == null)
                                {
                                    /*
                                    if (msreg) msreg->AddError("Ошибка формата потока. Лишняя закрывающая скобка }.",
                                         "Позиция", i,
                                         "Путь", path);
                                    */
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }. Позиция " + i + ", Путь " + path);
                                    ret = null;
                                    return null;
                                }
                                break;
                            default:
                                /*
                                if (msreg) msreg->AddError("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.",
                                     "Символ", sym,
                                     "Код символа", tohex(sym),
                                     "Путь", path);
                                */
                                ShowMessage("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя. Символ " + sym + ", Код символа " + sym.ToString() + ", Путь " + path);
                                ret = null;
                                return null;
                        }
                        break;
                    case _state.s_string:
                        if (sym == '"')
                        {
                            state = _state.s_quote_or_endstring;
                        }
                        else
                            __curvalue__.Append(sym);
                        break;
                    case _state.s_quote_or_endstring:
                        if (sym == '"')
                        {
                            __curvalue__.Append(sym);
                            state = _state.s_string;
                        }
                        else
                        {
                            t.AddChild(__curvalue__.ToString(), Node_Type.nd_string);
                            switch (sym)
                            {
                                case ' ': // space
                                case '\t':
                                case '\r':
                                case '\n':
                                    state = _state.s_delimitier;
                                    break;
                                case ',':
                                    state = _state.s_value;
                                    break;
                                case '}':
                                    t = t.Get_Parent();
                                    if (t == null)
                                    {
                                        /*
                                        if (msreg) msreg->AddError("Ошибка формата потока. Лишняя закрывающая скобка }.",
                                             "Позиция", i,
                                             "Путь", path);
                                        */
                                        ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка }. Позиция " + i.ToString() + ", Путь " + path);
                                        ret = null;
                                        return null;
                                    }
                                    state = _state.s_delimitier;
                                    break;
                                default:
                                    /*
                                    if (msreg) msreg->AddError("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя.",
                                         "Символ", sym,
                                         "Код символа", tohex(sym),
                                         "Путь", path);
                                    */
                                    ShowMessage("Ошибка формата потока. Ошибочный символ в режиме ожидания разделителя. Символ " + sym + ", Код символа " + sym.ToString() + ", Путь " + path);
                                    ret = null;
                                    return null;
                            }
                        }
                        break;
                    case _state.s_nonstring:
                        switch (sym)
                        {
                            case ',':
                                curvalue = __curvalue__.ToString();
                                nt = ClassificationValue(curvalue);
                                if (nt == Node_Type.nd_unknown)
                                {
                                    /*
                                    if (msreg) msreg->AddError("Ошибка формата потока. Неизвестный тип значения.",
                                      "Значение", curvalue,
                                      "Путь", path);
                                      */
                                    ShowMessage("Ошибка формата потока. Неизвестный тип значения. Значение " + curvalue + ", Путь " + path);
                                }
                                t.AddChild(curvalue, nt);
                                state = _state.s_value;
                                break;
                            case '}':
                                curvalue = __curvalue__.ToString();

                                nt = ClassificationValue(curvalue);

                                if (nt == Node_Type.nd_unknown)
                                {
                                    //if (msreg) msreg->AddError("Ошибка формата потока. Неизвестный тип значения.", "Значение", curvalue, "Путь", path);
                                    ShowMessage("Ошибка формата потока. Неизвестный тип значения. Значение " + curvalue + ", Путь " + path);
                                }
                                t.AddChild(curvalue, nt);
                                t = t.Get_Parent();
                                if (t == null)
                                {
                                    /*
                                    if (msreg) msreg->AddError("Ошибка формата потока. Лишняя закрывающая скобка }.",
                                         "Позиция", i,
                                         "Путь", path);
                                    */
                                    ShowMessage("Ошибка формата потока. Лишняя закрывающая скобка. Позиция " + i.ToString() + ", Путь " + path);
                                    ret = null;
                                    return null;
                                }
                                state = _state.s_delimitier;
                                break;
                            default:
                                __curvalue__.Append(sym);
                                break;
                        }
                        break;
                    default:
                        /*
                        if (msreg) msreg->AddError("Ошибка формата потока. Неизвестный режим разбора.",
                             "Режим разбора", tohex(state),
                             "Путь", path);
                             */
                        ShowMessage("Ошибка формата потока. Неизвестный режим разбора. Режим разбора " + state.ToString() + ", Путь " + path);
                        ret = null;
                        return null;
                }
            }


            if (state == _state.s_nonstring)
            {
                curvalue = __curvalue__.ToString();
                nt = ClassificationValue(curvalue);
                if (nt == Node_Type.nd_unknown)
                {
                    /*
                    if (msreg) msreg->AddError("Ошибка формата потока. Неизвестный тип значения.",
                      "Значение", curvalue,
                      "Путь", path);
                      */
                    ShowMessage("Ошибка формата потока. Неизвестный тип значения. Значение: " + curvalue + ", Путь " + path);
                }
                t.AddChild(curvalue, nt);
            }
            else
                if (state == _state.s_quote_or_endstring)
                t.AddChild(__curvalue__.ToString(), Node_Type.nd_string);
            else
                if (state != _state.s_delimitier)
            {
                /*
                if (msreg) msreg->AddError("Ошибка формата потока. Незавершенное значение",
                     "Режим разбора", tohex(state),
                     "Путь", path);
                */
                ShowMessage("Ошибка формата потока. Незавершенное значение. Режим разбора: " + state.ToString() + ", Путь " + path);
                ret = null;
                return null;
            }

            if (t != ret)
            {
                /*
                if (msreg) msreg->AddError("Ошибка формата потока. Не хватает закрывающих скобок } в конце текста разбора.",
                     "Путь", path);
                    */
                ShowMessage("Ошибка формата потока. Не хватает закрывающих скобок в конце текста разбора, путь " + path);
                ret = null;
                return null;
            }

            return ret;

        } // End parse_1Ctext

        public int Get_num_subnode()
        {
            return num_subnode;
        }


    }
}
