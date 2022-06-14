using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class GoAlphaUpdater : GoBaseUpdater
    {
        private MaterialPropertyBlock _materialPropertyBlock;
        private Color _fguiColor = Color.white;
        List<Renderer> _renderers;
        public GoAlphaUpdater()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        public override void OnReplace(GameObject oldGameObject, GameObject newGameObject)
        {
            CacheRenderers(newGameObject);
        }

        public override void OnRefresh()
        {
            CacheRenderers(context.wrapperTarget);
        }

        public override void OnUpdate()
        {
            if (context.wrapperTarget != null && context.wrapperContext != null)
            {
                //要区别,模型,特效,模型特效这些情况
                var alpha = context.wrapperContext.alpha;
                foreach(var renderer in _renderers)
                {
                    renderer.GetPropertyBlock(_materialPropertyBlock);

                    _fguiColor.a = alpha;
                    _materialPropertyBlock.SetColor("_FGUIColor", _fguiColor);

                    renderer.SetPropertyBlock(_materialPropertyBlock);
                }
            }
        }

        private void CacheRenderers(GameObject gameObject)
        {
            if (gameObject == null)
            {
                _renderers.Clear();
                return;
            }

            var rendererArray = gameObject.GetComponentsInChildren<Renderer>();
            if (rendererArray != null && rendererArray.Length >= 0)
            {
                _renderers = _renderers ?? new List<Renderer>();
                _renderers.Clear();

                _renderers.AddRange(rendererArray);
            }
        }
    }
}

