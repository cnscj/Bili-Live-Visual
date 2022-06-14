
using UnityEngine;

namespace THGame.UI
{
    public class EffectLoader : GameObjaceLoader
    {
        public EffectLoader(string packageName, string componentName):base(packageName, componentName)
        {

        }

        protected override XGoWrapper GetOrCreateGOWrapper()
        {
            var goWrapper = base.GetOrCreateGOWrapper();
            goWrapper.AddUpdater<GoSortingOrderUpdater>();
            return goWrapper;
        }
    }

}

