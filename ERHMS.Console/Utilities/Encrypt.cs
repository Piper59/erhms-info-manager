﻿using Epi;

namespace ERHMS.Console.Utilities
{
    public class Encrypt : Utility
    {
        public string Text { get; }

        public Encrypt(string text)
        {
            Text = text;
        }

        public override void Run()
        {
            Out.WriteLine(Configuration.Encrypt(Text));
        }
    }
}
