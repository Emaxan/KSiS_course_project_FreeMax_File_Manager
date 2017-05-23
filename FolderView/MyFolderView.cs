using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;

namespace FolderView {
    [DefaultProperty("MySelectedItem")]
    [Localizability(LocalizationCategory.ListBox)]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ListBoxItem))]
    public class MyFolderView: ListBox {

        /*__________________________________DEPENDENCY_PROPERTIES__________________________________________________________________*/
        #region Dependency properties
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path",
                typeof(string),
                typeof(MyFolderView),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.AffectsMeasure|FrameworkPropertyMetadataOptions.AffectsRender,
                    OnPathChanged,
                    CoercePath));

        public static readonly DependencyProperty AttributesProperty =
            DependencyProperty.Register("CurAttributes",
                typeof(FileAttributes),
                typeof(MyFolderView),
                new FrameworkPropertyMetadata(FileAttributes.Directory|FileAttributes.Archive|FileAttributes.Compressed,
                    FrameworkPropertyMetadataOptions.AffectsMeasure|FrameworkPropertyMetadataOptions.AffectsRender,
                    OnAttributesChanged,
                    CoerceAttributes));

        public static readonly DependencyProperty NegativeAttributeProperty =
            DependencyProperty.Register("CurNegativeAttributes",
                typeof(FileAttributes),
                typeof(MyFolderView),
                new FrameworkPropertyMetadata(FileAttributes.Temporary|FileAttributes.System|FileAttributes.Hidden,
                    FrameworkPropertyMetadataOptions.AffectsMeasure|FrameworkPropertyMetadataOptions.AffectsRender,
                    OnNegativeAttributesChanged,
                    CoerceNegativeAttributes));
        #endregion
        /*_______________________________________FIELDS_______________________________________________________________________________*/
        #region Fields
        private static MyFileSystemWatcher _watcher;
        private FileAttributes _curAttributes;
        private FileAttributes _curNegativeAttributes;
        private bool _isActive, _notUpdateView = false;
        private int _mySelectedItem;
        public IHubProxy Proxy;
        #endregion
        /*________________________________________________PROPERTIES______________________________________________________________________*/
        #region Properties
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path {
            get { return (string) GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileAttributes CurAttributes {
            get { return (FileAttributes) GetValue(AttributesProperty); }
            set {
                _curAttributes = value;
                SetValue(AttributesProperty, value);
            }
        }

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileAttributes CurNegativeAttributes {
            get { return (FileAttributes) GetValue(NegativeAttributeProperty); }
            set {
                _curNegativeAttributes = value;
                SetValue(NegativeAttributeProperty, value);
            }
        }

        public int MySelectedItem {
            get { return _mySelectedItem; }
            set {
                if(Items.Count < 1) return;
                _mySelectedItem = value;
                var selectable = Items[value] as ISelectable;
                selectable?.Select();
                if(!_notUpdateView) UpdateView();
            }
        }

        private int MyLastSelectedItem{ get; set; }

        public bool IsActive {
            get { return _isActive; }
            set {
                _isActive = value;
                if(!(Items.Count > 0)) return;
                if(value) {
                    MySelectedItem = MyLastSelectedItem < 0? 0 : MyLastSelectedItem%Items.Count;
                    if(MyLastSelectedItems != null)
                        foreach(var item in MyLastSelectedItems) {
                            item.Select();
                        }
                }
                else {
                    MyLastSelectedItem = MySelectedItem;
                    MyLastSelectedItems = (Items.OfType<ISelectable>()).Where(item => item.IsSelected).ToArray();
                    if(MyLastSelectedItems != null)
                        foreach(var item in MyLastSelectedItems) {
                            item.UnSelect();
                        }
                }
                UpdateView();
            }
        }

        public ISelectable[] MySelectedItems {
            get {
                return (Items.OfType<ISelectable>()).Where(item => item.IsSelected).ToArray();
            }
        }

        private ISelectable[] MyLastSelectedItems{ get; set; }
        #endregion
        /*_____________________________________METHODS___________________________________________________________________*/
        #region Methods
        private static object CoercePath(DependencyObject d, object value) {
            if(value == null)
                value = string.Empty;
            OnPathChanged(d, new DependencyPropertyChangedEventArgs(PathProperty, "", value));
            return value;
        }

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var files = GetFolderContent((MyFolderView)d, (string)e.NewValue, ((MyFolderView)d).Proxy);
            //if(files == null) return;
            ((MyFolderView) d).ItemsSource = files;
            /*_watcher?.Stop();//TODO Make FileWatcher
            _watcher = new MyFileSystemWatcher((string) e.NewValue) {SomeObject = d};
            _watcher.Changed += _watcher_Event;
            _watcher.Created += _watcher_Event;
            _watcher.Deleted += _watcher_Event;
            _watcher.Renamed += _watcher_Event;
            _watcher.Start();*/
        }

        private static object CoerceAttributes(DependencyObject d, object value) {
            if(value == null)
                value = 0;
            OnAttributesChanged(d,
                new DependencyPropertyChangedEventArgs(AttributesProperty, ((MyFolderView) d)._curAttributes, value));
            return value;
        }

        private static void OnAttributesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((MyFolderView) d)._curAttributes = (FileAttributes) e.NewValue;
            if(((MyFolderView) d).Path.Length > 0)
                ((MyFolderView) d).UpdateSource();
        }

        private static object CoerceNegativeAttributes(DependencyObject d, object value) {
            if(value == null)
                value = 0;
            OnNegativeAttributesChanged(d,
                new DependencyPropertyChangedEventArgs(NegativeAttributeProperty,
                    ((MyFolderView) d)._curNegativeAttributes,
                    value));
            return value;
        }

        private static void OnNegativeAttributesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((MyFolderView) d)._curNegativeAttributes = (FileAttributes) e.NewValue;
            if(((MyFolderView) d).Path.Length > 0)
                ((MyFolderView) d).UpdateSource();
        }

        private static IEnumerable<StringElement> GetFolderContent(MyFolderView view, string path, IHubProxy proxy) {
            if(proxy == null) return null;
            var curA = (int) view.CurAttributes;
            var curNa = (int) view.CurNegativeAttributes;
            var pathn = view.Path;
            var combine = Task.Run(()=>proxy.Invoke<string>("GetFolderContent", curA, curNa, pathn)).GetAwaiter().GetResult();
            var files = combine?.Split('|').Select(f => new StringElement(f.Split('*')));
            return files;
        }

        private static void _watcher_Event(object sender, FileSystemEventArgs e) {
            Application.Current.Dispatcher.Invoke(
                () => ((sender as MyFileSystemWatcher)?.SomeObject as MyFolderView)?.UpdateSource());
        }

        public void UpdateView() {
            var temp = ItemsSource;
            ItemsSource = null;
            ItemsSource = temp;
            if(Items.Count == 0) return;
            if(MySelectedItem >= Items.Count) {
                _notUpdateView = true;
                MySelectedItem %= Items.Count;
                _notUpdateView = false;
            }
            ScrollIntoView(Items[MySelectedItem]);
        }

        public void UpdateSource() {
            ItemsSource = null;
            var tmp = Path.Trim();
            Path = tmp;
            if(Items.Count == 0) return;
            if(MySelectedItem >= Items.Count) {
                MySelectedItem %= Items.Count;
            }
            ScrollIntoView(Items[MySelectedItem]);
            if(IsActive)
                (Items[MySelectedItem] as ISelectable)?.Select();
            UpdateView();
        }
        #endregion
    }
}
