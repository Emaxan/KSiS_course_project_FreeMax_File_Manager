namespace GeneralClasses {
	public interface ISelectable {
		void Select();
		void UnSelect();
        bool IsSelected{ get; set; }
	}
}