using System;

namespace Innocellence.WeChat
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogTypeFilter : Attribute
    {
        private readonly int _type;
        private readonly string _Content;

        public LogTypeFilter(int type,string content)
        {
            _type = type;
            _Content = content;
          
        }

        public int LogType { get { return _type; } }
        public string LogContent { get { return _Content; }}
    }
}