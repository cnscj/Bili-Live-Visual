using System;
using System.Collections.Generic;

namespace THGame.UI
{
    public class RedDotNode<T> where T : new()
    {
        public string name;
        public RedDotNode<T> parent;
        public Dictionary<string, RedDotNode<T>> children;
        public T data = new T();
        public RedDotCallback<T> callback;

        public void RemoveFromParent()
        {
            if (parent != null && parent.children != null)
            {
                parent.children.Remove(name);
            }
        }

        public RedDotNode<T> Find(params string[] args)
        {
            return Visit(false, args);
        }

        public RedDotNode<T> Create(params string[] args)
        {
            return Visit(true, args);
        }
        
        public bool Delete(params string[] args)
        {
            var node = Visit(false, args);
            if (node == null)
                return false;

            var nodeParent = node.parent;
            nodeParent.children?.Remove(node.name);

            return true;
        }

        public void TraverseTop(Action<RedDotNode<T>> action)
        {
            var node = this;
            while (node != null)
            {
                action?.Invoke(node);
                node = node.parent;
            }
        }

        public void TraverseBottom(Action<RedDotNode<T>> action, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            var node = this;
            foreach (var nodeName in args)
            {
                if (string.IsNullOrEmpty(nodeName))
                    return;

                if (node.children == null)
                    return;

                node.children.TryGetValue(nodeName, out node);

                if (node == null)
                    return;

                action?.Invoke(node);

            }
        }

        ///
        private RedDotNode<T> Visit(bool isCreate, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return null;

            var node = this;
            foreach (var nodeName in args)
            {
                if (string.IsNullOrEmpty(nodeName))
                    return null;

                if (isCreate)
                {
                    node.children = node.children ?? new Dictionary<string, RedDotNode<T>>();

                    if (!node.children.ContainsKey(nodeName))
                    {
                        //信息附加
                        var newNode = new RedDotNode<T>();
                        newNode.name = nodeName;
                        newNode.parent = node;

                        node.children[nodeName] = newNode;
                    }
                    node.children.TryGetValue(nodeName, out node);
                }
                else
                {
                    if (node == null)
                        return null;

                    if (node.children == null)
                        return null;

                    node.children.TryGetValue(nodeName, out node);
                }
            }
            return node;
        }

    }
}
