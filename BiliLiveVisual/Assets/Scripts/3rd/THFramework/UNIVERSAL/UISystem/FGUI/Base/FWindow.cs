
using FairyGUI;

namespace THGame.UI
{

    public class FWindow : FComponent
    {
        private static readonly string iconName = "icon";
        private static readonly string titleName = "title";
        private static readonly string closeBtnName = "closeButton";
        private static readonly string dragAreaName = "dragArea";
        private static readonly string contentAreaName = "contentArea";

        public FLoader icon { get; protected set; }
        public FRichText title { get; protected set; }
        public FButton closeButton { get; protected set; }

        public FGraph dragArea { get; protected set; }
        public FGraph contentArea { get; protected set; }

        public void SetTitle(string text)
        {
            if (title != null)
            {
                title.SetText(text);
            }
        }

        public string GetTitle()
        {
            if (title != null)
            {
                return title.GetText();
            }
            return "";
        }

        public new void SetDraggable(bool val)
        {
            dragArea.SetDraggable(val);
        }


        public new bool GetDraggable()
        {
            return dragArea.GetDraggable();
        }

        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);

            icon = GetChild<FLoader>(iconName);
            title = GetChild<FRichText>(titleName);
            closeButton = GetChild<FButton>(closeBtnName);
            dragArea = GetChild<FGraph>(dragAreaName);
            contentArea = GetChild<FGraph>(contentAreaName);

            return this;
        }
    }

}
