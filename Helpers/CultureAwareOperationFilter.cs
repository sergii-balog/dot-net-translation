using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace dotNetWebApiTranslation.Helpers;

public class AcceptLanguageOperationFilter: IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    if (operation.Parameters == null)
    {
      operation.Parameters = new List<OpenApiParameter>();
    }

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "Accept-Language",
      @In = ParameterLocation.Header,
      Required = true,
      Schema = new OpenApiSchema
      {
        Type = "string",
        Default = new OpenApiString("en"),
        Enum = new List<IOpenApiAny>
        {
          new OpenApiString("en"),
          new OpenApiString("fr"),
          new OpenApiString("uk")
        }
      }
    });
  }
}
