using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;

namespace FreeMax_File_Manager.Windows {
	[Flags]
	public enum Buttons {
		BtnOk = 1,
		BtnCancel = 2,
		BtnYes = 4,
		BtnNo = 8
	}

	public enum Results {
		Ok = 0,
		Bad = 1
	}

	/// <summary>
	///     Логика взаимодействия для AdditionalWindow.xaml
	/// </summary>
	public partial class AdditionalWindow {
		private bool _attrActive, _driveSelection, _driveActive, _creationFile, _creationFolder, _rename, _authentication;
		private int _attributes, _visibleButtons;

		private StringElement[] _dr;
		private StringElement _elemName;
		private RadioButton[] _rb;
		private string _title = string.Empty, _text = string.Empty;
		public StringElement Drive;
		public StringElement NewElem;
		public string NewName;
	    public IHubProxy Proxy;

		public Results Result = Results.Bad;

		public AdditionalWindow() { InitializeComponent(); }

	    public bool Authentication {
	        get { return _authentication; }
	        set {
	            _authentication = value;
	            GAuthentication.Visibility = value? Visibility.Visible : Visibility.Collapsed;
	        }
	    }

	    public bool Rename {
			get { return _rename; }
			set {
				_rename = value;
				if(value) {
					TbRename.Visibility = Visibility.Visible;
                    RealFileNameLabel.Visibility = Visibility.Visible;
					TbRename.Focus();
				}
				else
					TbRename.Visibility = Visibility.Collapsed;
                    
			}
		}

		public bool DriveSelection {
			get { return _driveSelection; }
			set {
				_driveSelection = value;
				if(value) {
					_driveActive = true;
					GDrives.Visibility = Visibility.Visible;
				    var combine = Task.Run(async () => await Proxy.Invoke<string>("GetReadyDrives")).GetAwaiter().GetResult();
					_dr = combine.Split('|').Select(file => new StringElement(file.Split('*'))).ToArray();
                    _rb = new RadioButton[_dr.Length];
					GDrives.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
					var j = 0;
					for(var i = 0; i < _rb.Length; i++){
						GDrives.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
						_rb[j] = new RadioButton {
													Content = $"{_dr[i].Name} ({_dr[i].FullPath})",
													Style = (Style) FindResource("Drives"),
													Name = $"Drive{i}"
												};
						GDrives.Children.Add(_rb[j]);
						Grid.SetRow(_rb[j], j);
						j++;
					}
					_rb[0].IsChecked = true;
					Drive = new StringElement(_dr[0].FullPath);
				}
				else {
					_driveActive = false;
					GDrives.Visibility = Visibility.Collapsed;
				}
			}
		}

		public StringElement ElemName {
			get { return _elemName; }
			set {
				_elemName = value;
				if(value != null) {
					_creationFile = true;
					_creationFolder = true;
					TbNewElem.Visibility = Visibility.Visible;
                    RealFileNameLabel.Visibility = Visibility.Visible;
					TbNewElem.Focus();
				}
				else {
					_creationFile = false;
					_creationFolder = false;
					TbNewElem.Visibility = Visibility.Collapsed;
				}
			}
		}

		public int Attributes {
			get { return _attributes; }
			set {
				_attributes = value;
				if(value == -1) {
                    _attrActive = false;
                    LbAttributes.Visibility = Visibility.Collapsed;
					return;
				}
                _attrActive = true;
                LbAttributes.Visibility = Visibility.Visible;
				((CheckBox) LbAttributes.Items[0]).IsChecked = (value&(int) FileAttributes.System) == (int) FileAttributes.System;
				((CheckBox) LbAttributes.Items[1]).IsChecked = (value&(int) FileAttributes.Temporary) ==
																(int) FileAttributes.Temporary;
				((CheckBox) LbAttributes.Items[2]).IsChecked = (value&(int) FileAttributes.Hidden) == (int) FileAttributes.Hidden;
				((CheckBox) LbAttributes.Items[0]).Focus();
				LbAttributes.SelectedIndex = 0;
			}
		}

