using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ePollApplication;

public class Functions
{
  /// <summary>
  /// Default constructor that Lambda will invoke.
  /// </summary>
  public Functions()
  {
  }


  /// <summary>
  /// A Lambda function to respond to HTTP Get methods from API Gateway
  /// </summary>
  /// <param name="request"></param>
  /// <returns>The API Gateway response.</returns>
  public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogInformation("Get Request\n");

    var response = new APIGatewayProxyResponse
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = "Hello AWS Serverless",
      Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
    };

    return response;
  }
  public APIGatewayProxyResponse GetById(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogInformation("GetById Request\n");
    var pathParameters = request.PathParameters.Values;
    context.Logger.LogInformation(pathParameters.ToString());

    var response = new APIGatewayProxyResponse
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = "Get by Id endpoint" + pathParameters.ToString(),
      Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
    };

    return response;
  }
  public APIGatewayProxyResponse PostOption(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogInformation("Get Request\n");

    var response = new APIGatewayProxyResponse
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = "PostOption endpoint",
      Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
    };

    return response;
  }
  public APIGatewayProxyResponse CreatePoll(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogInformation("Get Request\n");

    var response = new APIGatewayProxyResponse
    {
      StatusCode = (int)HttpStatusCode.OK,
      Body = "CreatePoll endpoint",
      Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
    };

    return response;
  }
}