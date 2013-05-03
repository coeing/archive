using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ants.Source
{
    public static class Log
    {
        public static void Clear()
        {
#if DEBUG || STATS
            FileStream fs = new FileStream("debug.log", System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fs.Close();
#endif
        }

        public static void WriteLine(string log)
        {
#if DEBUG || STATS
            FileStream fs = new FileStream("debug.log", System.IO.FileMode.Append, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(log);
            sw.Close();
            fs.Close();
#endif
        }
    }
}
