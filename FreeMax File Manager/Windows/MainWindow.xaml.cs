using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Threading.Tasks;
using FolderView;
using FreeMax_File_Manager.Properties;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;

namespace FreeMax_File_Manager.Windows {
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        #region Form

        public MainWindow() {
            InitializeComponent();
            var result = Task.Run(async ()=> await ConnectAsync($"http://127.0.0.1:13666/signalr")).GetAwaiter().GetResult();//TODO Change it
            if(!result) {
                MessageBox.Show(this, "Can't connect to the server");
                Application.Current.Shutdown();
            }
            var authenticated = Task.Run(async ()=> await HubProxy.Invoke<bool>("Authentication", "emaxan1997@gmail.com", "MAX-WIN10-X64", "r8004846")).GetAwaiter().GetResult();
            if(!authenticated) MessageBox.Show(this, "Wrong Data!");

            FileWork.MainWindow = this;
            FileWork.Proxy = HubProxy;
            LbLeftPanel.Proxy = HubProxy;
            LbRightPanel.Proxy = HubProxy;
            LbLeftPanel.IsActive = true;
            LbLeftPanel.UpdateSource();
            LbRightPanel.UpdateSource();
            (LbLeftPanel.Items[0] as ISelectable)?.Select();
            LbLeftPanel.Focus();
            (LbRightPanel.Items[0] as ISelectable)?.UnSelect();
        }

        private void wMain_KeyDown(object sender, KeyEventArgs e) {
            bool b = false;
            switch(e.Key) {
                case Key.System:
                    SystemKeyDown(e);
                    b = true;
                    break;
                case Key.F3:
                    FileWork.CreateFolder(LbLeftPanel.IsActive
                                              ? new FileInfo(LbLeftPanel.Path)
                                              : new FileInfo(LbRightPanel.Path));
                    b = true;
                    break;
                case Key.F4:
                    FileWork.CreateFile(LbLeftPanel.IsActive
                                            ? new FileInfo(LbLeftPanel.Path)
                                            : new FileInfo(LbRightPanel.Path));
                    b = true;
                    break;
                case Key.F5:
                    FileWork.Rename(
                        (StringElement) ((LbLeftPanel.IsActive
                                            ? LbLeftPanel
                                            : LbRightPanel).Items[LbLeftPanel.IsActive
                                                                      ? LbLeftPanel.MySelectedItem
                                                                      : LbRightPanel.MySelectedItem]));
                    b = true;
                    break;
                case Key.F6:
                    FileWork.CopyElements(
                        (LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel).Items.OfType<StringElement>()
                                                                          .Where(elem => elem.IsSelected)
                                                                          .ToArray(),
                        new StringElement(LbLeftPanel.IsActive? LbRightPanel.Path : LbLeftPanel.Path));
                    b = true;
                    break;
                case Key.F7:
                    FileWork.MoveElements(
                        (LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel).Items.OfType<StringElement>()
                                                                          .Where(elem => elem.IsSelected)
                                                                          .ToArray(),
                        new StringElement(LbLeftPanel.IsActive ? LbRightPanel.Path : LbLeftPanel.Path));
                    b = true;
                    break;
                case Key.F8:
                    FileWork.DeleteElements(
                        (LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel).Items.OfType<StringElement>()
                                                                          .Where(elem => elem.IsSelected)
                                                                          .ToArray());
                    b = true;
                    break;
                case Key.F9:
                    ShowSettings();
                    break;
                case Key.Back:
                    BackProcess(LbLeftPanel.IsActive? LbLeftPanel : LbRightPanel);
                    b = true;
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
            if(b) {
                LbLeftPanel.UpdateSource();
                LbRightPanel.UpdateSource();
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
                                                        : (string) item.Content == "Temporary"
                                                              ? FileAttributes.Temporary
                                                              : FileAttributes.System;
            }

            foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == false)) {
                Settings.Default.NegRigthAttributes |= (string) item.Content == "Hidden"
                                                           ? FileAttributes.Hidden
                                                           : (string) item.Content == "Temporary"
                                                                 ? FileAttributes.Temporary
                                                                 : FileAttributes.System;
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
                                                       : (string) item.Content == "Temporary"
                                                             ? FileAttributes.Temporary
                                                             : FileAttributes.System;
            }

            foreach(var item in aw.LbAttributes.Items.Cast<CheckBox>().Where(item => item.IsChecked == false)) {
                Settings.Default.NegLeftAttributes |= (string) item.Content == "Hidden"
                                                          ? FileAttributes.Hidden
                                                          : (string) item.Content == "Temporary"
                                                                ? FileAttributes.Temporary
                                                                : FileAttributes.System;
            }
            Settings.Default.Save();
        }

        private void EnterProcess(MyFolderView panel) {
            try {
                var fileSystemInfo = (StringElement) panel.Items[panel.MySelectedItem];
                if(fileSystemInfo == null) return;
                if(fileSystemInfo.IsDir) {
                    panel.Path = fileSystemInfo.FullPath;
                    panel.MySelectedItem = 0;
                }
                else
                    Process.Start(fileSystemInfo.FullPath);//TODO 
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
            var path = panel.Path;
            var parent = Task.Run(async ()=> await HubProxy.Invoke<string>("GetParent", path)).GetAwaiter().GetResult();
            if(parent == "null")
                return;
            panel.Path = parent;
            panel.MySelectedItem = 0;
        }

        private void ShowSettings() {
            var sw = new SettingsWindow {Owner = this};
            sw.ShowDialog();
        }

        #endregion

        #region Connection

        public IHubProxy HubProxy{ get; set; }
        public HubConnection Connection{ get; set; }

        //public IObservable<IList<JToken>> PathChanged; 

        private async Task<bool> ConnectAsync(string serverUri) //URI is : $"http://{TbConnectionIp.Text}:13666/signalr"
        {
            Connection = new HubConnection(serverUri);
            //Connection.Closed += () => MessageBox.Show("Server has closed connection.");
            Connection.Error += ex => {
                                    MessageBox.Show($"SignalR error: {ex.Message}");
                                    new StreamWriter($"{Path.GetTempPath()}FreeMaxManager.log").Write(
                                        $"SignalR error: {ex.Message}");
                                };
            Connection.TraceLevel = TraceLevels.All;
            var writer = new StreamWriter($"{Path.GetTempPath()}FreeMaxManager.log", true) {AutoFlush = true};
            Connection.TraceWriter = writer;
            ServicePointManager.DefaultConnectionLimit = 12;
            HubProxy = Connection.CreateHubProxy("MyHub");
            
            HubProxy.On<string>("AddMessage", message => Dispatcher.Invoke(() => MessageBox.Show($"{message}\r")));

            try {
                await Connection.Start();
            }
            catch(HttpRequestException) {
                MessageBox.Show("Unable to connect to server.");
                return false;
            }
       
            //PathChanged = HubProxy.Observe("PathChanged");//TODO Read about it
            return true;
        }

        #endregion
    }
}
