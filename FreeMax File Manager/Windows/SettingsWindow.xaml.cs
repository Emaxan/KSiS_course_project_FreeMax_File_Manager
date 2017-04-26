using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TextSettings = FreeMax_File_Manager.Properties.UserSettings.Text.SettingsWindow.ColorSettings;
using MWFontSettings = FreeMax_File_Manager.Properties.UserSettings.Text.MainWindow.Fonts;
using MWFontToRead = FreeMax_File_Manager.Properties.UserSettings.Text.MainWindow.FontsToRead;
using SWFontSettings = FreeMax_File_Manager.Properties.UserSettings.Text.SettingsWindow.Fonts;
using SWFontToRead = FreeMax_File_Manager.Properties.UserSettings.Text.SettingsWindow.FontsToRead;
using AWFontSettings = FreeMax_File_Manager.Properties.UserSettings.Text.AdditionalWindow.Fonts;
using AWFontToRead = FreeMax_File_Manager.Properties.UserSettings.Text.AdditionalWindow.FontsToRead;
using ColorSettings = FreeMax_File_Manager.Properties.UserSettings.Colors;

namespace FreeMax_File_Manager.Windows {
	public enum Windows {
		NoWindow = -1,
		Main = 0,
		Settings = 1,
		Additional = 2
	}

	public enum Menus {
		Colors = 0,
		Fonts = 1
	}

	/// <summary>
	///     Логика взаимодействия для wSettings.xaml
	/// </summary>
	public partial class SettingsWindow {
		private readonly SettingColorElement[] _emptyColorElements;
		private readonly int _menuCount;
		private readonly TextBlock[] _menus;
		private Menus _activeMenu = Menus.Colors;

		private Windows _activeWindow = Windows.NoWindow;

		private GlyphTypeface[] _fonts;
		private string[] _fontsNames;

		private SettingColorElement[] _mainColorColorElements,
									_settingsColorColorElements,
									_windowColorElements,
									_additionalColorColorElements;

		private int _mainColorCount,
					_settingsColorCount,
					_additionalColorCount,
					_windowsCount,
					_mainWindowFontsCount,
					_settingsWindowFontCount,
					_additionalWindowsFontCount,
					_lastIndex;

		private SettingFontElement[] _mainWindowFontElements,
									_settingsWindowFontElements,
									_additionalWindowsFontElements;

		public SettingsWindow() {
			InitializeComponent();
			_menus = new[] {TbMenu1, TbMenu2};
			_menuCount = SpMenu.Children.Count;
			GetParams();
			Update();
			LbSetting.SelectedIndex = 0;
			_emptyColorElements = new SettingColorElement[0];
		}

		private void Update() {
			for(var i = 0; i < _menuCount; i++)
				_menus[i].Style = (Style) ((Menus) i == _activeMenu
												? FindResource("StackPanelActiveStyle")
												: FindResource("StackPanelPasiveStyle"));

			switch(_activeMenu) {
				case Menus.Colors:
					switch(_activeWindow) {
						case Windows.NoWindow:
							LbSetting.ItemsSource = _windowColorElements;
							break;
						case Windows.Main:
							LbSetting.ItemsSource = _mainColorColorElements;
							break;
						case Windows.Settings:
							LbSetting.ItemsSource = _settingsColorColorElements;
							break;
						case Windows.Additional:
							LbSetting.ItemsSource = _additionalColorColorElements;
							break;
					}
					break;
				case Menus.Fonts:
					switch(_activeWindow) {
						case Windows.NoWindow:
							LbSetting.ItemsSource = _windowColorElements;
							break;
						case Windows.Main:
							LbSetting.ItemsSource = _mainWindowFontElements;
							break;
						case Windows.Settings:
							LbSetting.ItemsSource = _settingsWindowFontElements;
							break;
						case Windows.Additional:
							LbSetting.ItemsSource = _additionalWindowsFontElements;
							break;
					}
					break;
			}
		}

		private void WSettings_KeyDown(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Add:
					AddProcess();
					break;
				case Key.Subtract:
					SubProcess();
					break;
			}
		}

