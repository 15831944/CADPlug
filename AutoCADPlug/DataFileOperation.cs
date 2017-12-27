using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace AutoCADPlug
{
    public class DataFileOperation
    {
        /// <summary>
        /// 写TXT文件
        /// </summary>
        /// <param name="wordsLine"></param>
        /// <param name="fileName"></param>
        public static void WriteLine(string wordsLine, string fileName)
        {
            FileStream fsm = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fsm);
            sw.WriteLine(wordsLine);
            sw.Close();
        }
    }
}
