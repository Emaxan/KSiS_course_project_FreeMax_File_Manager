using System.IO;

namespace FreeMax_File_Manager {
	public class FileElement: FileSystemInfo, ISelectable {
		public FileElement(FileSystemInfo fileInfo) { FileInfo = fileInfo; }

		public override string FullName => FileInfo.FullName;

		public override bool Exists => FileInfo.Exists;

		public override string Name => FileInfo.Name;

		public bool IsFolder => (FileInfo.Attributes&FileAttributes.Directory) == FileAttributes.Directory;

		private FileSystemInfo FileInfo{ get; }

		public bool IsSelected{ get; private set; }

		public override void Delete() => FileInfo.Delete();

		public override string ToString() => ((FileInfo.Attributes & FileAttributes.Hidden) != 0 ? "(hidden)" : "") + (IsFolder ? "[ " + Name + " ]" : Name);

		void ISelectable.Select() { IsSelected = true; }
		void ISelectable.UnSelect() { IsSelected = false; }
	}
}
