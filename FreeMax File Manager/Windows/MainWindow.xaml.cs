using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using FreeMax_File_Manager.Properties;

namespace FreeMax_File_Manager.Windows {
	/// <summary>
	///     Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();
			FileWork.MainWindow = this;
			LbLeftPanel.IsActive = true;
			LbLeftPanel.MySelectedItem = 0;
			LbLeftPanel.Focus();
			var t = LbLeftPanel.Path;
			LbLeftPanel.Path = t;
			t = LbRightPanel.Path;
			LbRightPanel.Path = t;
		}

		private void wMain_KeyDown(object sender, KeyEventArgs e) {
			switch(e.Key) {
				case Key.System:
					SystemKeyDown(e);
					break;
				case Key.F3:
					FileWork.CreateFolder(LbLeftPanel.IsActive? new FileInfo(LbLeftPanel.Path) : new FileInfo(LbRightPanel.Path));
					break;
				case Key.F4:
					FileWork.CreateFile(LbLeftPanel.IsActive? new FileInfo(LbLeftPanel.Path) : new FileInfo(LbRightPanel.Path));
					break;
				case Key.F5:
					FileWork.Rename(
						(FileElement)((LbLeftPanel.IsActive
													? LbLeftPanel 
													: LbRightPanel).Items[LbLeftPanel.IsActive
																						? LbLeftPanel.MySelectedItem 
																						: LbRightPanel.MySelectedItem]));
					break;
				case Key.F6:
					if(LbLeftPanel.IsActive)
						FileWork.CopyElements(((FileElement[]) LbLeftPanel.ItemsSource).Where(elem => elem.IsSelected).ToArray(),
							new FileInfo(LbRightPanel.Path));
					else
						FileWork.CopyElements(((FileElement[]) LbRightPanel.ItemsSource).Where(elem => elem.IsSelected).ToArray(),
							new FileInfo(LbLeftPanel.Path));
					break;
				case Key.F7:
					if(LbLeftPanel.IsActive)
						FileWork.MoveElements(((FileElement[]) LbLeftPanel.ItemsSource).Where(elem => elem.IsSelected).ToArray(),
							new FileInfo(LbRightPanel.Path));
					else
						FileWork.MoveElements(((FileElement[]) LbRightPanel.ItemsSource).Where(elem => elem.IsSelected).ToArray(),
							new FileInfo(LbLeftPanel.Path));
					break;
				case Key.F8:
					FileWork.DeleteElements(
						((FileElement[]) (LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel).ItemsSource).Where(elem => elem.IsSelected)
																										.ToArray());
					break;
				case Key.F9:
					ShowSettings();
					break;
				case Key.Back:
					BackProcess(LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel);
					break;
				case Key.Tab:
					TabProcess(LbLeftPanel, LbRightPanel);
					break;
				case Key.Down:
					DownProcess(LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel);
					break;
				case Key.Up:
					UpProcess(LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel);
					break;
			}
		}

		private void wMain_KeyUp(object sender, KeyEventArgs e) {
			switch(e.Key) {
				case Key.Enter:
					EnterProcess(LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel);
					break;
				case Key.D1:
				case Key.NumPad1:
					if(Keyboard.IsKeyDown(Key.LeftCtrl)) {
						var aw = new AdditionalWindow {
														MyTitle = "Левое окно.",
														Text = "Выберите атрибуты файлов для левого окна.",
														Attributes = (int) LbLeftPanel.CurAttributes,
														Owner = this,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = false,
														ElemName = null,
														Rename = false
													};
						aw.ShowDialog();
						if(aw.Result == Results.Bad) return;
						SaveLeftWindowAttributes(aw);
					}
					if(Keyboard.IsKeyDown(Key.RightCtrl)) {
						var aw = new AdditionalWindow {
														MyTitle = "Правое окно.",
														Text = "Выберите атрибуты файлов для правого окна.",
														Attributes = (int) LbRightPanel.CurAttributes,
														Owner = this,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = false,
														ElemName = null,
														Rename = false
													};
						aw.ShowDialog();
						if(aw.Result == Results.Bad) return;
						SaveRightWindowAttributes(aw);
					}
					break;
				case Key.D2:
				case Key.NumPad2:
					if(Keyboard.IsKeyDown(Key.LeftCtrl)) {
						var aw = new AdditionalWindow {
														MyTitle = "Левое окно.",
														Text = "Выберите диск для левого окна.",
														Attributes = -1,
														Owner = this,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = true,
														ElemName = null,
														Rename = false
													};
						aw.ShowDialog();
						if(aw.Result == Results.Bad)
							return;
						if(aw.Drive == null)
							return;
						LbLeftPanel.Path = aw.Drive.FullName;
						LbLeftPanel.MySelectedItem = 0;
					}
					if(Keyboard.IsKeyDown(Key.RightCtrl)) {
						var aw = new AdditionalWindow {
														MyTitle = "Правое окно.",
														Text = "Выберите диск для правого окна.",
														Attributes = -1,
														Owner = this,
														VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
														DriveSelection = true,
														ElemName = null,
														Rename = false
													};
						aw.ShowDialog();
						if(aw.Result == Results.Bad)
							return;
						if(aw.Drive == null)
							return;
						LbRightPanel.Path = aw.Drive.FullName;
						LbRightPanel.MySelectedItem = 0;
					}
					break;
				case Key.D3:
				case Key.NumPad3:
					break;
				case Key.D4:
				case Key.NumPad4:
					break;
				case Key.D5:
				case Key.NumPad5:
					break;
			}
		}

		private void WMain_Activated(object sender, EventArgs e) {
			Effect = new BlurEffect {
										Radius = 0,
										KernelType = KernelType.Gaussian,
										RenderingBias = RenderingBias.Quality
									};
		}

		private void WMain_Deactivated(object sender, EventArgs e) {
			Effect = new BlurEffect {
										Radius = 10,
										KernelType = KernelType.Gaussian,
										RenderingBias = RenderingBias.Quality
									};
		}

		private static void SystemKeyDown(KeyEventArgs e) {
			switch(e.SystemKey) {
				case Key.F10:
					Application.Current.Shutdown();
					break;
			}
		}

		private void SaveRightWindowAttributes(AdditionalWindow aw) {
			Settings.Default.RightAttributes = FileAttributes.Archive|FileAttributes.Compressed|
												FileAttributes.Directory;
			Settings.Default.NegRigthAttributes = 0;
			foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == true)) {
				Settings.Default.RightAttributes |= (string) item.Content == "Hidden"
														? FileAttributes.Hidden
														: (string) item.Content == "Temporary"? FileAttributes.Temporary : FileAttributes.System;
			}

			foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == false)) {
				Settings.Default.NegRigthAttributes |= (string) item.Content == "Hidden"
															? FileAttributes.Hidden
															: (string) item.Content == "Temporary"? FileAttributes.Temporary : FileAttributes.System;
			}
			Settings.Default.Save();
		}

		private void SaveLeftWindowAttributes(AdditionalWindow aw) {
			Settings.Default.LeftAttributes = FileAttributes.Archive|FileAttributes.Compressed|
											FileAttributes.Directory;
			Settings.Default.NegLeftAttributes = 0;
			foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == true)) {
				Settings.Default.LeftAttributes |= (string) item.Content == "Hidden"
														? FileAttributes.Hidden
														: (string) item.Content == "Temporary"? FileAttributes.Temporary : FileAttributes.System;
			}

			foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == false)) {
				Settings.Default.NegLeftAttributes |= (string) item.Content == "Hidden"
														? FileAttributes.Hidden
														: (string) item.Content == "Temporary"? FileAttributes.Temporary : FileAttributes.System;
			}
			Settings.Default.Save();
		}

		private void EnterProcess(MyFolderView panel) {
			try {
				var fileSystemInfo = (FileElement) panel.Items[panel.MySelectedItem];
				if(fileSystemInfo == null) return;
				if(fileSystemInfo.IsFolder) {
					panel.Path = fileSystemInfo.FullName;
					panel.MySelectedItem = 0;
				}
				else
					Process.Start(fileSystemInfo.FullName);
			}
			catch(UnauthorizedAccessException) {
				var aw = new AdditionalWindow {
												MyTitle = "Ошибка!",
												Text = "Недостаточно прав доступа к элементу!",
												Owner = this,
												Attributes = -1,
												VisibleButtons = (int) Buttons.BtnOk,
												DriveSelection = false,
												ElemName = null,
												Rename = false
											};
				aw.ShowDialog();
			}
		}

		private void TabProcess(MyFolderView firstPanel, MyFolderView secondPanel) {
			foreach(ISelectable item in (firstPanel.IsActive? firstPanel : secondPanel).Items) item.UnSelect();
			secondPanel.IsActive = !secondPanel.IsActive;
			firstPanel.IsActive = !firstPanel.IsActive;
		}

		private void UpProcess(MyFolderView panel) {
			if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
				foreach(var item in panel.Items)
					((ISelectable) item).UnSelect();
			var id = (panel.MySelectedItem - 1 + panel.Items.Count)%panel.Items.Count;
			panel.MySelectedItem = id;
		}

		private void DownProcess(MyFolderView panel) {
			if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
				foreach(var item in panel.Items)
					((ISelectable) item).UnSelect();
			var id = (panel.MySelectedItem + 1)%panel.Items.Count;
			panel.MySelectedItem = id;
		}

		private void BackProcess(MyFolderView panel) {
			var parent = new DirectoryInfo(panel.Path);
			parent = parent.Parent;
			if(parent == null)
				return;
			panel.Path = parent.FullName;
			panel.MySelectedItem = 0;
		}

		private void ShowSettings() {
			var sw = new SettingsWindow {Owner = this};
			sw.ShowDialog();
		}
	}
}
