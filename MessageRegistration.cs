using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace MetaRead
{
    public enum MessageState
    {
        msEmpty      = -1,
        msSuccesfull = 0,
        msWarning    = 1,
        msInfo       = 2,
        msError      = 3,
        msWait       = 4,
        msHint       = 5
    }

    public class MessageRegistrator
    {
        // Признак отладочной информации
        private bool DebugMessage;

        public MessageRegistrator()
        {
            DebugMessage = false;
        }

        public void SetDebugMode(bool dstate)
        {
            DebugMessage = dstate;
        }

        public bool GetDebugMode()
        {
            return DebugMessage;
        }

        // Где-то в наследуемом классе должны переопределить обязательно
        public virtual void AddMessage(string description, MessageState mstate, StringCollection param = null)
        {
        }

        // Где-то в наследуемом классе должны переопределить обязательно
        public virtual void Status(string message)
        {
        }

        public void AddError(string description)
        {
            AddMessage(description, MessageState.msError);
        }

        public void AddError(string description, string parname1, string par1)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2, string parname3, string par3)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            ts.Add(parname6 + " = " + par6);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddError(string description, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6, string parname7, string par7)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            ts.Add(parname6 + " = " + par6);
            ts.Add(parname7 + " = " + par7);
            AddMessage(description, MessageState.msError, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            ts.Add(parname6 + " = " + par6);
            AddMessage(description, mstate, ts);
        }

        public void AddMessage_(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6, string parname7, string par7)
        {
            StringCollection ts = new StringCollection();
            ts.Add(parname1 + " = " + par1);
            ts.Add(parname2 + " = " + par2);
            ts.Add(parname3 + " = " + par3);
            ts.Add(parname4 + " = " + par4);
            ts.Add(parname5 + " = " + par5);
            ts.Add(parname6 + " = " + par6);
            ts.Add(parname7 + " = " + par7);
            AddMessage(description, mstate, ts);
        }

        public void AddDebugMessage(string description, MessageState mstate)
        {
            if (!DebugMessage)
                return;
            AddMessage(description, mstate);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2, parname3, par3);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2, parname3, par3, parname4, par4);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2, parname3, par3, parname4, par4, parname5, par5);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2, parname3, par3, parname4, par4, parname5, par5, parname6, par6);
        }

        public void AddDebugMessage(string description, MessageState mstate, string parname1, string par1, string parname2, string par2, string parname3, string par3, string parname4, string par4, string parname5, string par5, string parname6, string par6, string parname7, string par7)
        {
            if (!DebugMessage)
                return;
            AddMessage_(description, mstate, parname1, par1, parname2, par2, parname3, par3, parname4, par4, parname5, par5, parname6, par6);
        }

    }
}
