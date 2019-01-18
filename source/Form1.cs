using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace MetaRead
{
    public partial class Form1 : Form
    {
        public static Logger log;

        public Form1()
        {
            InitializeComponent();
            MetaTypeSet.EventError += MetaTypeSet_EventError; // подписываемся на событие EventError. Если оно произойдет, то запустить метод (Analyze_EventError).   
            Tree.EventError += Tree_EventError;
            
            #region Logs
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log_file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");


            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            log = LogManager.GetCurrentClassLogger();

            #endregion



        }

        private void Tree_EventError(object sender, EventArgs e)
        {
            textBox1.Text += string.Format("{0}", Tree.MsgError) + Environment.NewLine;
        }

        private void MetaTypeSet_EventError(object sender, EventArgs e)
        {
            textBox1.Text += string.Format("{0}", MetaTypeSet.MsgError) + Environment.NewLine;
        }

        public void Error(string str)
        {
            textBox1.Text += str + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name_cf = "1Cv8_Test_export.cf";
            string folder_cf = "D:\\work\\awa15-metaread-e7fe7d987355\\awa15-metaread-e7fe7d987355\\1C";

            ConfigStorage Storage = new ConfigStorageCFFile(folder_cf + "\\" + name_cf);
            // var Container = new MetaContainer(Storage);

            Form1.log.Info($"Добавляем все гуиды внутренних файлов...");
            if (((ConfigStorageCFFile)Storage).Cat.Files != null)
            {
                
                foreach (var item_v8 in ((ConfigStorageCFFile)Storage).Cat.Files)
                {
                    treeConfig.Nodes[0].Nodes.Add(item_v8.Key);
                    Form1.log.Info($"Добавили в дерево...{item_v8.Key}");
                }
            }

            Form1.log.Info($"Закончили добавление всех гуидов внутренних файлов...");

            var Container = new MetaContainer(Storage);


            #region Old_Code
            //              //MetaBase metaBase = new MetaBase("Справочник", "Reference");
            //              //Text = metaBase.GetName(true);
            //              
            //              V8Header_Struct v8Header_Struct = new V8Header_Struct();
            //              
            //              v8Header_Struct.Time_Create = new DateTime(2018, 5, 9, 10, 10, 10);
            //              v8Header_Struct.Time_Modify = new DateTime(2018, 5, 10, 10, 10, 10);
            //              v8Header_Struct.Zero = 999;
            //              
            //              // создаем объект BinaryFormatter
            //              //BinaryFormatter formatter = new BinaryFormatter();
            //              
            //              // получаем поток, куда будем записывать сериализованный объект
            //              //using (FileStream fs = new FileStream("D:\\work\\git_MetaRead\\bin\\Debug\\test.dat", FileMode.OpenOrCreate))
            //              //{
            //              //    formatter.Serialize(fs, v8Header_Struct);
            //              
            //              //Console.WriteLine("Объект сериализован");
            //              //}
            //              
            //              using (FileStream fs = new FileStream("D:\\work\\git_MetaRead\\bin\\Debug\\test.dat", FileMode.OpenOrCreate))
            //              {
            //                  using (BinaryReader reader = new BinaryReader(fs, Encoding.ASCII))
            //                  {
            //                      long tc = reader.ReadInt64();
            //                      long tm = reader.ReadInt64();
            //              
            //                      DateTime dateTime_tc = DateTime.FromBinary(tc);
            //                      DateTime dateTime_tm = DateTime.FromBinary(tm);
            //              
            //                      int zero = reader.ReadInt32();
            //              
            //                  }
            //                  //using (BinaryWriter writer = new BinaryWriter(fs,Encoding.ASCII))
            //                  //{
            //                  //    // записываем в файл значение каждого поля структуры
            //                  //    long tc = v8Header_Struct.Time_Create.ToBinary();
            //                  //    long tm = v8Header_Struct.Time_Modify.ToBinary();
            //                  //    writer.Write(tc);
            //                  //    writer.Write(tm);
            //                  //    writer.Write(v8Header_Struct.Zero);
            //              
            //                  //}
            //              }
            //              
            //              textBox1.Text += ("Новая текстовая строка"+Environment.NewLine);
            //              // Example usage
            //              Logger logger = LogManager.GetLogger("Example");
            //              logger.Trace("trace log message");
            //              logger.Debug("debug log message");
            //              logger.Info("info log message");
            //              logger.Warn("warn log message");
            //              logger.Error("error log message");
            //              logger.Fatal("fatal log message");
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Stream rstr;
            // string s = Application.ExecutablePath;
            // 
            // string path = Path.GetDirectoryName(s);
            // string path_log = path + "\\" + "MetaTree.txt";
            // if (File.Exists(path_log))
            //     rstr = new FileStream(path_log, FileMode.OpenOrCreate);
            // else
            //     rstr = new MemoryStream();
            // 
            // //MemoryStream rstr = new MemoryStream();
            // MetaTypeSet.StaticTypesLoad(rstr);
            // 
            // rstr.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int n1 = 2;
            int n2 = 5;

            int result = n2 * 3 + 20 / 2 * (n1--);

            Logger logger = LogManager.GetLogger("Example");
            logger.Info("result = " + result);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stream rstr;
            string s = Application.ExecutablePath;

            string path = Path.GetDirectoryName(s);
            string path_log = path + "\\" + "MetaTree1.txt";
            if (File.Exists(path_log))
                rstr = new FileStream(path_log, FileMode.OpenOrCreate);
            else
                rstr = new MemoryStream();

            //MemoryStream rstr = new MemoryStream();
            //this.textBox1.Text += "sdfsdfsdf"+Environment.NewLine;
            MetaTypeSet.StaticTypesLoad(rstr);

            rstr.Dispose();

        }

        private void treeConfig_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Text += e.Node.Text+Environment.NewLine;


        }
    }

    public class Messager : MessageRegistrator
    {
        private string LogFile;
        private TextBox MemoLog;
        private string _s;

        public Messager()
        {
        }

        public Messager(string _logfile, TextBox _MemoLog)
        {
            SetLogFile(_logfile);
            MemoLog = _MemoLog;
        }

        public void SetLogFile(string _logfile)
        {
            if (_logfile.Length > 0)
            {
                LogFile = Path.GetFullPath(_logfile);
                if (File.Exists(LogFile))
                    File.Delete(LogFile);
            }
            else
                LogFile = _logfile;

        }

        public override void AddMessage(string message, MessageState mstate, StringCollection param = null)
        {
            string s = "";
            s = DateTime.Now.ToString();
            s += " ";
            switch (mstate)
            {
                case MessageState.msEmpty:
                    s += "<>";
                    break;
                case MessageState.msSuccesfull:
                    s += "<ok>";
                    break;
                case MessageState.msWarning:
                    s += "<warning>";
                    break;
                case MessageState.msInfo:
                    s += "<info>";
                    break;
                case MessageState.msError:
                    s += "<error>";
                    break;
                case MessageState.msWait:
                    s += "<wait>";
                    break;
                case MessageState.msHint:
                    s += "<hint>";
                    break;
                default:
                    s += "<>";
                    break;
            }
            s += " ";
            s += message;
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++)
                {
                    s += (Environment.NewLine + "\t");
                    s += param[i];
                }
            }
            s += Environment.NewLine;
            _s = s;
        }

        public override void Status(string message)
        {
            base.Status(message);
        }

    }


}
