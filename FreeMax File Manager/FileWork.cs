using System.IO;
using System.Linq;
using System.Windows;
using FreeMax_File_Manager.Windows;

namespace FreeMax_File_Manager {
	public static class FileWork {
		public static Window MainWindow;

		public static void Rename(FileElement fileElement) {
			var aw = new AdditionalWindow {
											MyTitle = "Переименование.",
											Text = $"Введите новое имя для {fileElement.Name}.",
											Attributes = -1,
											Owner = MainWindow,
											VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
											DriveSelection = false,
											ElemName = null,
											Rename = true
										};

			aw.ShowDialog();

			if(aw.Result == Results.Bad) return;

			if(fileElement.IsFolder) {
				var directoryInfo = new DirectoryInfo(fileElement.FullName).Parent;
				if(directoryInfo == null) return;
				if(!new DirectoryInfo(directoryInfo.FullName + '\\' + aw.NewName).Exists)
					new DirectoryInfo(fileElement.FullName).MoveTo(directoryInfo.FullName + '\\' + aw.NewName);
				else {
					var aw1 = new AdditionalWindow {
														MyTitle = "Подтвердите действие.",
														Text = $"Папка {aw.NewName} существует. Хотите её заменить?",
														Attributes = -1,
														Owner = MainWindow,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = false,
														ElemName = null,
														Rename = false
													};
					aw1.ShowDialog();
					if(aw1.Result == Results.Bad) return;
					Delete(new[] {new FileElement(new DirectoryInfo(directoryInfo.FullName + '\\' + aw.NewName))});
					new DirectoryInfo(fileElement.FullName).MoveTo(directoryInfo.FullName + '\\' + aw.NewName);
				}
			}
			else {
				var fileInfo = new FileInfo(fileElement.FullName).Directory;
				if(fileInfo == null) return;
				if(!new FileInfo(fileInfo.FullName + '\\' + aw.NewName).Exists)
					new FileInfo(fileElement.FullName).MoveTo(fileInfo.FullName + '\\' + aw.NewName);
				else {
					var aw1 = new AdditionalWindow {
														MyTitle = "Подтвердите действие.",
														Text = $"Файл {aw.NewName} существует. Хотите её заменить?",
														Attributes = -1,
														Owner = MainWindow,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = false,
														ElemName = null,
														Rename = false
													};
					aw1.ShowDialog();
					if(aw1.Result == Results.Bad) return;
					new FileInfo(fileInfo.FullName + '\\' + aw.NewName).Delete();
					new FileInfo(fileElement.FullName).MoveTo(fileInfo.FullName + '\\' + aw.NewName);
				}
			}
		}

		public static void CreateFile(FileSystemInfo fileElement) {
			var aw = new AdditionalWindow {
											MyTitle = "Создание файла.",
											Text = "Введите имя файла(с расширением)!",
											Owner = MainWindow,
											Attributes = -1,
											VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
											DriveSelection = false,
											ElemName = new DirectoryInfo(fileElement.FullName),
											Rename = false
										};
			aw.ShowDialog();
			if(aw.Result == Results.Bad) return;

			if(aw.NewElem.Exists) {
				var aw1 = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Файл {aw.Drive.Name} существует. Хотите заменить его?",
													Owner = MainWindow,
													Attributes = -1,
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
				aw1.ShowDialog();
				if(aw.Result == Results.Bad) return;
				aw.NewElem.Delete();
			}

			aw.NewElem.Create();
		}

		public static void CreateFolder(FileSystemInfo fileElement) {
			var aw = new AdditionalWindow {
											MyTitle = "Создание папки.",
											Text = "Введите имя папки!",
											Owner = MainWindow,
											Attributes = -1,
											VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
											DriveSelection = false,
											ElemName = new DirectoryInfo(fileElement.FullName),
											Rename = false
										};
			aw.ShowDialog();
			if(aw.Result == Results.Bad) return;

			if(aw.Drive.Exists) {
				var aw1 = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Папка {aw.Drive.Name} существует. Хотите заменить её?",
													Owner = MainWindow,
													Attributes = -1,
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
				aw1.ShowDialog();
				if(aw.Result == Results.Bad) return;
				aw.Drive.Delete();
			}

			aw.Drive.Create();
		}

