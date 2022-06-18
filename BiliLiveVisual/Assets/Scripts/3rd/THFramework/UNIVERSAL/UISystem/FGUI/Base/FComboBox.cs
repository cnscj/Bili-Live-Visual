
using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;

namespace THGame.UI
{

    public class FComboBox : FComponent
    {
        protected IList _dataProvider;
        Func<int,object,string> _stateFunc;

        public void SetList(string[] array)
        {
            _obj.asComboBox.items = array;
        }

        public string[] GetList()
        {
            return _obj.asComboBox.items;
        }

        public string GetSelectedText()
        {
            return _obj.asComboBox.items[_obj.asComboBox.selectedIndex];
        }

        public void SetSelectedIndex(int index, bool call = true)
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

        public void OnChanged(EventCallback0 func)
        {
            _obj.asComboBox.onChanged.Add(func);
        }
        public void OnChanged(EventCallback1 func)
        {
            _obj.asComboBox.onChanged.Add(func);
        }

        public void SetState(Func<int, object, string> stateFunc)
        {
            _stateFunc = stateFunc;
        }
        public void SetDataProvider(IList array)
        {
            _dataProvider = array;
            if (_dataProvider != null)
            {
                if (_stateFunc != null)
                {
                    List<string> titleList = new List<string>();
                    for(int i = 0;i < _dataProvider.Count;i++)
                    {
                        var title = _stateFunc(i, _dataProvider[i]) ?? "";
                        titleList.Add(title);
                    }
                    SetList(titleList.ToArray());
                }
            }
            else
            {
                SetVisibleItemCount(0);
            }

        }

        public IList GetDataProvider()
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
