using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Assets.Scripts
{
    public static class XmlUtil
    {
        public static string GetXml(Action<XmlWriter> a)
        {
            var s = new XmlWriterSettings();
            s.ConformanceLevel = ConformanceLevel.Fragment;
            s.Encoding = System.Text.UTF8Encoding.UTF8;
            s.Indent = true;
            // To make it easier for me to interoperate with Cygwin
            s.NewLineChars = "\n";

            var sb = new StringBuilder();

            using (var w = XmlWriter.Create(sb, s))
            {
                a(w);
                w.Flush();
            }

            // Seems like XmlWriter might not put a newline after the last line
            sb.Append("\n");

            return sb.ToString();
        }

        public static Action<string, Action> MakeWecClosure(XmlWriter w)
        {
            return (name, a) => {
                w.WriteStartElement(name);
                a();
                w.WriteEndElement();
            };
        }

        public static Action<string, string> MakeWesClosure(XmlWriter w)
        {
            return (name, value) => {
                w.WriteElementString(name, value);
            };
        }

        static Encoding Utf8
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}
