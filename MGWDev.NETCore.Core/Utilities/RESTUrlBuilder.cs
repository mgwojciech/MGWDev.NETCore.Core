﻿using MGWDev.NETCore.Core.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace MGWDev.NETCore.Core.Utilities
{
    public class RESTUrlBuilder<T> : IUrlBuilder<T>
    {
        ExpressionToRESTMapper<T> ExpressionMapper { get; set; } = new ExpressionToRESTMapper<T>();
        public string BuildFilterClause(Expression<Func<T, bool>> query)
        {
            return ExpressionMapper.Translate(query.Body);
        }

        public string BuildIdQuery<U>(string baseUrl, U id)
        {
            if (typeof(U).FullName == typeof(Guid).FullName)
                return String.Format("{0}(guid'{1}')", baseUrl, id);
            return String.Format("{0}({1})", baseUrl, id);
        }

        public static List<string> GetSimplePropertiesNames(Type type)
        {
            List<string> mappedPropertiesNames = new List<string>();
            var mappingProperties = type.GetProperties().Where(mp => mp.GetSetMethod() != null);


            if (type.GetInterface("IEnumerable") != null)
            {
                var childProperties = GetSimplePropertiesNames(type.GenericTypeArguments.FirstOrDefault());
                childProperties.ForEach(delegate (string propName)
                {
                    mappedPropertiesNames.Add(propName);
                });

                return mappedPropertiesNames;
            }

            foreach (var mappedProperty in mappingProperties)
            {
                if (PropertyHelper.IsSimpleType(mappedProperty.PropertyType))
                    mappedPropertiesNames.Add(mappedProperty.Name);
                else
                {
                    var childProperties = GetSimplePropertiesNames(mappedProperty.PropertyType);
                    childProperties.ForEach(delegate (string propName)
                    {
                        mappedPropertiesNames.Add(String.Format("{0}/{1}", mappedProperty.Name, propName));
                    });
                }
            }
            return mappedPropertiesNames;
        }

        public static List<string> GetComplexPropertiesNames(Type type)
        {
            List<string> mappedPropertiesNames = new List<string>();
            var mappingProperties = type.GetProperties().Where(mp => mp.GetSetMethod() != null);
            foreach (var mappedProperty in mappingProperties)
            {
                if (!PropertyHelper.IsSimpleType(mappedProperty.PropertyType))
                    mappedPropertiesNames.Add(mappedProperty.Name);
            }
            return mappedPropertiesNames;
        }
        public string BuildSelect()
        {
            return String.Join(",", GetSimplePropertiesNames(typeof(T)));
        }

        public string BuildSkip(int skip)
        {
            return skip.ToString();
        }

        public string BuildTop(int top)
        {
            return top.ToString();
        }
        public string BuildExpand()
        {
            return String.Join(",", GetComplexPropertiesNames(typeof(T)));
        }
    }
}
