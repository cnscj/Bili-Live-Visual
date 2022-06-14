using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XLibrary
{
    public static class XGameObjectTools
    {
        /// <summary>
        /// 取得GameIbject的路径
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string GetPathByGameObject(GameObject obj, GameObject root = null)
        {
            string path = obj.name;
            while (obj.transform.parent != null && obj.transform.parent.gameObject != root)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// 根据路径取得GO
        /// </summary>
        /// <param name="go"></param>
        /// <param name="path"></param>
        /// <param name="isExceptLeafNode"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectByPath(GameObject go, string path, bool isExceptLeafNode = false)
        {
            string finalPath = (isExceptLeafNode) ? path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal)) : path;
            Transform retTransform = go.transform.Find(finalPath);
            if (retTransform != null)
            {
                return retTransform.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 遍历所有可用节点
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void Traverse(GameObject obj, Action<GameObject> action, bool includeinactive = false)
        {
            if (obj == null || action == null)
                return;

            foreach(var transform in obj.GetComponentsInChildren<Transform>(includeinactive))
            {
                action.Invoke(transform.gameObject);
            }
        }

        /// <summary>
        /// 深度遍历
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void TraverseDFS(GameObject obj, Action<GameObject> action , Func<GameObject,bool> excludeExp = null)
        {
            if (obj == null || action == null)
                return;

            Stack<GameObject> parentNodes = new Stack<GameObject>();
            Stack<GameObject> transNodes = new Stack<GameObject>();
            parentNodes.Push(obj);

            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Pop();
                transNodes.Push(curNode);

                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;
                    if (excludeExp != null && !excludeExp(itNode)) continue;

                    parentNodes.Push(itNode);
                }
            }

            while(transNodes.Count > 0)
            {
                var curNode = transNodes.Pop();
                action.Invoke(curNode);
            }

        }

        /// <summary>
        /// 广度遍历
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void TraverseBFS(GameObject obj, Action<GameObject> action, Func<GameObject, bool> excludeExp = null)
        {
            if (obj == null || action == null)
                return;

            Queue<GameObject> parentNodes = new Queue<GameObject>();
            parentNodes.Enqueue(obj);

            while (parentNodes.Count > 0)
            {
                var curNode = parentNodes.Dequeue();
                for (int i = 0; i < curNode.transform.childCount; i++)
                {
                    var itNode = curNode.transform.GetChild(i).gameObject;
                    if (curNode == itNode) continue;
                    if (excludeExp != null && !excludeExp(itNode)) continue;

                    parentNodes.Enqueue(itNode);
                }

                action.Invoke(curNode);
            }

        }

        /// <summary>
        /// 用一个GO填充另一个GO,如果dest没有,则在src中找
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="cndtFunc">填充条件</param>
        public static void UnionGameObject(GameObject dest, GameObject src, bool isReplace = true, Action<GameObject> action = null)
        {
            if (dest == null || src == null)
                return;

            TraverseBFS(src, (curNode) =>
            {
                if (curNode != src)
                {
                    string curNodePath = GetPathByGameObject(curNode, src);
                    GameObject destNode = GetGameObjectByPath(dest, curNodePath);
                    bool isNeedCreate = false;
                    if (destNode == null)   //目标没有这个节点
                    {
                        isNeedCreate = true;
                    }
                    else
                    {
                        if (isReplace)  //有这个节点,但是要替换
                        {
                            isNeedCreate = true;
                            UnityEngine.Object.Destroy(destNode);
                        }
                    }

                    if (isNeedCreate)
                    {
                        string destParentPath = Path.GetDirectoryName(curNodePath);
                        GameObject destParentNode = GetGameObjectByPath(dest, destParentPath);
                        if (destParentNode != null)
                        {
                            var newNode = UnityEngine.Object.Instantiate(curNode, destParentNode.transform, false);
                            newNode.name = curNode.name;
                            destNode = newNode;
                        }
                    }
                    action?.Invoke(destNode);
                }
            });
        }

        /// <summary>
        /// 替换GO,如果在src中找到对应node,则替换dest对应node
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        public static void RelpaceGameObject(GameObject dest, GameObject src)
        {
            if (dest == null || src == null)
                return;

            TraverseBFS(dest, (curNode) =>
            {
                if (curNode != dest)
                {
                    string curNodePath = GetPathByGameObject(curNode, dest);
                    GameObject srcNode = GetGameObjectByPath(src, curNodePath);

                    if (srcNode != null)
                    {
                        GameObject parentNode = curNode.transform.parent.gameObject;
                        UnityEngine.Object.Destroy(curNode);
                        var newNode = UnityEngine.Object.Instantiate(srcNode, parentNode.transform, false);
                        newNode.name = curNode.name;
                        curNode = newNode;
                    }
                }
            });
        }

        /// <summary>
        /// 合并2个GO,如果src中没有在src找到对应的,则移除
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="isReplace"></param>
        /// <param name="action"></param>
        public static void MergeGameObject(GameObject dest, GameObject src, bool isReplace = true, Action<GameObject> action = null)
        {
            if (dest == null || src == null)
                return;

            TraverseBFS(dest, (curNode) =>
            {
                if (curNode != dest)
                {
                    string curNodePath = GetPathByGameObject(curNode, dest);
                    GameObject srcNode = GetGameObjectByPath(src, curNodePath);

                    if (srcNode == null)
                    {
                        UnityEngine.Object.Destroy(curNode);
                    }
                    else
                    {
                        if(isReplace)
                        {
                            GameObject parentNode = curNode.transform.parent.gameObject;
                            UnityEngine.Object.Destroy(curNode);
                            var newNode = UnityEngine.Object.Instantiate(srcNode, parentNode.transform, false);
                            newNode.name = curNode.name;
                            curNode = newNode;
                        }
                        action?.Invoke(curNode);
                    }
                }
            });
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        public static void SetLayer(GameObject gameObject, int layer)
        {
            if (gameObject == null)
                return;

            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(true);
            if (transforms != null && transforms.Length > 0)
            {
                foreach (var transform in transforms)
                {
                    transform.gameObject.layer = layer;
                }
            }
        }

        public static void SetLayer(GameObject gameObject, GameObject parentGameObject = null)
        {
            if (gameObject == null)
                return;
            parentGameObject = parentGameObject ?? gameObject.transform.parent?.gameObject;

            if (parentGameObject == null)
                return;

            SetLayer(gameObject, parentGameObject.layer);
        }
       
    }

}
