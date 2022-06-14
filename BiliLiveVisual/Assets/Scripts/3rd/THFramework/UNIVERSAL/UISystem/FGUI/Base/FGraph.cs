using UnityEngine;
using FairyGUI;
using System.Collections.Generic;

namespace THGame.UI
{

    public class FGraph : FComponent
    {
        public void DrawRect(float aWidth, float aHeight, int lineSize, Color lineColor, Color fillColor)
        {
            _obj.asGraph.DrawRect(aWidth, aHeight, lineSize, lineColor, fillColor);
        }

        public void DrawEllipse(float aWidth, float aHeight, Color fillColor)
        {
            _obj.asGraph.DrawEllipse(aWidth, aHeight, fillColor);
        }

        public void DrawRoundRect(float aWidth, float aHeight, Color fillColor, float[] corner)
        {
            _obj.asGraph.DrawRoundRect(aWidth, aHeight, fillColor, corner);
        }

        public void DrawPolygon(float aWidth, float aHeight, IList<Vector2> points, Color fillColor)
        {
            _obj.asGraph.DrawPolygon(aWidth, aHeight, points, fillColor);
        }

        public void SetNativeObject(DisplayObject displayObject)
        {
            _obj.asGraph.SetNativeObject(displayObject);
        }
    }

}
