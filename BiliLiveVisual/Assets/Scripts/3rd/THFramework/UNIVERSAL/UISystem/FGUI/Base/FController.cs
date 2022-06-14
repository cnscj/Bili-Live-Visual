using FairyGUI;

namespace THGame.UI
{

    public class FController : Wrapper<Controller>
    {
        public static FController Create(Controller obj)
        {
            if (obj != null)
            {
               return new FController().InitWithObj(obj) as FController;
            }
            return null;
        }

        public void SetSelectedIndex(int index)
        {
            _obj.selectedIndex = index;
        }

        public int GetSelectedIndex()
        {
            return _obj.selectedIndex;
        }

        public int GetPreviousIndex()
        {
            return _obj.previsousIndex;
        }

        public void SetSelectedName(string name)
        {
            _obj.selectedPage = name;
        }

        public string GetSelectedName()
        {
            var id = GetSelectedIndex();
            return _obj.GetPageName(id);
         }

        //获取页面数量
        public int GetPageCount()
        {
            return _obj.pageCount;
        }

        public void AddPage(string name)
        {
            _obj.AddPage(name);
        }

        public void OnChanged(EventCallback1 func)
        {
            _obj.onChanged.Add(func);
        }

        public void ClearPages()
        {
            _obj.ClearPages();
        }
    }

}
