using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GeneralClasses;
using Microsoft.AspNet.SignalR;

namespace FileManagerService {
    public class PathChangetEventArgs: EventArgs {
        public string NewPath;
        public string OldPath;
    }

    public class MyHub: Hub {
        public event EventHandler<PathChangetEventArgs> PathChanged;

        public override Task OnConnected() {
            Logger.RecordEntry($"Client connected. ConnectionID is: {Context.ConnectionId}.");

            return base.OnConnected();
        }

        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(string userName,
                                             string domainName,
                                             string password,
                                             int logonType,
                                             int logonProvider,
                                             ref IntPtr phToken);

        public bool Authentication(string name, string domain, string password) {
            var ptr = IntPtr.Zero;
            var result = LogonUser(name, domain, password, 2, 0, ref ptr);
            if(!result) return false;
            Clients.Caller.Name = name;
            return true;
        }

        public string GetFolderContent(int attr, int negAttr, string path) {
            var files = new DirectoryInfo(path).GetFileSystemInfos()
                                               .Where(
                                                   f =>
                                                   ((f.Attributes&(FileAttributes) attr) != 0 &&
                                                    (f.Attributes&(FileAttributes) negAttr) == 0)).ToArray();
            if(!files.Any()) return null;
            
            return files.OrderByDescending(
                f => (f.Attributes&FileAttributes.Directory) == FileAttributes.Directory)
                        .ThenBy(f => f.Name)
                        .Select(
                            f =>
                            new FileElement(f).ToString() + "*" + new FileElement(f).FileInfo.FullName +
                            "*" + (new FileElement(f).IsFolder? "1" : "0"))
                        .Aggregate((res, cur) => res + ("|" + cur));
        }

        public string GetParent(string path) {
            var parent = new DirectoryInfo(path);
            parent = parent.Parent;
            return parent?.FullName ?? "null";
        }

        public string GetDirectoryOfFile(string path) {
            return new FileInfo(path).Directory?.FullName ?? "null";
        }

        public bool IsDirectoryExist(string path) {
            return new DirectoryInfo(path).Exists;
        }

        public void DeleteDirectory(string path) {
            new DirectoryInfo(path).Delete(true);
        }

        public void CreateDirectory(string path) {
            new DirectoryInfo(path).Create();
        }

        public void CopyDirectory(string item, string dest) {
            foreach(var directoryInfo in new DirectoryInfo(item).GetDirectories())
                CopyDirectory(directoryInfo.FullName, dest);
            foreach(var file in new DirectoryInfo(item).GetFiles()) {
                file.CopyTo(dest + '\\' + file.Name, true);
            }
        }

        public void MoveDirectory(string item, string dest) {
            new DirectoryInfo(item).MoveTo(dest);
        }

        public bool IsFileExist(string path) {
            return new FileInfo(path).Exists;
        }

        public void DeleteFile(string path) {
            new FileInfo(path).Delete();
        }

        public void CreateFile(string path) {
            new FileInfo(path).Create();
        }

        public void CopyFile(string item, string dest) {
            new FileInfo(item).CopyTo(dest, true);
        }

        public void MoveFile(string item, string dest){
            new FileInfo(item).MoveTo(dest);
        }

        public bool SetFileWatcher(string path) { return true; }

        public bool RemoveFileWathcer(string path) { return true; }

        public bool ResetFileWatcher(string path) { return true; }

        /* TODO Sample of progress reporting
        public async Task<string> DoLongRunningThing(IProgress<int> progress){
            for (int i = 0; i <= 100; i+=5){
                await Task.Delay(200);
                progress.Report(i);
            }
            return "Job complete!";
        }*/

        public override Task OnDisconnected(bool stopCalled) {
            Logger.RecordEntry(stopCalled
                                   ? $"Client {Context.ConnectionId} explicitly closed the connection."
                                   : $"Client {Context.ConnectionId} timed out .");

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected() {
            Logger.RecordEntry($"Client reconnected. ConnectionID is: {Context.ConnectionId}.");
            return base.OnReconnected();
        }
    }
}
