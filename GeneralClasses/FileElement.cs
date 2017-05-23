using System;
using System.IO;

namespace GeneralClasses {
    [Serializable]
	public class FileElement: ISelectable {
        public FileElement() { }
        public FileElement(FileSystemInfo fileInfo) { FileInfo = fileInfo; }

		public bool IsFolder => (FileInfo.Attributes&FileAttributes.Directory) == FileAttributes.Directory;

        public FileSystemInfo FileInfo{ get; set; }
        
		public bool IsSelected{ get; set; }

		public override string ToString() => ((FileInfo.Attributes & FileAttributes.Hidden) != 0 ? "(hidden)" : "") + (IsFolder ? "[ " + FileInfo.Name + " ]" : FileInfo.Name);

		void ISelectable.Select() { IsSelected = true; }
		void ISelectable.UnSelect() { IsSelected = false; }
	}
}
