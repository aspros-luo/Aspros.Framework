using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Framework.Infrastructure.Swagger
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomSwaggerResponseAttribute : Attribute, IFilterMetadata
    {
        public CustomSwaggerResponseAttribute()
        {
            this.IsSubmit = true;
        }

        public CustomSwaggerResponseAttribute(string dataName, Type type, bool isSubmit = false, bool isPage = false, bool isList = false)
        {
            if (!string.IsNullOrEmpty(dataName) && type != null)
            {
                this.Datas.Add(dataName, type);
            }
            this.IsPage = isPage;
            this.IsSubmit = isSubmit;
            this.IsList = isList;

            if (isPage)
            {
                this.IsList = true;
            }
        }

        public CustomSwaggerResponseAttribute(string dataName1, string dataName2, Type type, bool isSubmit = false, bool isPage = false, bool isList = false)
        {
            this.Datas.Add(dataName1, type);
            this.Datas.Add(dataName2, type);
            this.IsPage = isPage;
            this.IsSubmit = isSubmit;
            this.IsList = isList;

            if (isPage)
            {
                this.IsList = true;
            }
        }

        public CustomSwaggerResponseAttribute(string dataName1, string dataName2, Type type1, Type type2, bool isSubmit = false, bool isPage = false, bool isList = false)
        {
            this.Datas.Add(dataName1, type1);
            this.Datas.Add(dataName2, type2);
            this.IsPage = isPage;
            this.IsSubmit = isSubmit;
            this.IsList = isList;

            if (isPage)
            {
                this.IsList = true;
            }
        }

        public CustomSwaggerResponseAttribute(string dataName1, string dataName2, string dataName3, Type type1, Type type2, Type type3 = null, bool isSubmit = false, bool isPage = false, bool isList = false)
        {
            this.Datas.Add(dataName1, type1);
            this.Datas.Add(dataName2, type2);
            this.Datas.Add(dataName3, type3);
            this.IsPage = isPage;
            this.IsSubmit = isSubmit;
            this.IsList = isList;

            if (isPage)
            {
                this.IsList = true;
            }
        }

        public Dictionary<string, Type> Datas { get; set; } = new Dictionary<string, Type>();

        public bool IsPage { get; set; }

        public bool IsSubmit { get; set; }

        public bool IsList { get; set; }

        public string Description { get; set; } = "";
    }
}

