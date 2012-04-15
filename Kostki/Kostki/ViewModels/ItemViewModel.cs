using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace Kostki.ViewModels
{
    public class ItemViewModel
    {
        private string _firstItem;

        public string firstItem
        {
            get
            {
                return _firstItem;
            }
            set
            {
                if (value != _firstItem)
                {
                    _firstItem = value;
                }
            }
        }

        private string _secondItem;

        public string secondItem
        {
            get
            {
                return _secondItem;
            }
            set
            {
                if (value != _secondItem)
                {
                    _secondItem = value;
                }
            }
        }

        private string _color;

        public string  color
        {
            get
            {
                return _color;
            }
            set
            {
                if (value != _color)
                {
                    _color = value;
                }
            }
        }
    }
}
