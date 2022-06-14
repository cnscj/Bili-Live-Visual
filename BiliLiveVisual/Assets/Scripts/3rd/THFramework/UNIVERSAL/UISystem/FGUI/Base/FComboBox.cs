
using System.Collections.Generic;
using FairyGUI;

namespace THGame.UI
{

    public class FComboBox : FComponent
    {
        protected List<object> _dataProvider;
        public void SetList(string[] list)
        {
            _obj.asComboBox.items = list;
        }

        public string[] GetList()
        {
            return _obj.asComboBox.items;
        }

        public string GetSelectedText()
        {
            return _obj.asComboBox.items[_obj.asComboBox.selectedIndex];
        }

        public void SetSelectedIndex(int index, bool call)
        {
            _obj.asComboBox.selectedIndex = index;
        
            if (call)
            {
                _obj.asComboBox.onChanged.Call();
            }
        }

        public int GetSelectedIndex()
        {
            return _obj.asComboBox.selectedIndex;
        }

        public object GetSelectedData()
        {
            if (_dataProvider != null)
            {
                int index = GetSelectedIndex();
                return _dataProvider[index];
            }
            return null;
        }

        public new void SetText(string text)
        {
            _obj.asComboBox.title = text;
        }
        public new string GetText()
        {
            return _obj.asComboBox.title;
        }

        public void OnChanged(EventCallback1 func)
        {
            _obj.asComboBox.onChanged.Add(func);
        }

        public void SetDataProvider<T>(List<T> array)
        {
            List<object> list = array.ConvertAll(s => (object)s);
            _dataProvider = list;
        }

        public List<object> GetDataProvider()
        {
            return _dataProvider;
        }

        public void SetVisibleItemCount(int count)
        {
            _obj.asComboBox.visibleItemCount = count;
        }


        public void OnClosed(EventCallback1 func)
        {
            _obj.asComboBox.dropdown.onRemovedFromStage.Add(func);
        }

        /*
        FairyGUI.PopupDirection
        Auto
        Up
        Down
        */
        public void SetPopupDirection(PopupDirection dir)
        {
            _obj.asComboBox.popupDirection = dir;
        }
    }

}
