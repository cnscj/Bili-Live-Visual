using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    public class ObjectManager : Singleton<ObjectManager>
    {
        private Dictionary<GObject, FObject> _mapper = new Dictionary<GObject, FObject>();

        public bool Add(GObject gObj, FObject fObj, bool isReplace = true)
        {
            if (gObj == null || fObj == null)
                return false;

            if (_mapper.ContainsKey(gObj))
            {
                if (isReplace)
                {
                    _mapper.Remove(gObj);
                }
                else
                {
                    return false;
                }
            }

            gObj.onRemovedFromStage.Add(OnRemovedFromStage);
            _mapper.Add(gObj, fObj);
            return true;
        }

        public void Remove(GObject gObj)
        {
            if (gObj == null)
                return;

            gObj.onRemovedFromStage.Remove(OnRemovedFromStage);
            _mapper.Remove(gObj);
        }

        public FObject Get(GObject gObj)
        {
            if (gObj == null)
                return null;

            if (_mapper.TryGetValue(gObj,out var fObj))
            {
                return fObj;
            }
            return null;
        }

        public T GetOrCreate<T>(GObject gObj) where T :FObject, new()
        {
            var fObj = Get(gObj);
            if (fObj == null)
            {
                fObj = new T();
                if (gObj != null)
                {
                    fObj.InitWithObj(gObj);

                    Add(gObj, fObj);
                }
            }
            return fObj as T;
        }

        public FObject GetOrCreate(GObject gObj)
        {
            return GetOrCreate<FObject>(gObj);
        }

        public void Clear()
        {
            _mapper.Clear();
        }

        private void OnRemovedFromStage(EventContext context)
        {
            var gObj = context.sender as GObject;
            //if (gObj.isDisposed)    //FIXME:Remove是在Dispose之前的,所以可能判断不了
            //{
                Remove(gObj);
            //}

        }
    }
}
