namespace Kostki.ViewModels
{
    public class ItemViewModel
    {
        private string _firstItem;

        public string FirstItem
        {
            get
            {
                return this._firstItem;
            }
            set
            {
                if (value != this._firstItem)
                {
                    this._firstItem = value;
                }
            }
        }

        private string _secondItem;

        public string SecondItem
        {
            get
            {
                return this._secondItem;
            }
            set
            {
                if (value != this._secondItem)
                {
                    this._secondItem = value;
                }
            }
        }

        private string _color;

        public string Color
        {
            get
            {
                return this._color;
            }
            set
            {
                if (value != this._color)
                {
                    this._color = value;
                }
            }
        }
    }
}
