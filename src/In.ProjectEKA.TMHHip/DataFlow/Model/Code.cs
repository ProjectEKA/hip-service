using System.Collections.Generic;

namespace In.ProjectEKA.TMHHip.DataFlow.Model
{
    public class Code
    {
        public Code(string text)
        {
            Text = text;
        }

        public Code(List<Coding> coding, string text)
        {
            Coding = coding;
            Text = text;
        }

        public List<Coding> Coding { get; set; }
        public string Text { set; get; }
    }

    public class Coding
    {
        public string System { get; set; }
        public string Code { get; set; }
        public string Display { get; set; }

        public Coding(string system, string code, string display)
        {
            System = system;
            Code = code;
            Display = display;
        }

        public Coding(string code, string display)
        {
            Code = code;
            Display = display;
        }
    }
}