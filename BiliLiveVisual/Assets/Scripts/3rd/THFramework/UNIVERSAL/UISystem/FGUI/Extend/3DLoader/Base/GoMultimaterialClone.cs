using UnityEngine;
namespace THGame.UI
{
    //多材质克隆-// 多材质单渲染器变单材质单渲染器(仅模型)，注意：gameObject不要再用于实例化
    //如果GO含有SkinnedMeshRenderer并且Material是多个
    //如果Material的RendererQueue不是一样的话,
    //复制该含SMR组件节点(有多少个Material就复制多少个),并用一个空的Material占位
    public class GoMultimaterialClone : GoBaseUpdater
    {
        public override void OnReplace(GameObject oldGo,GameObject newGo)
        {

        }
    }

}
