using System.IO;
using System.Threading.Tasks;
using System.Windows;
using FreeMax_File_Manager.Windows;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;

namespace FreeMax_File_Manager { //TODO ПРОТЕСТИТЬ ВСЕ ФУНКЦИИ И СМЕНУ АТРИБУТОВ

    public static class FileWork {
        public static Window MainWindow;
        public static IHubProxy Proxy;

        public static void Rename(StringElement fileElement) {//TODO Нельзя перемещать между томами
            var name = fileElement.Name;
            if(name.StartsWith("(hidden)")) name = name.Substring(8, name.Length - 8);
            if(fileElement.IsDir) name = name.Substring(2, name.Length - 4);
            var aw = new AdditionalWindow {
                                              MyTitle = "Переименование.",
                                              Text = $"Введите новое имя для {name}.",
                                              Attributes = -1,
                                              Owner = MainWindow,
                                              VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
                                              DriveSelection = false,
                                              ElemName = null,
                                              Rename = true
                                          };

            aw.ShowDialog();

            if(aw.Result == Results.Bad) return;

            if(fileElement.IsDir) {
                var directory =
                    Task.Run(async () => await Proxy.Invoke<string>("GetParent", fileElement.FullPath))
                        .GetAwaiter()
                        .GetResult();
                if(directory == "null") return;

                if(
                    Task.Run(async () => await Proxy.Invoke<bool>("IsDirectoryExist", directory + '\\' + aw.NewName))
                        .GetAwaiter()
                        .GetResult()) {
                    var aw1 = new AdditionalWindow {
                                                       MyTitle = "Подтвердите действие.",
                                                       Text =
                                                           $"Папка {aw.NewName} существует. Хотите выполнить слияние?",
                                                       Attributes = -1,
                                                       Owner = MainWindow,
                                                       VisibleButtons = (int) Buttons.BtnOk|(int) Buttons.BtnCancel,
                                                       DriveSelection = false,
                                                       ElemName = null,
                                                       Rename = false
                                                   };
                    aw1.ShowDialog();
                    if(aw1.Result == Results.Bad) return;
                }
                Task.Run(
                    async () =>
                    await Proxy.Invoke("MoveDirectory", fileElement.FullPath, directory + '\\' + aw.NewName))
                    .GetAwaiter()
                    .GetResult();
            }
            else {
                var fileInfo = Task.Run(async () => await Proxy.Invoke<string>("GetDirectoryOfFile", fileElement.FullPath))
                    .GetAwaiter()
                    .GetResult();
                if(fileInfo == "null") return;
                if(Task.Run(async () => await Proxy.Invoke<bool>("IsFileExist", fileInfo + '\\' + aw.NewName))
                       .GetAwaiter()
                       .GetResult()) {
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
                    Task.Run(async () => await Proxy.Invoke("DeleteFile", fileInfo + '\\' + aw.NewName)).GetAwaiter().GetResult();
                }
                Task.Run(async () => await Proxy.Invoke("MoveFile", fileElement.FullPath, fileInfo + '\\' + aw.NewName)).GetAwaiter().GetResult();
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

            if(
                Task.Run(async () => await Proxy.Invoke<bool>("IsFileExist", aw.NewElem.FullName))
                    .GetAwaiter()
                    .GetResult()) {
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
                if(aw1.Result == Results.Bad) return;
                Task.Run(async () => await Proxy.Invoke("DeleteFile", aw.NewElem.FullName)).GetAwaiter().GetResult();
            }

            Task.Run(async () => await Proxy.Invoke("CreateFile", aw.NewElem.FullName)).GetAwaiter().GetResult();
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

            if(
                Task.Run(async () => await Proxy.Invoke<bool>("IsDirectoryExist", aw.Drive.FullName))
                    .GetAwaiter()
                    .GetResult()) {
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
                Task.Run(async () => await Proxy.Invoke("DeleteDirectiry", aw.Drive.FullName)).GetAwaiter().GetResult();
            }

            Task.Run(async () => await Proxy.Invoke("CreateDirectory", aw.Drive.FullName)).GetAwaiter().GetResult();
        }

        public static void DeleteElements(StringElement[] items) {
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

        private static void Delete(StringElement[] items) {
            foreach(var item in items) {
                Task.Run(async () => await Proxy.Invoke(item.IsDir?"DeleteDirectory": "DeleteFile", item.FullPath)).GetAwaiter().GetResult();
            }
        }

        public static void CopyElements(StringElement[] items, StringElement destination) {
            if(items.Length == 0) return;

            var aw = new AdditionalWindow {
                                              Owner = MainWindow,
                                              Text =
                                                  $"Вы уверены, что хотите скопировать {items.Length} элемент(-а,-ов)?",
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

        private static void Copy(StringElement item, StringElement destination) {
            var name = item.Name;
            if(item.Name.StartsWith("(hidden)")) name = item.Name.Substring(8, item.Name.Length - 8);
            if(item.IsDir) {
                var dir = item.FullPath;
                name = item.Name.Substring(2, item.Name.Length - 4);

                var dest = destination.FullPath + '\\' + name;
                if(
                    Task.Run(
                        async () =>
                        await Proxy.Invoke<bool>("IsDirectoryExist", dest))
                        .GetAwaiter()
                        .GetResult()) {
                    var aw = new AdditionalWindow {
                                                      MyTitle = "Подтвердите операцию.",
                                                      Text = $"Папка {dest} существует. Хотите выполнить слияние?",
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
                    Task.Run(async () => await Proxy.Invoke("CreateDirectory", dest))
                        .GetAwaiter()
                        .GetResult();
                Task.Run(async () => await Proxy.Invoke("CopyDirectory", dir, dest))
                    .GetAwaiter()
                    .GetResult();
            }
            else {
                if(Task.Run(
                    async () =>
                    await Proxy.Invoke<bool>("IsFileExist", destination.FullPath + '\\' + name))
                       .GetAwaiter()
                       .GetResult()) {
                    var aw = new AdditionalWindow {
                                                      MyTitle = "Подтвердите операцию.",
                                                      Text = $"Файл {name} существует. Хотите заместить его?",
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
                Task.Run(async () => await Proxy.Invoke("CopyFile", item.FullPath, destination.FullPath + '\\' + name))
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public static void MoveElements(StringElement[] items, StringElement destination) {
            //TODO Нельзя перемещать между томами
            if(items.Length == 0) return;

            var aw = new AdditionalWindow {
                                              Owner = MainWindow,
                                              Text =
                                                  $"Вы уверены, что хотите переместить {items.Length} элемент(-а,-ов)?",
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
                var name = item.Name;
                if(name.StartsWith("(hidden)")) name = name.Substring(8, name.Length - 8);
                if(item.IsDir) {
                    name = name.Substring(2, name.Length - 4);
                    if(
                        Task.Run(
                            async () => await Proxy.Invoke<bool>("IsDirectoryExist", destination.FullPath + '\\' + name))
                            .GetAwaiter()
                            .GetResult()) {
                        aw = new AdditionalWindow {
                                                      MyTitle = "Подтвердите операцию.",
                                                      Text = $"Папка {name} существует. Хотите выполнить слияние?",
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
                    Task.Run(
                        async () =>
                        await Proxy.Invoke("MoveDirectory", item.FullPath, destination.FullPath + '\\' + name))
                        .GetAwaiter()
                        .GetResult();
                }
                else {
                    if(Task.Run(async () => await Proxy.Invoke<bool>("IsFileExist", destination.FullPath + '\\' + name))
                           .GetAwaiter()
                           .GetResult()) {
                        aw = new AdditionalWindow {
                                                      MyTitle = "Подтвердите операцию.",
                                                      Text = $"Файл {name} существует. Хотите заменить его?",
                                                      VisibleButtons = (int) Buttons.BtnYes|(int) Buttons.BtnNo,
                                                      Owner = MainWindow,
                                                      Attributes = -1,
                                                      DriveSelection = false,
                                                      ElemName = null,
                                                      Rename = false
                                                  };
                        aw.ShowDialog();
                        if(aw.Result == Results.Bad) return;
                        Task.Run(async () => await Proxy.Invoke("DeleteFile", destination.FullPath + '\\' + name))
                            .GetAwaiter()
                            .GetResult();
                    }
                    Task.Run(
                        async () => await Proxy.Invoke("MoveFile", item.FullPath, destination.FullPath + '\\' + name))
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }
    }
}
