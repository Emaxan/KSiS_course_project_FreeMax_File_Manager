using System.IO;

namespace GeneralClasses {
	public class MyFileSystemWatcher: FileSystemWatcher {
		public MyFileSystemWatcher() { }

	    public int attr;
	    public int negAttr;

		public MyFileSystemWatcher(string path)
			: base(path) { }

		public MyFileSystemWatcher(string path, string filter)
			: base(path, filter) { }

		public void Start() { EnableRaisingEvents = true; }

		public void Stop() { EnableRaisingEvents = false; }
	}
}
