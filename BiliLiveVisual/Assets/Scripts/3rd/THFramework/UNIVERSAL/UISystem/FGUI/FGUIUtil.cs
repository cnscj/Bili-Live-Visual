using System;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    /// <summary>
    /// 这个类的作用是封装一些Fgui的方法，
    /// </summary>
    public static class FGUIUtil
    {
        public class BaseArgs
        {
            public string packageName;
            public string componentName;

            public Vector2 xy = new Vector2(0,0);
            public Vector2 size = new Vector2(100, 100);
            public Vector2 pivot = new Vector2(0,0);
            public bool pivotAsAnchor = false;
            public Vector2 scale = new Vector2(1, 1);
            public float alpha = 1f;
            public bool center = false;
            public string title;

            public FComponent parent = UIManager.GetInstance().Root;
        }

        public class LabelArgs : BaseArgs
        {
            public class Style
            {
                public Color color = Color.black;
                public int size = 20;
                public string font = "";
                public bool underline = false;
            }
            public Style style = new Style();
        }

        public class GraphArgs : BaseArgs
        {
            public class Wrapper
            {
                public GameObject target;
                public bool cloneMaterial;
            }
            public Wrapper wrapper;
        }

        public class ButtonArgs : BaseArgs
        {
            public EventCallback1 onClick;
        }

        public class RichTextArgs : BaseArgs
        {

        }

        public class TextInputArgs : BaseArgs
        {
            public string promptText;
        }

        public class ListArgs : BaseArgs
        {
            public FComponent template;
            public FList.ItemStateFuncT0 onState;
            public EventCallback1 onClickItem;
        }

        public class LoaderArgs : BaseArgs
        {
            public string url;
            public NTexture texture;
        }
        ///////////////////////////

        public static FComponent NewComponent(BaseArgs baseArgs = null)
        {
            var fComponent = NewT<FComponent, GComponent>(baseArgs);
            return fComponent;
        }

        public static FButton NewButton(ButtonArgs buttonArgs = null)
        {
            var fButton = NewT<FButton, GButton>(buttonArgs);
            if (buttonArgs != null)
            {
                if (buttonArgs.onClick != null) fButton.OnClick(buttonArgs.onClick);
            }

            return fButton;
        }

        public static FGraph NewGraph(GraphArgs graphArgs = null)
        {
            var fGraph = NewT<FGraph, GGraph>(graphArgs);
            if (graphArgs != null)
            {
                if (graphArgs.wrapper != null)
                {
                    var goWrapper = new GoWrapper();
                    goWrapper.SetWrapTarget(graphArgs.wrapper.target, graphArgs.wrapper.cloneMaterial);
                    fGraph.SetNativeObject(goWrapper);
                }
            }
            return fGraph;
        }

        public static FLabel NewLabel(LabelArgs labelArgs = null)
        {
            var fLabel = NewT<FLabel, GLabel>(labelArgs);
            if (labelArgs != null)
            {
                fLabel.SetColor(labelArgs.style.color);
            }

            return fLabel;
        }

        public static FRichText NewRichText(RichTextArgs richTextArgs = null)
        {
            var fRichText = NewT<FRichText, GRichTextField>(richTextArgs);
            if (richTextArgs != null)
            {

            }
            return fRichText;
        }

        public static FTextInput NewTextInput(TextInputArgs textInputArgs = null)
        {
            var fTextInput = NewT<FTextInput, GTextInput>(textInputArgs);
            if (textInputArgs != null)
            {
                fTextInput.SetPlaceHolder(textInputArgs.promptText);
            }
            
            return fTextInput;
        }

        public static FLoader NewLoader(LoaderArgs loaderArgs = null)
        {
            var fLoader = NewT<FLoader>(UIObjectFactory.NewObject(ObjectType.Loader), loaderArgs);
            InitBaseArgs(fLoader, loaderArgs);
            if (loaderArgs != null)
            {
                if (loaderArgs.texture != null)
                {
                    fLoader.SetTexture(loaderArgs.texture);
                }
                else
                {
                    fLoader.SetUrl(loaderArgs.url);
                }
            }
            
            return fLoader;
        }

        public static FList NewList(ListArgs listArgs)
        {
            var fList = NewT<FList, GList>(listArgs);
            if (listArgs != null)
            {
                if (listArgs.onState != null) fList.SetState(listArgs.onState);
                if (listArgs.onClickItem != null) fList.OnClickItem(listArgs.onClickItem);
            }

            return fList;
        }

        private static T NewT<T>(GObject fguiObj, BaseArgs baseArgs) where T : FComponent, new()
        {
            T fComponent = null;
 
            if (baseArgs != null && !string.IsNullOrEmpty(baseArgs.packageName))
            {
                fComponent = FComponent.Create<T>(baseArgs.packageName, baseArgs.componentName);
            }
            else
            {
                fComponent = FComponent.Create<T>(fguiObj);
            }
            InitBaseArgs(fComponent, baseArgs);
            return fComponent;
        }
        private static T1 NewT<T1, T2>(BaseArgs baseArgs) where T1 : FComponent, new() where T2 : GObject, new()
        {
            var fComponent = NewT<T1>(new T2(), baseArgs);
            InitBaseArgs(fComponent, baseArgs);

            return fComponent;
        }

        /// <summary>
        /// 初始化基准组件
        /// </summary>
        /// <param name="fComponent"></param>
        /// <param name="baseArgs"></param>
        private static void InitBaseArgs(FComponent fComponent, BaseArgs baseArgs)
        {
            if (fComponent == null)
                return;

            if (baseArgs == null)
                return;

            fComponent.SetXY(baseArgs.xy);
            fComponent.SetSize(baseArgs.size);
            fComponent.SetPivot(baseArgs.pivot.x, baseArgs.pivot.y, baseArgs.pivotAsAnchor);
            fComponent.SetScale(baseArgs.scale.x, baseArgs.scale.y);
            fComponent.SetAlpha(baseArgs.alpha);
            fComponent.SetText(baseArgs.title);

            if (baseArgs.center) fComponent.Center();
            if (baseArgs.parent != null) baseArgs.parent.AddChild(fComponent);
        }

        ///////////////////////////
        ///
        public static GComponent CreateLayerObject(int sortingOrder, string layerName = null)
        {
            var obj = new GComponent();
            obj.sortingOrder = sortingOrder;
            obj.SetSize(GRoot.inst.width, GRoot.inst.height);
            obj.AddRelation(GRoot.inst, RelationType.Size);
            GRoot.inst.AddChild(obj);

            if (!string.IsNullOrEmpty(layerName))
            {
                obj.rootContainer.gameObject.name = layerName;
            }

            return obj;
        }

        public static string GetUIUrl(string package, string component)
        {
            return UIPackage.GetItemURL(package, component);
        }

      
    }
}
