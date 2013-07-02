using System;

namespace Spillville.Utilities
{
    class Message
    {
        public String Text { get; private set; }
        public TimeSpan Duration {get; private set;}
        public TimeSpan StartTime { get; set; }

        public Message(String msg)
        {
            this.Text = msg;
            this.Duration = TimeSpan.FromSeconds(5);
        }

        public Message(String msg, TimeSpan length)
        {
            this.Text = msg;
            this.Duration = length;
        }
    }
}
