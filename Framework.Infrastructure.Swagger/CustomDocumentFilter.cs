using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common.Core;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.Infrastructure.Swagger
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public partial class HiddenSwaggerApiAttribute : Attribute { }

    public class CustomDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription apiDescription in context.ApiDescriptionsGroups.Items.SelectMany(e => e.Items))
            {
                var key = "/" + apiDescription.RelativePath.TrimEnd('/');
                if (key.ToLower().Contains(".inside.") || key.ToLower().Contains("/health") || apiDescription.ControllerAttributes().OfType<HiddenSwaggerApiAttribute>().Count() > 0)
                    swaggerDoc.Paths.Remove(key);
            }

            IDictionary<string, Schema> newDefinitions = new Dictionary<string, Schema>();
            swaggerDoc.Definitions.ToList().ForEach(k =>
            {
                var schema = k.Value;
                foreach (var property in schema.Properties)
                {
                    if (property.Value.Ref != null)
                    {
                        property.Value.Ref = property.Value.Ref.Replace("DTO", "Dto").ToUnderscoreCase().Replace("/_", "/");
                    }
                    if (property.Value.Items != null)
                    {
                        if (property.Value.Items.Ref != null)
                            property.Value.Items.Ref = property.Value.Items.Ref.Replace("DTO", "Dto").ToUnderscoreCase().Replace("/_", "/");
                    }
                }
                newDefinitions.Add(k.Key.Replace("DTO", "Dto").ToUnderscoreCase().TrimStart('_'), schema);
            });
            swaggerDoc.Definitions = newDefinitions;
            foreach (var path in swaggerDoc.Paths)
            {
                if (path.Value.Post != null)
                {
                    if (path.Value.Post.Parameters == null) continue;
                    SetParameter(path.Value.Post.Parameters);
                }
                if (path.Value.Put != null)
                {
                    if (path.Value.Put.Parameters == null) continue;
                    SetParameter(path.Value.Put.Parameters);
                }
                if (path.Value.Get != null)
                {
                    if (path.Value.Get.Parameters == null) continue;
                    SetParameter(path.Value.Get.Parameters);
                }
                if (path.Value.Delete != null)
                {
                    if (path.Value.Delete.Parameters == null) continue;
                    SetParameter(path.Value.Delete.Parameters);
                }
            }
        }

        private void SetParameter(IList<IParameter> parms)
        {
            foreach (var para in parms)
            {
                para.Name = para.Name.Replace("DTO", "Dto").ToUnderscoreCase().TrimStart('_');
                if (para is NonBodyParameter) continue;
                var p = para as BodyParameter;
                if (p.Schema.Type == "array")
                {
                    if (p.Schema.Items.Ref != null) p.Schema.Items.Ref = p.Schema.Items.Ref.Replace("DTO", "Dto").ToUnderscoreCase().Replace("/_", "/");
                    continue;
                }
                if (p.Schema != null && p.Schema.Ref != null) p.Schema.Ref = p.Schema.Ref.Replace("DTO", "Dto").ToUnderscoreCase().Replace("/_", "/");
            }
        }
    }
}