		public int VisibleButtons {
			get { return _visibleButtons; }
			set {
				_visibleButtons = value;
				BYes.Visibility = (value&(int) Buttons.BtnYes) == (int) Buttons.BtnYes? Visibility.Visible : Visibility.Hidden;
				BCancel.Visibility = (value&(int) Buttons.BtnCancel) == (int) Buttons.BtnCancel
										? Visibility.Visible
										: Visibility.Hidden;
				BNo.Visibility = (value&(int) Buttons.BtnNo) == (int) Buttons.BtnNo? Visibility.Visible : Visibility.Hidden;
				BOk.Visibility = (value&(int) Buttons.BtnOk) == (int) Buttons.BtnOk? Visibility.Visible : Visibility.Hidden;
			}
		}

		public string MyTitle {
			get { return _title; }
			set {
				_title = value;
				Properties.UserSettings.Text.AdditionalWindow.Text.Default.Title = value;
				Properties.UserSettings.Text.AdditionalWindow.Text.Default.Save();
			}
		}

		public string Text {
			get { return _text; }
			set {
				_text = value;
				Properties.UserSettings.Text.AdditionalWindow.Text.Default.Content = value;
				Properties.UserSettings.Text.AdditionalWindow.Text.Default.Save();
			}
		}

	    public int Progress {
	        set { Text = value.ToString(); }
	    }

	    private void OnKeyUp(object sender, KeyEventArgs e) {
			switch(e.Key) {
				case Key.Enter:
					Result = Results.Ok;
					Close();
					break;
				case Key.Escape:
					Result = Results.Bad;
					Close();
					break;
				case Key.Y:
					if(((VisibleButtons&(int) Buttons.BtnYes) == (int) Buttons.BtnYes) &&
						(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
						Result = Results.Ok;
						Close();
					}
					break;
				case Key.N:
					if((VisibleButtons&(int) Buttons.BtnNo) == (int) Buttons.BtnNo &&
						(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
						Result = Results.Bad;
						Close();
					}
					break;
				case Key.C:
					if((VisibleButtons&(int) Buttons.BtnCancel) == (int) Buttons.BtnCancel &&
						(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
						Result = Results.Bad;
						Close();
					}
					break;
				case Key.O:
					if((VisibleButtons&(int) Buttons.BtnOk) == (int) Buttons.BtnOk &&
						(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
						Result = Results.Ok;
						Close();
					}
					break;
				case Key.Space:
					if(!_attrActive) return;
					((CheckBox) LbAttributes.Items[LbAttributes.SelectedIndex]).IsChecked =
						!((CheckBox) LbAttributes.Items[LbAttributes.SelectedIndex]).IsChecked;
					break;
				case Key.Down:
					if(_driveActive) {
						var number = 0;
						foreach(var radioButton in _rb)
							if(radioButton.IsChecked != true) number++;
							else {
								radioButton.IsChecked = false;
								break;
							}
						_rb[(number + 1)%GDrives.Children.Count].IsChecked = true;
						Drive = new StringElement(_dr[(number + 1)%GDrives.Children.Count].FullPath);
					}
					break;
				case Key.Up:
					if(_driveActive) {
						var number = 0;
						foreach(var radioButton in _rb)
							if(radioButton.IsChecked != true) number++;
							else {
								radioButton.IsChecked = false;
								break;
							}
						_rb[(number - 1 + GDrives.Children.Count)%GDrives.Children.Count].IsChecked = true;
						Drive = new StringElement(_dr[(number - 1 + GDrives.Children.Count)%GDrives.Children.Count].FullPath);
					}
					break;
			}
		}

		private void AdditionalWindow_OnClosed(object sender, EventArgs e) {
			if(_creationFile) NewElem = new StringElement(ElemName.FullPath + '\\' + NewName);
			if(_creationFolder) Drive = new StringElement(ElemName.FullPath + '\\' + NewName);
		}

		private void TbNewElem_OnKeyUp(object sender, KeyEventArgs e) {
			NewName = ((TextBox)sender).Text.Where(
				c =>
				(c != '*') && (c != '|') && (c != '\\') && (c != ':') && (c != '"') && (c != '<') && (c != '>') && (c != '?') && (c != '/')).Aggregate("",(str, c) => str + c);
			RealFileName.Content = NewName;
		}
	}
}
