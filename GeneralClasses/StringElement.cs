using System;
using Newtonsoft.Json;

namespace GeneralClasses {
    [Serializable]
    public class StringElement: ISelectable {
        public StringElement(string[] name) {
            Name = name[0];
            FullPath = name[1];
            IsDir = name[2] == "1";
        }

        [JsonConstructor]
        public StringElement(string name, string fullpath, bool isdir, bool isselected) {
            Name = name;
            FullPath = fullpath;
            IsSelected = isselected;
            IsDir = isdir;
        }

        public StringElement(string fullPath) {
            FullPath = fullPath;
        }

        public string Name{ get; set; }
        public string FullPath{ get; set; }
        public bool IsDir{ get; set; }
        public bool IsSelected{ get; set; }

        void ISelectable.Select() { IsSelected = true; }
        void ISelectable.UnSelect() { IsSelected = false; }
        public override string ToString() { return Name; }
    }
}
