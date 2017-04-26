using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FreeMax_File_Manager {
	public class MyFolderView: ListBox {
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

		private static MyFileSystemWatcher _watcher;

		private FileAttributes _curAttributes;
		private FileAttributes _curNegativeAttributes;
		private bool _isActive;
		private int _mySelectedItem;

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
				if(Items.Count<1) return;
				_mySelectedItem = value;
				var selectable = Items[value] as ISelectable;
				selectable?.Select();
				UpdateView();
			}
		}

		private int MyLastSelectedItem{ get; set; }

		public bool IsActive {
			get { return _isActive; }
			set {
				_isActive = value;
				if(!(Items.Count > 0)) return;
				if(value) MySelectedItem = MyLastSelectedItem < 0? 0 : MyLastSelectedItem%Items.Count;
				else MyLastSelectedItem = MySelectedItem;
				UpdateView();
			}
		}

		private static object CoercePath(DependencyObject d, object value) {
			if(value == null)
				value = string.Empty;
			OnPathChanged(d, new DependencyPropertyChangedEventArgs(PathProperty, "", value));
			return value;
		}

		private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MyFolderView) d).ItemsSource = GetFolderContent((MyFolderView) d, (string) e.NewValue);
			_watcher?.Stop();
			_watcher = new MyFileSystemWatcher((string) e.NewValue) {SomeObject = d};
			_watcher.Changed += _watcher_Event;
			_watcher.Created += _watcher_Event;
			_watcher.Deleted += _watcher_Event;
			_watcher.Renamed += _watcher_Event;
			_watcher.Start();
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
				new DependencyPropertyChangedEventArgs(NegativeAttributeProperty, ((MyFolderView) d)._curNegativeAttributes, value));
			return value;
		}

		private static void OnNegativeAttributesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MyFolderView) d)._curNegativeAttributes = (FileAttributes) e.NewValue;
			if(((MyFolderView) d).Path.Length > 0)
				((MyFolderView) d).UpdateSource();
		}

		private static FileElement[] GetFolderContent(MyFolderView view, string path) {
			var dir = new DirectoryInfo(path);
			var elems = dir.GetFileSystemInfos()//TODO Не работает с внешними устройствами
							.Where(f => ((f.Attributes&view._curAttributes) != 0 && (f.Attributes&view._curNegativeAttributes) == 0))
							.OrderByDescending(f => (f.Attributes&FileAttributes.Directory) == FileAttributes.Directory)
							.ThenBy(f => f.Name)
							.ToArray();
			var files = new FileElement[elems.Length];
			for(var i = 0; i < elems.Length; i++) {
				files[i] = new FileElement(elems[i]);
			}
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
			ScrollIntoView(Items[MySelectedItem]);
		}

		public void UpdateSource() {
			ItemsSource = null;
			var tmp = Path.Trim();
			Path = tmp;
			if (Items.Count == 0) return;
			ScrollIntoView(Items[MySelectedItem]);
			var selectable = Items[MySelectedItem] as ISelectable;
			selectable?.Select();
			UpdateView();
		}
	}
}
