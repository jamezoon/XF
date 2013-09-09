using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using XFramework.Log;

namespace XFramework.Data
{
    /// <summary>
    /// XFramework绑定实体
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static class EntityBuilder
    {
        #region ReferencedTypeBindingInfo

        /// <summary>
        /// Contains data mapping info for a property that is referencing another type.
        /// </summary>
        private class ReferencedTypeBindingInfo
        {
            private ReferencedEntityAttribute m_ReferencedEntityAttribute;

            private PropertyInfo m_PropertyInfo;

            public ReferencedTypeBindingInfo(ReferencedEntityAttribute attri, PropertyInfo propertyInfo)
            {
                m_ReferencedEntityAttribute = attri;
                m_PropertyInfo = propertyInfo;
            }

            /// <summary>
            /// Get the type of the property
            /// </summary>
            public Type Type
            {
                get { return m_ReferencedEntityAttribute.Type; }
            }

            public string Prefix
            {
                get { return m_ReferencedEntityAttribute.Prefix; }
            }

            public string ConditionalProperty
            {
                get { return m_ReferencedEntityAttribute.ConditionalProperty; }
            }

            public PropertyInfo PropertyInfo
            {
                get { return m_PropertyInfo; }
            }
        }

        #endregion

        #region PropertyDataBindingInfo

        /// <summary>
        /// Contains data mapping info for a property in a type
        /// </summary>
        internal class PropertyDataBindingInfo
        {
            public PropertyInfo PropertyInfo { get; private set; }

            public DataMappingAttribute DataMapping { get; private set; }

            public PropertyDataBindingInfo(DataMappingAttribute mapping, PropertyInfo propertyInfo)
            {
                DataMapping = mapping;
                PropertyInfo = propertyInfo;
            }
        }

        #endregion

        #region fields

        private static readonly Type s_RootType = typeof(object);

        /// <summary>
        /// for each type, contains
        ///		string:							column name that could bound to a property
        ///		PropertyDataBindingInfo:		binding info
        /// </summary>
        private static Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>> _typeMappingInfo =
            new Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>>();

        /// <summary>
        /// for each type, contains:
        ///		a list of ReferencedTypeBindingInfo that the instance of this type refers to
        /// </summary>
        private static Dictionary<Type, List<ReferencedTypeBindingInfo>> _typeReferencedList =
            new Dictionary<Type, List<ReferencedTypeBindingInfo>>();

        /// <summary>
        /// for each type, contains
        ///		string:					property name
        ///		DataMappingAttribute:	data mapping attribute for this property
        /// </summary>
        private static Dictionary<Type, Dictionary<string, DataMappingAttribute>> _typePropertyInfo =
            new Dictionary<Type, Dictionary<string, DataMappingAttribute>>();

        private static Dictionary<Type, IMoneyCalculator> s_TypeMoneyCalculator =
            new Dictionary<Type, IMoneyCalculator>();

        private static object s_SyncMoneyCalculator = new object();

        private static readonly object _syncLock = new object();

        #endregion

        #region public static functions for building entity

        /// <summary>
        /// Builds the entity.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        public static T BuildEntity<T>(DataRow dr) where T : class, new()
        {
            return BuildEntity<T>(new DataRowEntitySource(dr), string.Empty);
        }

        /// <summary>
        /// Builds the entity list.
        /// Returns an empty list if the rows contains no data.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public static List<T> BuildEntityList<T>(DataRow[] rows) where T : class, new()
        {
            if (rows == null)
            {
                return new List<T>(0);
            }
            List<T> list = new List<T>(rows.Length);
            foreach (DataRow row in rows)
            {
                list.Add(BuildEntity<T>(row));
            }
            return list;
        }

        /// <summary>
        /// Builds the entity list.
        /// Returns an empty list if the rows contains no data.
        /// An exception will be thrown if failed to build the entity.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public static List<T> BuildEntityList<T>(DataTable table) where T : class, new()
        {
            if (table == null)
            {
                return new List<T>(0);
            }
            List<T> list = new List<T>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                list.Add(BuildEntity<T>(row));
            }
            return list;
        }

        #endregion

        #region private function

        private static T BuildEntity<T>(IEntityDataSource ds, string prefix) where T : class, new()
        {
            T obj = new T();
            FillEntity(ds, obj, typeof(T), prefix);
            return obj;
        }

        private static object BuildEntity(IEntityDataSource ds, Type type, string prefix)
        {
            object obj = Activator.CreateInstance(type);
            FillEntity(ds, obj, type, prefix);
            return obj;
        }

        private static void FillEntity(IEntityDataSource ds, object obj, Type type, string prefix)
        {
            Type baseType = type.BaseType;
            if (!s_RootType.Equals(baseType))
            {
                FillEntity(ds, obj, baseType, prefix);
            }
            DoFillEntity(ds, obj, type, prefix);
        }

