using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace THGame.UI
{
    //使支持SortingGroup
    public class GoSortingOrderUpdater : GoBaseUpdater
    {
        public override void OnReplace(GameObject oldGo, GameObject newGo)
        {
            var renderers = newGo.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length <= 0)
                return;

            foreach(var renderer in renderers)
            {
                int sortingGroupOrder;
                var sortingGroup = renderer.GetComponent<SortingGroup>();
                if (sortingGroup)
                {
                    sortingGroup.enabled = false;
                    sortingGroupOrder = sortingGroup.sortingOrder * 800;
                }
                else
                {
                    sortingGroupOrder = renderer.sortingOrder * 800;
                }

                Material[] mats = renderer.sharedMaterials;
                if (mats == null)
                {
                    continue;
                }

                int mcnt = mats.Length;
                for (int i = 0; i < mcnt; i++)
                {
                    Material mat = mats[i];
                    if (mat == null)
                    {
                        continue;
                    }

                    renderer.sortingOrder = mat.renderQueue - 2600 + sortingGroupOrder;
                    break;
                }

            }
        }
    }
}
