
using System.Collections.Generic;
using THGame.Tween;
using THGame.UI;
using UnityEngine;
using XLibGame;

namespace BLVisual
{
    public class MainUIDanmuShowLayer : FWidget
    {
        FComponent stage;
        FLabel danmuPerSecond;

        UIPool danmuComPool;
        int receiveCount = 0;
        int sendCount = 0;
        float lastTick = 0;
        int secondRecordCount = 0;
        int lastSecondRecordCount = 0;
        int partIndex = 0;
        Queue <BiliLiveDanmakuData.DanmuMsg> msgQueue = new Queue<BiliLiveDanmakuData.DanmuMsg>();

        public MainUIDanmuShowLayer()
        {
            _interval = 0.16f;
            danmuComPool = UIPoolManager.GetInstance().GetOrCreatePool<DanmuMsgCom>();
        }

        protected override void OnInitUI()
        {
            stage = GetChild<FComponent>("stage");
            danmuPerSecond = GetChild<FLabel>("danmuPerSecond");
        }

        protected void UpdateLayer()
        {
            stage.RemoveAllChildren();
            //danmuPerSecond.SetText(Language.GetString(10501, 0));
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
            AddEventListener(EventType.BILILIVE_START, OnBiliClientStart);
        }


        Vector2 CalculatePosition(FComponent danmuComp,FComponent stageComp)
        {
            var stageHeight = stageComp.GetHeight();
            var compHeight = danmuComp.GetHeight();

            var yParts = (int)(stageHeight / compHeight);

            //应该按顺序来
            if (partIndex >= yParts) partIndex = 0;
            else partIndex++;

            var x = stageComp.GetWidth() + 100;
            var y = partIndex * compHeight;

            return new Vector2(x,y);
        }
        void EmitDanmu(string text)
        {
            var comp = danmuComPool.GetOrCreate();
            var pos = CalculatePosition(comp, stage);

            comp.SetXY(pos);
            comp.SetText(text);

            stage.AddChild(comp);
            sendCount++;

            //理论上越长越快
            TweenUtil.CustomTweenFloat((t) =>
            {
                comp.SetX(t);
            }, comp.GetX()+100, -300, 15).SetCallBack((arg)=>
            {
                danmuComPool.Release(comp);
            });
        }

        protected void OnBiliClientStart(EventContext context)
        {
            msgQueue.Clear();
            receiveCount = 0;
            sendCount = 0;
            lastSecondRecordCount = 0;
        }
        protected void OnDanmuMsg(EventContext context)
        {
            var data = (BiliLiveDanmakuData.DanmuMsg)context.args[0];
            msgQueue.Enqueue(data);

            receiveCount++;
            secondRecordCount++;
        }

        private void UpdateDanmu()
        {
            while (msgQueue.Count > 0)
            {
                var data = msgQueue.Dequeue();
                EmitDanmu(data.content);
            }
        }

        private void UpdateState()
        {
            danmuPerSecond.SetText(string.Format("{0},{1},{2}", sendCount, receiveCount, lastSecondRecordCount));
        }

        private void UpdateFrames()
        {
            if (Time.realtimeSinceStartup - lastTick < 1f)
                return;


            lastSecondRecordCount = secondRecordCount;
            secondRecordCount = 0;
            lastTick = Time.realtimeSinceStartup;
        }


        protected override void OnEnter()
        {
            UpdateLayer();
            
        }

        protected override void OnExit()
        {

        }

        protected override void OnTick()
        {
            UpdateDanmu();
            UpdateState();
            UpdateFrames();


        }
    }
}
