using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public class RedDot : FWidget
    {
        private string[] m_keys;

        private FGraph m_graph;
        public RedDot():base(null, "RedDot")
        {
            GetObject().onRemovedFromStage.Add(OnRemove);
        }

        public void SetKeys(params string[] args)
        {
            //反注册
            RedDotManager.GetInstance().Unregister(OnCall, m_keys);
            //注册
            RedDotManager.GetInstance().Register(OnCall, args);

            m_keys = args;
        }

        public string[] GetKeys()
        {
            return m_keys;
        }

        protected override void OnInitUI()
        {
            //生成相应的组件
            m_graph = Create<FGraph>(new FairyGUI.GGraph());
            m_graph.DrawEllipse(16, 16, Color.red);
            AddChild(m_graph);

        }

        protected virtual void OnCall(RedDotData date)
        {
            if (date.curStatus == RedDotStatus.Hide)
            {
                SetVisible(false);
            }
            else
            {
                SetVisible(true);
            }
        }

        private void OnRemove()
        {
            RedDotManager.GetInstance().Unregister(OnCall, m_keys);
        }


    }

}
