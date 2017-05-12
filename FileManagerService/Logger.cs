using System;
using System.IO;

namespace FileManagerService {
    internal static class Logger {
        private static readonly object _obj = new object();

        public static void RecordEntry(string msg) {
            lock(_obj) {
                using(var writer = new StreamWriter($"{Path.GetTempPath()}FreeMaxService.log", true)) {
                    //C:\Users\emaxa\AppData\Local\Temp\FreeMaxService.log
                    writer.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} {msg}");
                    writer.Flush();
                }
            }
        }
    }
}