        private static void DoFillEntity(IEntityDataSource ds, object obj, Type type, string prefix)
        {
            // fill properties
            foreach (string columnName in ds)
            {
                string mappingName;
                mappingName = columnName.ToUpper();
                if (!String.IsNullOrEmpty(prefix))
                {
                    if (mappingName.StartsWith(prefix.ToUpper()))
                    {
                        mappingName = mappingName.Substring(prefix.Length);
                    }
                }

                if (String.IsNullOrEmpty(prefix))
                {
                    prefix = string.Empty;
                }

                PropertyDataBindingInfo propertyBindingInfo = GetPropertyInfo(type, mappingName);
                if (propertyBindingInfo == null || columnName.ToUpper() != (prefix.ToUpper() + mappingName))
                {
                    continue;
                }

                if (ds[columnName] != DBNull.Value && ValidateData(propertyBindingInfo, ds[columnName]))
                {
                    propertyBindingInfo.PropertyInfo.SetValue(obj, GetPropertyValue(propertyBindingInfo, ds[columnName], type), null);
                }
            }

            // fill referenced objects
            List<ReferencedTypeBindingInfo> refList = GetReferenceObjects(type);

            foreach (ReferencedTypeBindingInfo refObj in refList)
            {
                if (TryFill(ds, refObj))
                {
                    refObj.PropertyInfo.SetValue(obj, BuildEntity(ds, refObj.Type, refObj.Prefix), null);
                }
            }
        }

        private static object GetPropertyValue(PropertyDataBindingInfo propertyBindingInfo, object val, Type type)
        {
            if (propertyBindingInfo.PropertyInfo.PropertyType == typeof(string))
                return val.ToString().Trim();

            // check the rate
            if (propertyBindingInfo.PropertyInfo.PropertyType == typeof(decimal) && propertyBindingInfo.DataMapping.CaculatorType != null)
            {
                IMoneyCalculator cal = GetMoneyCalculator(propertyBindingInfo.DataMapping.CaculatorType);

                if (cal != null)
                {
                    val = cal.Calculate((decimal)val);
                }
                else
                {
                    LogUtil.Log(
                        "XFramework.DB实例对象Mapping时异常",
                        string.Format("Mapping对象:{0}；属性名称：{1}；计算类型：{2}", type.FullName, propertyBindingInfo.PropertyInfo.Name, propertyBindingInfo.DataMapping.CaculatorType.FullName));
                }
            }
            return val;
        }

        private static IMoneyCalculator GetMoneyCalculator(Type calcalatorType)
        {
            IMoneyCalculator cal;
            s_TypeMoneyCalculator.TryGetValue(calcalatorType, out cal);
            if (cal == null)
            {
                lock (s_SyncMoneyCalculator)
                {
                    s_TypeMoneyCalculator.TryGetValue(calcalatorType, out cal);
                    if (cal == null)
                    {
                        cal = Activator.CreateInstance(calcalatorType) as IMoneyCalculator;
                        s_TypeMoneyCalculator[calcalatorType] = cal;
                    }
                }
            }
            return cal;
        }

        private static bool TryFill(IEntityDataSource ds, ReferencedTypeBindingInfo refObj)
        {
            if (string.IsNullOrEmpty(refObj.ConditionalProperty))
            {
                return true;
            }
            string columnName = GetBindingColumnName(refObj.Type, refObj.ConditionalProperty, refObj.Prefix);
            if (columnName == null)
            {
                return false;
            }
            return ds.ContainsColumn(columnName);
        }

        private static string GetBindingColumnName(Type type, string propertyName, string prefix)
        {
            Dictionary<string, DataMappingAttribute> propertyInfos;
            string name = null;
            try
            {
                _typePropertyInfo.TryGetValue(type, out propertyInfos);
                if (propertyInfos == null)
                {
                    lock (_syncLock)
                    {
                        _typePropertyInfo.TryGetValue(type, out propertyInfos);
                        if (propertyInfos == null)
                        {
                            AddTypeInfo(type);
                            propertyInfos = _typePropertyInfo[type];
                        }
                    }
                }
                DataMappingAttribute mapping;
                propertyInfos.TryGetValue(propertyName, out mapping);
                if (mapping == null)
                {
                    name = null;
                }
                else
                {
                    name = mapping.ColumnName;
                }
            }
            catch
            {
                name = null;
            }

            if (name == null)
            {
                if (!s_RootType.Equals(type.BaseType) && !s_RootType.Equals(type))
                {
                    return GetBindingColumnName(type.BaseType, propertyName, prefix);
                }
                else
                {
                    return null;
                }
            }
            return prefix + name;
        }

