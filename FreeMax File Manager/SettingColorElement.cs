using System;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace FreeMax_File_Manager {
	public class SettingColorElement {
		private byte _a, _r, _g, _b;
		private bool _change = true;
		private string _text;

		public bool ColorSet{ get; set; }

		public bool Editable{ get; set; }

		public bool IsSelected{ get; set; }

		public int Active{ get; set; }

		public string Name{ get; set; }

		public string GoodName {
			get { return Name.ToCharArray().Aggregate("", (current, c) => current + (c == '_'? ' ' : c)); }
		}

		public SolidColorBrush Color{ get; set; }

		public string Text {
			get { return _text; }
			set {
				if(value == null) return;
				_text = value;
				var convertFromString = ColorConverter.ConvertFromString(value);
				if(convertFromString != null)
					Color = new SolidColorBrush((Color) convertFromString);
				if(!_change) return;
				A = byte.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
				R = byte.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
				G = byte.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
				B = byte.Parse(value.Substring(7, 2), NumberStyles.HexNumber);
			}
		}

		public byte A {
			get { return _a; }
			set {
				_a = value;
				_change = false;
				Text = ConvertToHex(value, _r, _g, _b);
				_change = true;
			}
		}

		public byte R {
			get { return _r; }
			set {
				_r = value;
				_change = false;
				Text = ConvertToHex(_a, value, _g, _b);
				_change = true;
			}
		}

		public byte G {
			get { return _g; }
			set {
				_g = value;
				_change = false;
				Text = ConvertToHex(_a, _r, value, _b);
				_change = true;
			}
		}

		public byte B {
			get { return _b; }
			set {
				_b = value;
				_change = false;
				Text = ConvertToHex(_a, _r, _g, value);
				_change = true;
			}
		}

		private static string ConvertToHex(byte a, byte r, byte g, byte b) {
			var res = "#";
			res += a == 0? "00" : (a > 15? "" : "0") + Convert.ToString(a, 16);
			res += r == 0? "00" : (r > 15? "" : "0") + Convert.ToString(r, 16);
			res += g == 0? "00" : (g > 15? "" : "0") + Convert.ToString(g, 16);
			res += b == 0? "00" : (b > 15? "" : "0") + Convert.ToString(b, 16);
			return res;
		}

		public override string ToString() { return GoodName; }
	}
}