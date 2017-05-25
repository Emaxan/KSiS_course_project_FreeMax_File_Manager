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

        public MainWindow() { InitializeComponent(); }
         
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            var authenticated = false;
            while(!authenticated) {
                var aw = new AdditionalWindow {
                                                               MyTitle = "Приветствую тебя, Пользователь.",
                                                               Text = "Введи IP и авторизуйся.",
                                                               Owner = this,
                                                               VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
                                                               Authentication = true
                                                           };
                aw.ShowDialog();
                if(aw.Result == Results.Bad) {
                    Application.Current.Shutdown();
                    return;
                }
                var pas = aw.TbPassword.Password;
                var name = aw.TbName.Text;
                var ip = aw.TbIp.Text;
                var dom = aw.TbDomain.Text;
                var result =
                    Task.Run(async () => await ConnectAsync($"http://{ip}:13666/signalr"))
                        .GetAwaiter()
                        .GetResult();
                if(!result) {
                    MessageBox.Show(this, "Can't connect to the server");
                    continue;
                }
                authenticated =
                    Task.Run(
                        async () =>
                        await HubProxy.Invoke<bool>("Authentication", name, dom, pas))
                        .GetAwaiter()
                        .GetResult();
                if(authenticated) continue;
                Connection.Stop();
                aw = new AdditionalWindow {
                                              MyTitle = "WrongData",
                                              Text = "Cannot authenticate.",
                                              Owner = this,
                                              VisibleButtons = (int) Buttons.BtnOk
                                          };
                aw.ShowDialog();
            }
            FileWork.MainWindow = this;
            FileWork.Proxy = HubProxy;
            LbLeftPanel.Position = "left";
            LbRightPanel.Position = "right";
            LbLeftPanel.Proxy = HubProxy;
            LbRightPanel.Proxy = HubProxy;
            LbLeftPanel.IsActive = true;
            LbLeftPanel.UpdateSource();
            LbRightPanel.UpdateSource();
            (LbLeftPanel.Items[0] as ISelectable)?.Select();
            LbLeftPanel.Focus();
            (LbRightPanel.Items[0] as ISelectable)?.UnSelect();
            HubProxy.Invoke("SetFileWatcher", LbLeftPanel.Path, "left", LbLeftPanel.CurAttributes, LbLeftPanel.CurNegativeAttributes);
            HubProxy.Invoke("SetFileWatcher", LbRightPanel.Path, "right", LbRightPanel.CurAttributes, LbRightPanel.CurNegativeAttributes);
            //Task.Run(async () => await HubProxy.Invoke<string>("DownloadFile")).GetAwaiter().GetResult();//TODO sending files
        }

        public void Fnc(int i) { MessageBox.Show(i.ToString()); }

        private void wMain_KeyDown(object sender, KeyEventArgs e) {
            bool b = false;
            switch(e.Key) {
                case Key.System:
                    SystemKeyDown(e);
                    b = true;
                    break;
                case Key.F3:
                    FileWork.CreateFolder(new StringElement(LbLeftPanel.IsActive? LbLeftPanel.Path : LbRightPanel.Path));
                    b = true;
                    break;
                case Key.F4:
                    FileWork.CreateFile(new StringElement(LbLeftPanel.IsActive? LbLeftPanel.Path : LbRightPanel.Path));
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
            if(!b) return;
            LbLeftPanel.UpdateSource();
            LbRightPanel.UpdateSource();
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
                                                          VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel
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
                                                          Proxy = HubProxy
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
                                                          Owner = this,
                                                          VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
                                                          Proxy = HubProxy,
                                                          DriveSelection = true
                                                      };
                        aw.ShowDialog();
                        if(aw.Result == Results.Bad)
                            return;
                        if(aw.Drive == null)
                            return;
                        LbLeftPanel.Path = aw.Drive.FullPath;
                        LbLeftPanel.MySelectedItem = 0;
                    }
                    if(Keyboard.IsKeyDown(Key.RightCtrl)) {
                        var aw = new AdditionalWindow {
                                                          MyTitle = "Правое окно.",
                                                          Text = "Выберите диск для правого окна.",
                                                          Owner = this,
                                                          VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
                                                          Proxy = HubProxy,
                                                          DriveSelection = true
                                                      };
                        aw.ShowDialog();
                        if(aw.Result == Results.Bad)
                            return;
                        if(aw.Drive == null)
                            return;
                        LbRightPanel.Path = aw.Drive.FullPath;
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
                var element = (StringElement) panel.Items[panel.MySelectedItem];
                if(element == null) return;
                if(element.IsDir) {
                    panel.Path = element.FullPath;
                    panel.MySelectedItem = 0;
                }
                else
                    Process.Start(element.FullPath);//TODO sending files
            }
            catch(UnauthorizedAccessException) {
                var aw = new AdditionalWindow {
                                                  MyTitle = "Ошибка!",
                                                  Text = "Недостаточно прав доступа к элементу!",
                                                  Owner = this,
                                                  VisibleButtons = (int) Buttons.BtnOk
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
            Connection.Closed += () => {
                                     Connection.TraceWriter.Close();
                                 };
            //Connection.Closed += () => MessageBox.Show("Server has closed connection.");
            Connection.Error += ex => {
                                    MessageBox.Show($"SignalR error: {ex.Message}");
                                    new StreamWriter($"{Path.GetTempPath()}FreeMaxManagerError.log").Write(
                                        $"SignalR error: {ex.Message}");
                                };
            Connection.TraceLevel = TraceLevels.All;
            var writer = new StreamWriter($"{Path.GetTempPath()}FreeMaxManager.log", true) {AutoFlush = true};
            Connection.TraceWriter = writer;
            ServicePointManager.DefaultConnectionLimit = 12;
            HubProxy = Connection.CreateHubProxy("MyHub");
            
            HubProxy.On<string>("Message", message => Dispatcher.Invoke(() => MessageBox.Show($"{message}\r")));
            HubProxy.On<string, StringElement[]>("UpdateSourceForPanel",
                (panel, elems) => {
                    Dispatcher.Invoke(() => (panel == "left"
                                                 ? LbLeftPanel
                                                 : LbRightPanel).UpdateSource(elems));
                });

            try
            {
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
