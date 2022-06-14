using UnityEngine;
using System.Collections.Generic;
namespace THGame.UI
{

    public class FLabel : FComponent
    {
        public new string GetText()
        {
            return _obj.asTextField.text;
        }

        public new string SetText(string value)
        {
            return _obj.asTextField.text = value;
        }

        public void SetColor(Color color)
        {
            _obj.asTextField.color = color;
        }
        public Color GetClolor()
        {
            return _obj.asTextField.color;
        }

        public void SetFont(string font)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.font = font;
        }
        public void SetFontSize(int size)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.size = size;
        }

        public int GetFontSize()
        {
            var textFormat = _obj.asTextField.textFormat;
            return textFormat.size;
        }

        public void SetTemplateVars(Dictionary<string,string> values)
        {
            _obj.asTextField.templateVars = values;
        }

        public void SetVar(string key, string value)
        {
            _obj.asTextField.SetVar(key, value).FlushVars(); ;
        }
        public void SetLetterSpacing(int letterSpacing)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.letterSpacing = letterSpacing;
        }
        public void SetLineSpacing(int lineSpacing)
        {
            var textFormat = _obj.asTextField.textFormat;
            textFormat.lineSpacing = lineSpacing;
        }

        //设置根据字数自动调节字距的文本
        public void SetGapText(string text)
        {
            int fontSize = GetFontSize();
            int fontNum = text.Length;
            int gap = GetGapText(fontSize, fontNum);
            SetLetterSpacing(gap);
            SetText(text);
        }

        //根据字数和字体大小计算间距
        public int GetGapText(int fontSize, int fontNum)
        {
            int maxWidth = 75;
            if (fontNum <= 1)
            {
                return 0;
            }
            int width = fontNum * fontSize;
            if(width < maxWidth)
            {
                return (int)Mathf.Floor(((maxWidth - width) / (fontNum - 1)));
            }
            return 0;
        }
    }

}
