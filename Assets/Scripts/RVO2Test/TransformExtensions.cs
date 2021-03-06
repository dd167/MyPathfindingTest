﻿using System.Collections.Generic;
using UnityEngine;

namespace Starry
{
    // Unity Transform的扩展功能
    public static partial class TransformExtensions
    {
        /// <summary>
        /// 广度优先搜索遍历
        /// </summary>
        /// <param name="root"></param>
        /// <typeparam name="TP">遍历时调用的函数的参数的类型</typeparam>
        /// <typeparam name="TR">遍历时调用的函数的返回值的类型</typeparam>
        /// <param name="visitFunc">遍历时调用的函数
        /// <para>TR Function(Transform t, T para)</para>
        /// </param>
        /// <param name="para">遍历时调用的函数的第二个参数</param>
        /// <param name="failReturnValue">遍历时查找失败的返回值</param>
        /// <returns>遍历时调用的函数的返回值</returns>
        public static TR BFSVisit<TP, TR>(this Transform root, System.Func<Transform, TP, TR> visitFunc, TP para, TR failReturnValue = default(TR))
        {
            TR ret = visitFunc(root, para);
            if (ret != null && !ret.Equals(failReturnValue))
                return ret;
            Queue<Transform> parents = new Queue<Transform>();
            parents.Enqueue(root);
            while (parents.Count > 0)
            {
                Transform parent = parents.Dequeue();
                foreach (Transform child in parent)
                {
                    ret = visitFunc(child, para);
                    if (ret != null && !ret.Equals(failReturnValue))
                        return ret;
                    parents.Enqueue(child);
                }
            }
            return failReturnValue;
        }

        /// <summary>
        /// 深度优先搜索遍历
        /// </summary>
        /// <param name="root"></param>
        /// <typeparam name="TP">遍历时调用的函数的参数的类型</typeparam>
        /// <typeparam name="TR">遍历时调用的函数的返回值的类型</typeparam>
        /// <param name="visitFunc">遍历时调用的函数
        /// <para>TR Function(Transform t, T para)</para>
        /// </param>
        /// <param name="para">遍历时调用的函数的第二个参数</param>
        /// <param name="failReturnValue">遍历时查找失败的返回值</param>
        /// <returns>遍历时调用的函数的返回值</returns>
        public static TR DFSVisit<TP, TR>(this Transform root, System.Func<Transform, TP, TR> visitFunc, TP para, TR failReturnValue = default(TR))
        {
            Stack<Transform> parents = new Stack<Transform>();
            parents.Push(root);
            while (parents.Count > 0)
            {
                Transform parent = parents.Pop();
                TR ret = visitFunc(parent, para);
                if (ret != null && !ret.Equals(failReturnValue))
                    return ret;
                for (int i = parent.childCount - 1; i >= 0; i--)
                {
                    parents.Push(parent.GetChild(i));
                }
            }
            return failReturnValue;
        }

        /// <summary>
        /// 根据名字查找并返回子孙，广度优先搜索
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="childName">要查找的子孙的名字</param>
        /// <returns>要查找的子孙的Transform</returns>
        public static T FindComponent_BFS<T>(this Transform trans, string childName) where T : Component
        {
            var target = BFSVisit<string, Transform>(trans,
                (t, str) => { if (t.name.Equals(str)) return t; return null; },
                childName
            );

            if (target == null)
            {
                Debug.LogError(string.Format("cann't find child transform {0} in {1}", childName, trans.gameObject.name));
                return null;
            }

            T component = target.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError("Component is null, type = " + typeof(T).Name);
                return null;
            }
            return component;
        }

        /// <summary>
        /// 根据名字查找并返回子孙，广度优先搜索
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="tagName">要查找的子孙的名字</param>
        /// <returns>要查找的子孙的Transform</returns>
        public static Transform FindChild_ByTag(this Transform trans, string tagName)
        {
            var target = BFSVisit<string, Transform>(trans,
                (t, str) => { if (t.tag.Equals(str)) return t; return null; },
                tagName
            );

            if (target == null)
            {
                //Debug.LogError(string.Format("cann't find child transform {0} in {1}", tagName, trans.gameObject.name));
                return null;
            }

            return target;
        }

