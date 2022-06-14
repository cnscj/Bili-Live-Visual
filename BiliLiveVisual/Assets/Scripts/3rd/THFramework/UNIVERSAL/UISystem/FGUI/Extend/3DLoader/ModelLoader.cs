using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public class ModelLoader : GameObjaceLoader
    {
        public class TargetParams
        {
            public bool isCloneMaterial;
        }

        public ModelLoader(string packageName, string componentName) : base(packageName, componentName)
        {

        }

        public void SetTarget(GameObject target, TargetParams targetParams = null)
        {
            var goWrapper = GetOrCreateGOWrapper();

            bool isCloneMaterial = (bool)(targetParams?.isCloneMaterial);

            goWrapper.SetWrapTarget(target, isCloneMaterial);
        }

        protected override XGoWrapper GetOrCreateGOWrapper()
        {
            return base.GetOrCreateGOWrapper();
        }
    }

}

