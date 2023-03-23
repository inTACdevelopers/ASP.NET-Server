using System.Collections;

namespace Server.Backend.Exeptions
{
    public class DataBaseExeption : Exception
    {
        public DataBaseExeption(string? message) : base(message)
        {

        }

        public override IDictionary Data => base.Data;

        public override string? HelpLink { get => base.HelpLink; set => base.HelpLink = value; }

        public override string Message => base.Message;

        public override string? Source { get => base.Source; set => base.Source = value; }

        public override string? StackTrace => base.StackTrace;
    }
}
