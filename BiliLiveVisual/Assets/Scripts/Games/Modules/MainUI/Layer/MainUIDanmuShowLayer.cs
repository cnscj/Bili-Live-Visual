
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
        int lastIndex = 0;

        float emitSpeedTime = 15;
        float emitInterval = 0.1f;
        float emitPerCount = 20;
        float emitLastTick = 0;

        NorepeatRandomer randomer = new NorepeatRandomer();
        Queue <BiliLiveDanmakuData.DanmuMsg> msgQueue = new Queue<BiliLiveDanmakuData.DanmuMsg>();

        public MainUIDanmuShowLayer()
        {
            _interval = 0.016f;
            danmuComPool = UIPoolManager.GetInstance().GetOrCreatePool<MainUIDanmuMsgCom>();
        }

        protected override void OnInitUI()
        {
            stage = GetChild<FComponent>("stage");
            danmuPerSecond = GetChild<FLabel>("danmuPerSecond");
        }

        protected void UpdateLayer()
        {
            stage.RemoveAllChildren();
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.BILILIVE_DANMU_MSG, OnDanmuMsg);
            AddEventListener(EventType.BILILIVE_START, OnBiliClientStart);
            AddEventListener(EventType.DANMUSHOWLAYER_CHANGE_DANMU_ARGS, OnCahgeDanmuArgs);
        }

        private void OnCahgeDanmuArgs(EventContext context)
        {
            int type = (int)context.args[0];
            int val = (int)context.args[1];
            if(type == 1)
            {
                emitSpeedTime = val;
            }
            else if (type == 2)
            {
                emitPerCount = val;
            }
        }

        Vector2 CalculatePosition(FComponent danmuComp,FComponent stageComp)
        {
            var stageHeight = stageComp.GetHeight();
            var compHeight = danmuComp.GetHeight();

            var yParts = (int)(stageHeight / compHeight);
            //应该按顺序来
            //if (partIndex >= yParts) partIndex = 0;
            //else partIndex++;

            //乱序,如果同帧没效果
            do
            {
                partIndex = randomer.Range(0, yParts);
            } while (lastIndex == partIndex);
            lastIndex = partIndex;

            var x = stageComp.GetWidth();
            var y = partIndex * compHeight;

            return new Vector2(x,y);
        }

        //内容越长越快
        float CalculateTime(string content)
        {
            var strLen = content.Length;
            var part = strLen / emitSpeedTime;
            var reduceTime = (part * 5);
            return emitSpeedTime - reduceTime;
        }

        void EmitDanmu(BiliLiveDanmakuData.DanmuMsg data)
        {
            var comp = (MainUIDanmuMsgCom)danmuComPool.GetOrCreate();
            var pos = CalculatePosition(comp, stage);
            var time = CalculateTime(data.content);

            comp.SetMsgData(data);
            comp.SetXY(pos);

            stage.AddChild(comp);
            sendCount++;

            //理论上越长越快
            var widget = comp.GetWidth();
            var tweener = TweenUtil.CustomTweenFloat((t) =>
            {
                comp.SetX(t);
            }, comp.GetX(), -widget, time).SetCallBack((arg)=>
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
            if (Time.realtimeSinceStartup < emitLastTick + emitInterval)
                return;

            randomer.Clear();
            var emitCount = 0;
            while (msgQueue.Count > 0)
            {
                if (emitCount >= emitPerCount)
                    break;

                var data = msgQueue.Dequeue();
                EmitDanmu(data);

                emitCount++;
            }
            emitLastTick = Time.realtimeSinceStartup;
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
