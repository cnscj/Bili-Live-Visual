
using System;
using XLibrary.Package;

namespace THGame.UI
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        private RedDotNode<RedDotData> m_treeRoot;      //红点树根节点

        public RedDotManager()
        {
            
        }

        public RedDotNode<RedDotData> Register(RedDotCallback<RedDotData> callback, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return null;

            if (callback == null)
                return null;

            var node = GetTreeRoot().Create(args);

            node.callback += callback;
            node.callback?.Invoke(node.data);

            return node;
        }

        public void Unregister(RedDotCallback<RedDotData> callback, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            if (callback == null)
                return;

            var node = GetTreeRoot().Find(args);
            if (node != null)
            {
                node.callback -= callback;
                if(node.children.Count <= 0 && node.callback == null)
                {
                    node.RemoveFromParent();
                }
            }     
        }

        public void SetStatus(int status, params string[] args)
        {
            //只要子节点有一个亮,父节点就不能为Hide
            //如果子节点全为Hide,则父节点为Hide
            var node = GetTreeRoot().Create(args);

            var oldStatus = node.data.curStatus;

            if (status != RedDotStatus.Hide) node.data.lightNum++;
            else node.data.lightNum--;
            node.data.curStatus = status;
            node.data.willStatus = node.data.curStatus;
            node.callback?.Invoke(node.data);

            if (oldStatus == status)
                return;

            if (status != RedDotStatus.Hide)
            {
                node.TraverseTop((child) =>
                {
                    if (child == node)
                        return;

                    child.data.lightNum++;
                    child.data.curStatus = status;
                    child.callback?.Invoke(child.data);
                });
            }
            else
            {
                //如果子节点还有一个亮,就是亮,否则为恢复原状态
                node.TraverseTop((child) =>
                {
                    if (child == node)
                        return;

                    child.data.lightNum--;
                    child.data.lightNum = Math.Max(0, child.data.lightNum);
                    if (child.data.lightNum <= 0)
                    {
                        child.data.curStatus = child.data.willStatus;
                        child.callback?.Invoke(child.data);
                    }
                });
            }

        }

        public int GetStatus(params string[] args)
        {
            var node = GetTreeRoot().Find(args);
            if (node != null)
            {
                return node.data.curStatus;
            }

            return RedDotStatus.Hide;
        }

        private RedDotNode<RedDotData> GetTreeRoot()
        {
            m_treeRoot = m_treeRoot ?? new RedDotNode<RedDotData>();
            return m_treeRoot;
        }

    }
}