        /// <summary>
        /// 根据名字查找并返回子孙，深度优先搜索
        /// </summary>
        /// /// <param name="trans"></param>
        /// <param name="childName">要查找的子孙的名字</param>
        /// <returns>要查找的子孙的Transform</returns>
        public static T FindComponent_DFS<T>(this Transform trans, string childName) where T : Component
        {
            var target = DFSVisit<string, Transform>(trans,
                (t, str) => { if (t.name.Equals(str)) return t; return null; },
                childName
            );

            if (target == null)
            {
                Debug.LogError(string.Format("cann't find child transform {0} in {1}", childName, trans.gameObject.name));
                return null;
            }

            T component = target.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError("Component is null, type = " + typeof(T).Name);
                return null;
            }
            return component;
        }

        /// <summary>
        /// 根据名字在子对象中查找组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <param name="name"></param>
        /// /// <param name="reportError"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this Transform trans, string name, bool reportError = true) where T : Component
        {
            Transform target = trans.Find(name);
            if (target == null)
            {
                if (reportError)
                {
                    Debug.LogError("Transform is null, name = " + name);
                }

                return null;
            }

            T component = target.GetComponent<T>();
            if (component == null)
            {
                if (reportError)
                {
                    Debug.LogError("Component is null, type = " + typeof(T).Name);
                }

                return null;
            }

            return component;
        }

        /// <summary>
        /// 初始化物体的相对位置、旋转、缩放
        /// </summary>
        /// <param name="trans"></param>
        public static void InitTransformLocal(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 直接设置物体x轴的世界坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        public static void SetPositionX(this Transform trans, float x)
        {
            var position = trans.position;
            position = new Vector3(x, position.y, position.z);
            trans.position = position;
        }

        /// <summary>
        /// 直接设置物体y轴的世界坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="y"></param>
        public static void SetPositionY(this Transform trans, float y)
        {
            var position = trans.position;
            position = new Vector3(position.x, y, position.z);
            trans.position = position;
        }

        /// <summary>
        /// 直接设置物体z轴的世界坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="z"></param>
        public static void SetPositionZ(this Transform trans, float z)
        {
            var position = trans.position;
            position = new Vector3(position.x, position.y, z);
            trans.position = position;
        }

        /// <summary>
        /// 直接设置物体x轴的本地坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        public static void SetLocalPositionX(this Transform trans, float x)
        {
            var localPosition = trans.localPosition;
            localPosition = new Vector3(x, localPosition.y, localPosition.z);
            trans.localPosition = localPosition;
        }

        /// <summary>
        /// 直接设置物体y轴的本地坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="y"></param>
        public static void SetLocalPositionY(this Transform trans, float y)
        {
            var localPosition = trans.localPosition;
            localPosition = new Vector3(localPosition.x, y, localPosition.z);
            trans.localPosition = localPosition;
        }

        /// <summary>
        /// 直接设置物体z轴的本地坐标
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="z"></param>
        public static void SetLocalPositionZ(this Transform trans, float z)
        {
            var localPosition = trans.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, z);
            trans.localPosition = localPosition;
        }

        /// <summary>
        /// 可以递归地查找所有子节点的某个T类型的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <param name="recursive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T[] FindComponentsInChildren<T>(this Transform transform, bool recursive = true, bool includeInactive = true) where T : Component
        {

            if (recursive)
            {
                var list = new List<T>();
                GetChildren(transform, includeInactive, ref list);
                return list.ToArray();
            }
            else
            {
                return transform.GetComponentsInChildren<T>(includeInactive);
            }
        }

        public static T GetComponentsInParent<T>(this Transform transform) where T : Component
        {
            if (transform == null)
            {
                return null;
            }
            if (transform.GetComponent<T>() != null)
            {
                return transform.GetComponent<T>();
            }
            if (transform.parent != null)
            {
                return transform.parent.GetComponentInParent<T>();
            }
            return null;
        }

        public static Transform GetChildByName(this Transform transform, string name, bool recursive = true, bool includeInactive = true)
        {
            Transform target;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null && (includeInactive || transform.gameObject.activeSelf))
                {
                    if (child.name == name)
                    {
                        return child;
                    }
                    else
                    {
                        target = child.GetChildByName(name);
                        if (target != null) return target;
                    }
                }
            }

            return null;
        }

        private static void GetChildren<T>(Transform t, bool includeInactive, ref System.Collections.Generic.List<T> list)
        {
            if (includeInactive || t.gameObject.activeSelf)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    if (t.GetChild(i) != null)
                    {
                        GetChildren(t.GetChild(i), includeInactive, ref list);
                    }
                }

                var comp = t.GetComponent<T>();
                if (comp != null)
                {
                    list.Add(comp);
                }
            }

        }
    }
}