		private void SubProcess() {
			switch (_activeMenu) {
				case Menus.Colors:
					switch (((SettingColorElement)LbSetting.SelectedItem).Active) {
						case 0:
							((SettingColorElement)LbSetting.SelectedItem).A--;
							break;
						case 1:
							((SettingColorElement)LbSetting.SelectedItem).R--;
							break;
						case 2:
							((SettingColorElement)LbSetting.SelectedItem).G--;
							break;
						case 3:
							((SettingColorElement)LbSetting.SelectedItem).B--;
							break;
					}
					break;
				case Menus.Fonts:
					var fontElement = LbSetting.SelectedItems[0] as SettingFontElement;
					if (fontElement == null)
						return;
					var font = fontElement.FontGlyph;
					font = _fonts[(_fonts.ToList().IndexOf(font) - 1 + _fonts.Length) % _fonts.Length];
					switch (_activeWindow) {
						case Windows.Main:
							_mainWindowFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_mainWindowFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_mainWindowFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _mainWindowFontElements[LbSetting.SelectedIndex].Text);
							break;
						case Windows.Settings:
							_settingsWindowFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_settingsWindowFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_settingsWindowFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _settingsWindowFontElements[LbSetting.SelectedIndex].Text);
							break;
						case Windows.Additional:
							_additionalWindowsFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_additionalWindowsFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_additionalWindowsFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _additionalWindowsFontElements[LbSetting.SelectedIndex].Text);
							break;
					}
					break;
			}

			Update();
			LbSetting.Items.Refresh();
		}

		private void AddProcess() {
			switch (_activeMenu) {
				case Menus.Colors:
					switch (((SettingColorElement)LbSetting.SelectedItem).Active) {
						case 0:
							((SettingColorElement)LbSetting.SelectedItem).A++;
							break;
						case 1:
							((SettingColorElement)LbSetting.SelectedItem).R++;
							break;
						case 2:
							((SettingColorElement)LbSetting.SelectedItem).G++;
							break;
						case 3:
							((SettingColorElement)LbSetting.SelectedItem).B++;
							break;
					}
					break;
				case Menus.Fonts:
					var fontElement = LbSetting.SelectedItems[0] as SettingFontElement;
					if (fontElement == null)
						return;
					var font = fontElement.FontGlyph;
					font = _fonts[(_fonts.ToList().IndexOf(font) + 1) % _fonts.Length];
					switch (_activeWindow) {
						case Windows.Main:
							_mainWindowFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_mainWindowFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_mainWindowFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _mainWindowFontElements[LbSetting.SelectedIndex].Text);
							break;
						case Windows.Settings:
							_settingsWindowFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_settingsWindowFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_settingsWindowFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _settingsWindowFontElements[LbSetting.SelectedIndex].Text);
							break;
						case Windows.Additional:
							_additionalWindowsFontElements[LbSetting.SelectedIndex].FontGlyph = font;
							_additionalWindowsFontElements[LbSetting.SelectedIndex].Text = font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
							_additionalWindowsFontElements[LbSetting.SelectedIndex].Font = new FontFamily(font.FontUri, _additionalWindowsFontElements[LbSetting.SelectedIndex].Text);
							break;
					}
					break;
			}
			Update();
			LbSetting.Items.Refresh();
		}

		private void WSettings_KeyUp(object sender, KeyEventArgs e) {
			switch(e.Key) {
				case Key.Down:
					DownProcess();
					break;
				case Key.Up:
					UpProcess();
					break;
				case Key.Enter:
					EnterProcess();
					break;
				case Key.Back:
					BackProcess();
					break;
				case Key.Tab:
					TabProcess();
					break;
				case Key.Escape:
					Close();
					break;
				case Key.Right:
					RightProcess();
					break;
				case Key.Left:
					LeftProcess();
					break;
			}
			if (Keyboard.Modifiers != ModifierKeys.Control || e.Key != Key.S)
				return;
			switch (_activeMenu) {
				case Menus.Colors:
					SaveColors();
					break;
				case Menus.Fonts:
					SaveFonts();
					break;
			}
		}

		private void TabProcess() {
			if(_activeMenu == Menus.Fonts || _activeWindow == Windows.NoWindow)
				return;
			((SettingColorElement) LbSetting.SelectedItem).Active = (((SettingColorElement) LbSetting.SelectedItem).Active + (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)? -1 : 1) + 4)%4; //4 - Windows count
			var temp = LbSetting.ItemsSource;
			var selId = LbSetting.SelectedIndex;
			LbSetting.ItemsSource = _emptyColorElements;
			LbSetting.ItemsSource = temp;
			LbSetting.SelectedIndex = selId;
		}

		private void BackProcess() {
			if(_activeWindow == Windows.NoWindow) return;
			if(_activeMenu == Menus.Colors) {
				((SettingColorElement) LbSetting.Items[LbSetting.SelectedIndex]).Active = 0;
				((SettingColorElement) LbSetting.Items[LbSetting.SelectedIndex]).IsSelected = false;
			}
			_activeWindow = Windows.NoWindow;
			Update();
			LbSetting.SelectedIndex = _lastIndex;
		}

		private void UpProcess() {
			if(_activeMenu == Menus.Colors) {
				((SettingColorElement) LbSetting.SelectedItem).IsSelected = false;
				((SettingColorElement) LbSetting.SelectedItem).Active = 0;
			}
			LbSetting.SelectedIndex = ChangeActiveMenu('-', LbSetting.SelectedIndex, LbSetting.Items.Count);
			if(_activeMenu == Menus.Colors)
				((SettingColorElement) LbSetting.SelectedItem).IsSelected = true;
			var temp = LbSetting.ItemsSource;
			var selId = LbSetting.SelectedIndex;
			LbSetting.ItemsSource = _emptyColorElements;
			LbSetting.ItemsSource = temp;
			LbSetting.SelectedIndex = selId;
		}

		private void DownProcess() {
			if(_activeMenu == Menus.Colors) {
				((SettingColorElement) LbSetting.SelectedItem).IsSelected = false;
				((SettingColorElement) LbSetting.SelectedItem).Active = 0;
			}
			LbSetting.SelectedIndex = ChangeActiveMenu('+', LbSetting.SelectedIndex, LbSetting.Items.Count);
			if(_activeMenu == Menus.Colors)
				((SettingColorElement) LbSetting.SelectedItem).IsSelected = true;
			var temp = LbSetting.ItemsSource;
			var selId = LbSetting.SelectedIndex;
			LbSetting.ItemsSource = _emptyColorElements;
			LbSetting.ItemsSource = temp;
			LbSetting.SelectedIndex = selId;
		}

		private void RightProcess() {
			_activeMenu = (Menus) ChangeActiveMenu('+', (int) _activeMenu, _menuCount);
			_activeWindow = Windows.NoWindow;
			Update();
			LbSetting.SelectedIndex = 0;
		}

		private void LeftProcess() {
			_activeMenu = (Menus) ChangeActiveMenu('-', (int) _activeMenu, _menuCount);
			_activeWindow = Windows.NoWindow;
			Update();
			LbSetting.SelectedIndex = 0;
		}

		private void SaveColors() {
			switch(_activeWindow) {
				case Windows.Main:
					foreach(SettingColorElement se in LbSetting.Items)
						ColorSettings.MainWindowColors.Default[se.Name] = se.Color.ToString();
					ColorSettings.MainWindowColors.Default.Save();
					break;
				case Windows.Settings:
					foreach(SettingColorElement se in LbSetting.Items)
						ColorSettings.SettingsWindowColors.Default[se.Name] = se.Color.ToString();
					ColorSettings.SettingsWindowColors.Default.Save();
					break;
				case Windows.Additional:
					foreach(SettingColorElement se in LbSetting.Items)
						ColorSettings.AdditionalWindowColors.Default[se.Name] = se.Color.ToString();
					ColorSettings.AdditionalWindowColors.Default.Save();
					break;
			}
		}

		private void SaveFonts() {
			switch(_activeWindow) {
				case Windows.Main:
					foreach(SettingFontElement se in LbSetting.Items)
						MWFontSettings.Default[se.Name] = _fontsNames[_fonts.ToList().IndexOf(se.FontGlyph)];
					MWFontSettings.Default.Save();
					break;
				case Windows.Settings:
					foreach(SettingFontElement se in LbSetting.Items)
						SWFontSettings.Default[se.Name] = _fontsNames[_fonts.ToList().IndexOf(se.FontGlyph)];
					SWFontSettings.Default.Save();
					break;
				case Windows.Additional:
					foreach(SettingFontElement se in LbSetting.Items)
						AWFontSettings.Default[se.Name] = _fontsNames[_fonts.ToList().IndexOf(se.FontGlyph)];
					AWFontSettings.Default.Save();
					break;
			}
		}

		private void EnterProcess() {
			_lastIndex = LbSetting.SelectedIndex;
			switch(_activeMenu) {
				case Menus.Colors:
					if(_activeWindow == Windows.NoWindow)
						switch(LbSetting.SelectedIndex) {
							case 0:
								_activeWindow = Windows.Main;
								Update();
								LbSetting.SelectedIndex = 0;
								((SettingColorElement) LbSetting.SelectedItem).IsSelected = true;
								break;
							case 1:
								_activeWindow = Windows.Settings;
								Update();
								LbSetting.SelectedIndex = 0;
								((SettingColorElement) LbSetting.SelectedItem).IsSelected = true;
								break;
							case 2:
								_activeWindow = Windows.Additional;
								Update();
								LbSetting.SelectedIndex = 0;
								((SettingColorElement) LbSetting.SelectedItem).IsSelected = true;
								break;
						}
					break;
				case Menus.Fonts:
					if(_activeWindow == Windows.NoWindow)
						switch(LbSetting.SelectedIndex) {
							case 0:
								_activeWindow = Windows.Main;
								Update();
								LbSetting.SelectedIndex = 0;
								break;
							case 1:
								_activeWindow = Windows.Settings;
								Update();
								LbSetting.SelectedIndex = 0;
								break;
							case 2:
								_activeWindow = Windows.Additional;
								Update();
								LbSetting.SelectedIndex = 0;
								break;
						}
					break;
			}
		}

		private static int ChangeActiveMenu(char c, int menu, int maxCount) { return c == '-'? ((menu == 0? maxCount : menu) - 1) : ((menu + 1)%maxCount); }

		private void GetParams() {
			ReadFontsFromFolder();
			GetColors();
			GetFonts();

			for(var i = 1; i <= _windowsCount; i++) {
				_windowColorElements[i - 1] = new SettingColorElement {
																		Name = (string) TextSettings.Default["Window" + i], Color = null, Text = null, Editable = false
																	};
			}
		}

		private void GetFonts() {
			_mainWindowFontsCount = MWFontSettings.Default.Count;
			_mainWindowFontElements = new SettingFontElement[_mainWindowFontsCount];
			for(var i = 0; i < _mainWindowFontsCount; i++) //TODO переименовать настройки, а то для пользователей не катит
			{
				_mainWindowFontElements[i] = new SettingFontElement {
																		Name = MWFontToRead.Default["T" + i].ToString(), ColorSet = false, Editable = true
																	};

				var font = new FontFamily(MWFontSettings.Default[_mainWindowFontElements[i].Name].ToString());

				_mainWindowFontElements[i].FontGlyph = _fonts[_fontsNames.ToList().IndexOf(font.ToString())];

				_mainWindowFontElements[i].Text = font.Source.Substring(font.Source.LastIndexOf('#') + 1);

				_mainWindowFontElements[i].Font = new FontFamily(_mainWindowFontElements[i].FontGlyph.FontUri, _mainWindowFontElements[i].Text);
			}

			_settingsWindowFontCount = SWFontSettings.Default.Count;
			_settingsWindowFontElements = new SettingFontElement[_settingsWindowFontCount];
			for(var i = 0; i < _settingsWindowFontCount; i++) {
				_settingsWindowFontElements[i] = new SettingFontElement {
																			Name = SWFontToRead.Default["T" + i].ToString(), ColorSet = false, Editable = true
																		};

				var font = new FontFamily(SWFontSettings.Default[_settingsWindowFontElements[i].Name].ToString());

				_settingsWindowFontElements[i].FontGlyph = _fonts[_fontsNames.ToList().IndexOf(font.ToString())];

				_settingsWindowFontElements[i].Text = font.Source.Substring(font.Source.LastIndexOf('#') + 1);

				_settingsWindowFontElements[i].Font = new FontFamily(_settingsWindowFontElements[i].FontGlyph.FontUri, _settingsWindowFontElements[i].Text);
			}

			_additionalWindowsFontCount = AWFontSettings.Default.Count;
			_additionalWindowsFontElements = new SettingFontElement[_additionalWindowsFontCount];
			for(var i = 0; i < _additionalWindowsFontCount; i++) {
				_additionalWindowsFontElements[i] = new SettingFontElement {
																				Name = AWFontToRead.Default["T" + i].ToString(), ColorSet = false, Editable = true
																			};
				var font = new FontFamily(AWFontSettings.Default[_additionalWindowsFontElements[i].Name].ToString());

				_additionalWindowsFontElements[i].FontGlyph = _fonts[_fontsNames.ToList().IndexOf(font.ToString())];

				_additionalWindowsFontElements[i].Text = font.Source.Substring(font.Source.LastIndexOf('#') + 1);

				_additionalWindowsFontElements[i].Font = new FontFamily(_additionalWindowsFontElements[i].FontGlyph.FontUri, _additionalWindowsFontElements[i].Text);
			}
		}

		private void ReadFontsFromFolder() {
			var dir = new DirectoryInfo(@"Properties\Resources\Font");
			_fonts = dir.GetFiles().Where(f => Equals(f.FullName.ToLower().Substring(f.FullName.Length - "otf".Length), "otf") || Equals(f.FullName.ToLower().Substring(f.FullName.Length - "ttf".Length), "ttf")).Select(font => new GlyphTypeface(new Uri(font.FullName))).ToArray();
			_fontsNames = new string[_fonts.Length];
			var i = 0;
			foreach(var font in _fonts) {
				_fontsNames[i++] = @"\Properties\Resources\Font\" + font.FontUri.OriginalString.Substring(font.FontUri.OriginalString.LastIndexOf("\\", StringComparison.Ordinal) + 1) + "#" + font.FamilyNames[CultureInfo.GetCultureInfo("en-US")];
			}
		}

		private void GetColors() {
			_mainColorCount = TextSettings.Default.MainColorCount;
			_settingsColorCount = TextSettings.Default.SettingsColorCount;
			_windowsCount = TextSettings.Default.WindowCount;
			_additionalColorCount = TextSettings.Default.AdditionalColorCount;

			_mainColorColorElements = new SettingColorElement[_mainColorCount];
			_settingsColorColorElements = new SettingColorElement[_settingsColorCount];
			_additionalColorColorElements = new SettingColorElement[_additionalColorCount];
			_windowColorElements = new SettingColorElement[_windowsCount];

			for(var i = 1; i <= _mainColorCount; i++)
				_mainColorColorElements[i - 1] = new SettingColorElement {
																			Name = (string) TextSettings.Default["MW" + i], Text = (string) ColorSettings.MainWindowColors.Default[(string) TextSettings.Default["MW" + i]], Editable = true, ColorSet = true, IsSelected = false
																		};

			for(var i = 1; i <= _settingsColorCount; i++)
				_settingsColorColorElements[i - 1] = new SettingColorElement {
																				Name = (string) TextSettings.Default["SW" + i], Text = (string) ColorSettings.SettingsWindowColors.Default[(string) TextSettings.Default["SW" + i]], Editable = true, ColorSet = true, IsSelected = false
																			};

			for(var i = 1; i <= _additionalColorCount; i++)
				_additionalColorColorElements[i - 1] = new SettingColorElement {
																					Name = (string) TextSettings.Default["AW" + i], Text = (string) ColorSettings.AdditionalWindowColors.Default[(string) TextSettings.Default["AW" + i]], Editable = true, ColorSet = true, IsSelected = false
																				};
		}

		private void SW_OnSizeChanged(object sender, SizeChangedEventArgs e) { Left -= (e.NewSize.Width - e.PreviousSize.Width)/2; }

		private void SW_OnActivated(object sender, EventArgs e) {
			Effect = new BlurEffect {
										Radius = 0, KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Quality
									};
		}

		private void SW_OnDeactivated(object sender, EventArgs e) {
			Effect = new BlurEffect {
										Radius = 5, KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Quality
									};
		}
	}
}
