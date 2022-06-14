
using System;

namespace XLibGame
{
    public class EventContext
    {
        public IComparable type;
        public object sender;
        public object[] args;

        public override string ToString()
        {
            string arg = null;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if ((args.Length > 1 && args.Length - 1 == i) || args.Length == 1)
                    {
                        arg += args[i];
                    }
                    else
                    {
                        arg += args[i] + " , ";
                    }
                }
            }

            return type + " [ " + ((sender == null) ? "null" : sender.ToString()) + " ] " + " [ " + ((arg == null) ? "null" : arg.ToString()) + " ] ";
        }

        public EventContext Clone()
        {
            return new EventContext(type, args, sender);
        }

        public EventContext(IComparable type)
        {
            this.type = type;
        }

        public EventContext(IComparable type, params object[] args)
        {
            this.type = type;
            this.args = args;
        }

        public EventContext(IComparable type, object sender, params object[] args)
        {
            this.type = type;
            this.sender = sender;
            this.args = args;
        }

    }

}
