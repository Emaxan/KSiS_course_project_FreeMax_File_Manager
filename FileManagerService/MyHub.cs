using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GeneralClasses;
using Microsoft.AspNet.SignalR;

namespace FileManagerService {
    public class MyPanel {
        public string ConnectionId;
        public string Panel;
    }

    public class MyHub: Hub {
        private static Dictionary<MyFileSystemWatcher, MyPanel> FSWs = new Dictionary<MyFileSystemWatcher, MyPanel>(); 

        public override Task OnConnected() {
            Logger.RecordEntry($"Client connected. ConnectionID is: {Context.ConnectionId}.");
            var left = new MyFileSystemWatcher();
            left.Changed += (sender, args) => { FSWEvent(sender); };
            left.Created += (sender, args) => { FSWEvent(sender); };
            left.Deleted += (sender, args) => { FSWEvent(sender); };
            left.Renamed += (sender, args) => { FSWEvent(sender); };
            var right = new MyFileSystemWatcher();
            right.Changed += (sender, args) => { FSWEvent(sender); };
            right.Created += (sender, args) => { FSWEvent(sender); };
            right.Deleted += (sender, args) => { FSWEvent(sender); };
            right.Renamed += (sender, args) => { FSWEvent(sender); };
            FSWs.Add(left, new MyPanel {ConnectionId = Context.ConnectionId, Panel = "left"});
            FSWs.Add(right, new MyPanel {ConnectionId = Context.ConnectionId, Panel = "right"});
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled) {
            Logger.RecordEntry(stopCalled
                                   ? $"Client {Context.ConnectionId} explicitly closed the connection."
                                   : $"Client {Context.ConnectionId} timed out .");

            var watcher = FSWs.Where((pair => pair.Value.ConnectionId == Context.ConnectionId)).ToArray();
            foreach(var pair in watcher) {
                pair.Key.EnableRaisingEvents = false;
                FSWs.Remove(pair.Key);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected() {
            Logger.RecordEntry($"Client reconnected. ConnectionID is: {Context.ConnectionId}.");
            return base.OnReconnected();
        }

        private void FSWEvent(object sender) {
            var watch = sender as MyFileSystemWatcher;
            if (watch == null) return;
            var elems = GetFolderContent(watch.attr, watch.negAttr, watch.Path)
                .Split('|')
                .Select(el => new StringElement(el.Split('*')));
            Clients.Client(FSWs[watch].ConnectionId)
                   .UpdateSourceForPanel(FSWs[watch].Panel, elems);
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

        public string GetDirectoryOfFile(string path) { return new FileInfo(path).Directory?.FullName ?? "null"; }

        public string GetReadyDrives() {
            var drives = DriveInfo.GetDrives().Where(dr => dr.IsReady).ToArray();
            return drives.Select(
                f => f.VolumeLabel + "*" + f.RootDirectory.FullName + "*1")
                         .Aggregate((res, cur) => res + ("|" + cur));
        }

        public bool IsDirectoryExist(string path) { return new DirectoryInfo(path).Exists; }

        public bool DeleteDirectory(string path) {
            try {
                new DirectoryInfo(path).Delete(true);
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool CreateDirectory(string path) {
            try {
                new DirectoryInfo(path).Create();
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool CopyDirectory(string item, string dest) {
            try {
                foreach(var directoryInfo in new DirectoryInfo(item).GetDirectories())
                    CopyDirectory(directoryInfo.FullName, dest);
                foreach(var file in new DirectoryInfo(item).GetFiles()) {
                    file.CopyTo(dest + '\\' + file.Name, true);
                }
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool MoveDirectory(string item, string dest) {
            try {
                new DirectoryInfo(item).MoveTo(dest);
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool IsFileExist(string path) { return new FileInfo(path).Exists; }

        public bool DeleteFile(string path) {
            try {
                new FileInfo(path).Delete();
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool CreateFile(string path) {
            try {
                new FileInfo(path).Create();
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool CopyFile(string item, string dest) {
            try {
                new FileInfo(item).CopyTo(dest, true);
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public bool MoveFile(string item, string dest) {
            try {
                new FileInfo(item).MoveTo(dest);
                return true;
            }
            catch(Exception e) {
                Clients.Caller.Message(e.Message);
                return false;
            }
        }

        public void SetFileWatcher(string path, string panel, int attr, int negattr) {
            var watcher =
                FSWs.Where((pair => (pair.Value.ConnectionId == Context.ConnectionId) && (pair.Value.Panel == panel))).ToArray();
            if (watcher.Count() < 1) return;
            watcher[0].Key.attr = attr;
            watcher[0].Key.negAttr = negattr;
            watcher[0].Key.Path = path;
            watcher[0].Key.EnableRaisingEvents = true;
        }

        public void RemoveFileWathcer(string panel) {
            var watcher =
                FSWs.Where((pair => (pair.Value.ConnectionId == Context.ConnectionId) && (pair.Value.Panel == panel))).ToArray();
            if (watcher.Count() < 1) return;
            watcher[0].Key.EnableRaisingEvents = true;
        }

        public void ResetFileWatcher(string path, string panel, int attr, int negattr) {
            var watcher =
                FSWs.Where((pair => (pair.Value.ConnectionId == Context.ConnectionId) && (pair.Value.Panel == panel))).ToArray();
            if(watcher.Count()<1) return;
            watcher[0].Key.attr = attr;
            watcher[0].Key.negAttr = negattr;
            watcher[0].Key.EnableRaisingEvents = false;
            watcher[0].Key.Path = path;
            watcher[0].Key.EnableRaisingEvents = true;
        }

        //public async Task<string> DownloadFile(IProgress<int> progress) {
        //    for (var i = 0; i <= 100; i += 1)
        //    {
        //        await Task.Delay(200);
        //        progress.Report(i);
        //    }
        //    return "Job complete!";
        //}

        /* TODO Sample of progress reporting
        public async Task<string> DoLongRunningThing(IProgress<int> progress){
            for (int i = 0; i <= 100; i+=5){
                await Task.Delay(200);
                progress.Report(i);
            }
            return "Job complete!";
        }*/
    }
}