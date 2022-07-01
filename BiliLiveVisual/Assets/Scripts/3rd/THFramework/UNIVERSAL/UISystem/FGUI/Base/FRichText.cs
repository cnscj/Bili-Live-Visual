using FairyGUI;

namespace THGame.UI
{

    public class FRichText : FLabel
    {

        public new string GetText()
        {
            return _obj.asTextField.text;
        }

        public new void SetText(string value)
        {
            _obj.asTextField.text = value;
        }

        public void SetAutoSize(AutoSizeType type)
        {
            _obj.asRichTextField.autoSize = type;
        }


    }

}