		public static void DeleteElements(FileElement[] items) {
			if(items.Length == 0) return;

			var aw = new AdditionalWindow {
											Owner = MainWindow,
											Text = $"Вы уверены, что хотите удалить {items.Length} элемент(-а,-ов)?",
											MyTitle = "Подтвердите операцию.",
											VisibleButtons = (int) (Buttons.BtnYes|Buttons.BtnNo),
											Attributes = -1,
											DriveSelection = false,
											ElemName = null,
											Rename = false
										};
			aw.ShowDialog();

			if(aw.Result == Results.Bad) return;

			Delete(items);
		}

		private static void Delete(FileElement[] items) {
			foreach(var dir in items.Where(item => item.Exists)) {
				if(dir.IsFolder) {
					var item = new DirectoryInfo(dir.FullName);
					Delete(item.GetFileSystemInfos().Select(elem => new FileElement(elem)).ToArray());
				}
				dir.Delete();
			}
		}

		public static void CopyElements(FileElement[] items, FileSystemInfo destination) {
			if(items.Length == 0) return;

			var aw = new AdditionalWindow {
											Owner = MainWindow,
											Text = $"Вы уверены, что хотите скопировать {items.Length} элемент(-а,-ов)?",
											MyTitle = "Подтвердите операцию.",
											VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
											Attributes = -1,
											DriveSelection = false,
											ElemName = null,
											Rename = false
										};
			aw.ShowDialog();
			if(aw.Result == Results.Bad) return;

			foreach(var elem in items) {
				Copy(elem, destination);
			}
		}

		private static void Copy(FileElement item, FileSystemInfo destination) {
			if(item.IsFolder) {
				var dir = new DirectoryInfo(item.FullName);
				var dest = new DirectoryInfo(destination.FullName + '\\' + item.Name);
				if(dest.Exists) {
					var aw = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Папка {dest.Name} существует. Хотите выполнить слияние?",
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													Owner = MainWindow,
													Attributes = -1,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
					aw.ShowDialog();
					if(aw.Result == Results.Bad) return;
				}
				else
					dest.Create();
				foreach(var directoryInfo in dir.GetDirectories())
					Copy(new FileElement(directoryInfo), new FileElement(dest));
				foreach(var file in dir.GetFiles()) {
					file.CopyTo(dest.FullName + '\\' + file.Name, true);
				}
			}
			else {
				if(new FileInfo(destination.FullName + '\\' + item.Name).Exists) {
					var aw = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Файл {item.Name} существует. Хотите заместить его?",
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													Owner = MainWindow,
													Attributes = -1,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
					aw.ShowDialog();
					if(aw.Result == Results.Bad) return;
				}
				new FileInfo(item.FullName).CopyTo(destination.FullName + '\\' + item.Name, true);
			}
		}

		public static void MoveElements(FileElement[] items, FileSystemInfo destination) {
			if(items.Length == 0) return;

			var aw = new AdditionalWindow {
											Owner = MainWindow,
											Text = $"Вы уверены, что хотите переместить {items.Length} элемент(-а,-ов)?",
											MyTitle = "Подтвердите операцию.",
											VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
											Attributes = -1,
											DriveSelection = false,
											ElemName = null,
											Rename = false
										};
			aw.ShowDialog();

			if(aw.Result == Results.Bad) return;

			foreach(var item in items) {
				if(item.IsFolder) {
					if(new DirectoryInfo(destination.FullName + '\\' + item.Name).Exists) {
						aw = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Папка {item.Name} существует. Хотите выполнить слияние?",
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													Owner = MainWindow,
													Attributes = -1,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
						aw.ShowDialog();
						if(aw.Result == Results.Bad) return;
					}
					new DirectoryInfo(item.FullName).MoveTo(destination.FullName + '\\' + item.Name);
				}
				else {
					if(new FileInfo(destination.FullName + '\\' + item.Name).Exists) {
						aw = new AdditionalWindow {
													MyTitle = "Подтвердите операцию.",
													Text = $"Файл {item.Name} существует. Хотите заменить его?",
													VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
													Owner = MainWindow,
													Attributes = -1,
													DriveSelection = false,
													ElemName = null,
													Rename = false
												};
						aw.ShowDialog();
						if(aw.Result == Results.Bad) return;
						new FileInfo(destination.FullName + '\\' + item.Name).Delete();
					}
					new FileInfo(item.FullName).MoveTo(destination.FullName + '\\' + item.Name);
				}
			}
		}
	}
}
