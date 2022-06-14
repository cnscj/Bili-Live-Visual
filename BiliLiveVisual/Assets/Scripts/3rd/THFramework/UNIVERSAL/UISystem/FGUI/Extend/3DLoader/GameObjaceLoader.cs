using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public class GameObjaceLoader : FWidget
    {
        public GameObjaceLoader(string packageName, string componentName) : base(packageName, componentName)
        {


        }

        public virtual void SetTarget(GameObject target)
        {

        }

        protected virtual XGoWrapper GetOrCreateGOWrapper()
        {
            var goWrapper = new XGoWrapper();
            goWrapper.AddUpdater<GoAlphaUpdater>();

            return goWrapper;
        }
    }

}

