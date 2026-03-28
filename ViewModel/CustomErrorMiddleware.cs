using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AngularWithNET.ViewModel
{
    public class CustomErrorMiddleware
	{
		private readonly RequestDelegate next;

		public CustomErrorMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context /* other dependencies */)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError; // 500 if unexpected
			Log.Error("Error Id : {errorId}, Request  : {request} , Query String : {query}", context.TraceIdentifier, context.Request.Method, context.Request.QueryString);
			Log.Error(exception, exception.Message);

			string result = JsonConvert.SerializeObject(new
			{
				error = "An error occurred in System.  Please refer to the System Developer. Error: " + exception.Message,
				id = context.TraceIdentifier
			});
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
		}
	}
}
