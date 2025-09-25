using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class UploadFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Áp dụng cho action Create của CVController
        if (context.ApiDescription.ActionDescriptor.RouteValues["controller"] == "CV"
            && context.ApiDescription.HttpMethod == "POST")
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["userId"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                ["fullCvJson"] = new OpenApiSchema { Type = "string" },
                                ["file"] = new OpenApiSchema { Type = "string", Format = "binary" }
                            },
                            Required = new HashSet<string> { "userId", "file" }
                        }
                    }
                },
                Required = true
            };

            operation.Parameters = null;
        }
    }
}