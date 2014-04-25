using System;
using System.IO;
using System.Text;

namespace HomeProjectWebForms.Helpers
{
    public static class ShellLogger
    {
        private const string Path = @"C:\SiteLog";

        public static void WriteLog(string name, string source)
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                var fs2 = new FileStream(Path + @"\" + name, FileMode.Append, FileAccess.Write);
                var sw = new StreamWriter(fs2, Encoding.Default);
                sw.WriteLine(source);
                sw.WriteLine();
                sw.Close();
            }
            catch (Exception ex)
            {
                WriteLog("ShellLogger.log", "Ошибка записи в файл", ex);
            }
        }

        public static void WriteLog(string FileName, string RussionErrorText, Exception ex)
        {
            WriteLog(FileName, RussionErrorText + Environment.NewLine);
            WriteLog(FileName,
                "Ошибка:" + Environment.NewLine + ex.Message + Environment.NewLine + "StackTrace:" +
                Environment.NewLine + ex.StackTrace);
        }
    }
}