        /// <summary>
        /// Validate data binding info.
        /// Note: type checking is skipped here.
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="dbValue"></param>
        /// <returns></returns>
        private static bool ValidateData(PropertyDataBindingInfo bindingInfo, object value)
        {
            return true;
        }

        /// <summary>
        /// Get the property binding info.
        /// Returns null if no relevant binding info is found.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static PropertyDataBindingInfo GetPropertyInfo(Type type, string columnName)
        {
            Dictionary<string, PropertyDataBindingInfo> propertyInfoList;
            try
            {
                _typeMappingInfo.TryGetValue(type, out propertyInfoList);
                if (propertyInfoList == null)
                {
                    lock (_syncLock)
                    {
                        _typeMappingInfo.TryGetValue(type, out propertyInfoList);
                        if (propertyInfoList == null)
                        {
                            AddTypeInfo(type);
                            propertyInfoList = _typeMappingInfo[type];
                        }
                    }
                }
            }
            catch
            {
                // EntityBuilderLogger.LogGetPropertyBindingInfoException(type, columnName, e);
                return null;
            }

            PropertyDataBindingInfo info;
            propertyInfoList.TryGetValue(columnName, out info);
            return info;
        }

        private static List<ReferencedTypeBindingInfo> GetReferenceObjects(Type type)
        {
            List<ReferencedTypeBindingInfo> list;
            _typeReferencedList.TryGetValue(type, out list);
            if (list == null)
            {
                lock (_syncLock)
                {
                    _typeReferencedList.TryGetValue(type, out list);
                    if (list == null)
                    {
                        AddTypeInfo(type);
                        list = _typeReferencedList[type];
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// If no relevant properties exist, an empty hashtable and list is returned.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataMappingInfos"></param>
        /// <param name="referObjs"></param>
        private static void GetTypeInfo(Type type, out Dictionary<string, PropertyDataBindingInfo> dataMappingInfos,
            out List<ReferencedTypeBindingInfo> referObjs,
            out Dictionary<string, DataMappingAttribute> propertyInfos)
        {
            PropertyInfo[] propertyList = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            dataMappingInfos = new Dictionary<string, PropertyDataBindingInfo>();
            referObjs = new List<ReferencedTypeBindingInfo>();
            //propertyInfos = new Dictionary<string, DataMappingAttribute>(new CaseInsensitiveStringEqualityComparer());

            propertyInfos = new Dictionary<string, DataMappingAttribute>(new CaseInsensitiveStringEqualityComparer());

            foreach (PropertyInfo propertyInfo in propertyList)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    // properties binding to a data column
                    if (attribute is DataMappingAttribute)
                    {
                        DataMappingAttribute obj = attribute as DataMappingAttribute;
                        dataMappingInfos[obj.ColumnName.ToUpper()] = new PropertyDataBindingInfo(obj, propertyInfo);

                        propertyInfos.Add(propertyInfo.Name, obj);
                        continue;
                    }

                    // properties that are referenced objects
                    if (attribute is ReferencedEntityAttribute)
                    {
                        ReferencedEntityAttribute obj = attribute as ReferencedEntityAttribute;
                        referObjs.Add(new ReferencedTypeBindingInfo(obj, propertyInfo));
                    }
                }
            }
        }

        private static void AddTypeInfo(Type type)
        {
            // EntityBuilderLogger.LogAddTypeInfo(type);
            Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>> newMappingList =
                new Dictionary<Type, Dictionary<string, PropertyDataBindingInfo>>(_typeMappingInfo);
            Dictionary<Type, List<ReferencedTypeBindingInfo>> newReferencedObjects =
                new Dictionary<Type, List<ReferencedTypeBindingInfo>>(_typeReferencedList);
            Dictionary<Type, Dictionary<string, DataMappingAttribute>> newPropertyList =
                new Dictionary<Type, Dictionary<string, DataMappingAttribute>>(_typePropertyInfo);

            Dictionary<string, PropertyDataBindingInfo> mappingInfos;
            List<ReferencedTypeBindingInfo> referObjs;
            Dictionary<string, DataMappingAttribute> propertyInfos;
            GetTypeInfo(type, out mappingInfos, out referObjs, out propertyInfos);

            newMappingList[type] = mappingInfos;
            newReferencedObjects[type] = referObjs;
            newPropertyList[type] = propertyInfos;

            _typeMappingInfo = newMappingList;
            _typeReferencedList = newReferencedObjects;
            _typePropertyInfo = newPropertyList;
        }

        #endregion
    }

    public interface IMoneyCalculator
    {
        decimal Calculate(decimal money);
    }

    public class CaseInsensitiveStringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return (string.Compare(x, y, true) == 0);
        }

        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }
    }
}