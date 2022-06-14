/*************************
 * 
 * 其他一些混杂的工具类
 * 
 * 
 **************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace XLibrary
{
    public static class XMiscTools
    {
        /// <summary>
        /// 把内容弄到剪贴板
        /// </summary>
        /// <param name="input"></param>
        public static void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
        CopyTextToClipboard_iOS(input);  
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass tool = new AndroidJavaClass("com.my.ugcf.Tool");
        tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif

        }

        /// <summary>
        /// 对象深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T src) where T : UnityEngine.Object
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bf.Serialize(ms, src);

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            object dst = bf.Deserialize(ms);
            ms.Close();
            return dst as T;
        }

        /// <summary>
        /// 对象拷贝
        /// </summary>
        /// <param name="obj">被复制对象</param>
        /// <returns>新对象</returns>
        public static object CopyOjbect(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            System.Object targetDeepCopyObj;
            Type targetType = obj.GetType();
            //值类型  
            if (targetType.IsValueType == true)
            {
                targetDeepCopyObj = obj;
            }
            //引用类型   
            else
            {
                targetDeepCopyObj = Activator.CreateInstance(targetType);   //创建引用对象   
                System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();

                foreach (System.Reflection.MemberInfo member in memberCollection)
                {
                    //拷贝字段
                    if (member.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                        System.Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, CopyOjbect(fieldValue));
                        }

                    }//拷贝属性
                    else if (member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;

                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            try
                            {
                                object propertyValue = myProperty.GetValue(obj, null);
                                if (propertyValue is ICloneable)
                                {
                                    myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                                }
                                else
                                {
                                    myProperty.SetValue(targetDeepCopyObj, CopyOjbect(propertyValue), null);
                                }
                            }
                            catch (System.Exception)
                            {

                            }
                        }
                    }
                }
            }
            return targetDeepCopyObj;
        }
    }
}