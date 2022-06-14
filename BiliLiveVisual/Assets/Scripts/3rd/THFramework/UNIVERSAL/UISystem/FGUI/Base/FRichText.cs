using FairyGUI;

namespace THGame.UI
{

    public class FRichText : FLabel
    {

        public void SetAutoSize(AutoSizeType type)
        {
            _obj.asRichTextField.autoSize = type;
        }


    }

}
