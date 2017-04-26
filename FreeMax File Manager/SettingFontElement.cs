using System.Linq;
using System.Windows.Media;

namespace FreeMax_File_Manager {
	public class SettingFontElement {
		public bool Editable{ get; set; }

		public bool ColorSet{ get; set; }

		public string Name{ get; set; }

		public string GoodName {
			get { return Name.ToCharArray().Aggregate("", (current, c) => current + (c == '_'? ' ' : c)); }
		}

		public FontFamily Font{ get; set; }

		public GlyphTypeface FontGlyph{ get; set; }

		public string Text{ get; set; }

		public override string ToString() { return GoodName; }
	}
}