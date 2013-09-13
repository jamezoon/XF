using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义列表结果类
    /// </summary>
    /// <typeparam name="T">列表中的元素</typeparam>
    [Serializable]
    public class ListData<T>
    {
        /// <summary>
        /// 记录集
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// 构造函数，实例化一个列表
        /// </summary>
        public ListData()
        {
            Data = new List<T>();
        }

        /// <summary>
        /// 搜索指定的对象，并返回整个列表中第一个匹配项从零开始的索引
        /// </summary>
        /// <param name="item">要在列表中查找的对象。对于引用类型，可以为null</param>
        /// <returns>整个列表中第一个匹配项从零开始的索引</returns>
        public int IndexOf(T item)
        {
            return Data.IndexOf(item);
        }

        /// <summary>
        /// 将元素插入指定的索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在此处插入item。</param>
        /// <param name="item">要插入的对象。对于引用类型，可以为null。</param>
        public void Insert(int index, T item)
        {
            Data.Insert(index, item);
        }

        /// <summary>
        /// 移除列表中指定索引出的元素
        /// </summary>
        /// <param name="index">要移除的元素从零开始的索引</param>
        public void RemoveAt(int index)
        {
            Data.RemoveAt(index);
        }

        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获取或设置元素从零开始的索引。</param>
        /// <returns>获取或设置指定索引处的元素。</returns>
        public T this[int index]
        {
            get
            {
                return Data[index];
            }

            set
            {
                Data[index] = value;
            }
        }

        /// <summary>
        /// 将对象添加到列表的结尾处。
        /// </summary>
        /// <param name="item">要添加到列表末尾处的对象。对应应用类型，可以为null。</param>
        public void Add(T item)
        {
            Data.Add(item);
        }

        /// <summary>
        /// 从列表出移除所有元素
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// 确定某元素是否在列表中。
        /// </summary>
        /// <param name="item">要在列表中查找的对象。对应应用类型，可以为null。</param>
        /// <returns>确定某元素是否在列表中</returns>
        public bool Contains(T item)
        {
            return Data.Contains(item);
        }

        /// <summary>
        /// 将整个列表复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
        /// </summary>
        /// <param name="array">作为从列表复制的元素的目标位置的一维System.Array。System.Array必须有从零开始的索引。</param>
        /// <param name="arrayIndex">arry中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取列表中实际包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return Data.Count;
            }
        }

        /// <summary>
        /// 从列表中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">需要删除的项</param>
        /// <returns>是否删除成功</returns>
        public bool Remove(T item)
        {
            return Data.Remove(item);
        }

        /// <summary>
        /// 返回循环访问列表的枚举器
        /// </summary>
        /// <returns>循环访问列表的枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// 列表的只读属性
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
}
