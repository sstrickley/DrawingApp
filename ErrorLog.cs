namespace DrawingApp
{
    using System;
    using System.IO;
    using System.Reflection;

    class ErrorLog
    {
        private static string _filename = Path.Combine(
            Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location), "ErrorLog.log");

        public static void WriteToLog(string lineNumber, string message)
        {
            StreamWriter streamwriter = new StreamWriter(_filename, true);
            streamwriter.WriteLine("{0}: Line {1} -- {2}", DateTime.Now, lineNumber, message);
            streamwriter.Close();
        }
    }
}
