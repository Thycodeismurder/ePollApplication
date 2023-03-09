using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DocumentModel;

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
    private async Task<string> QueryPollsAsync(ILambdaContext context)
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        var response = await client.QueryAsync(new QueryRequest { TableName = "polls", KeyConditionExpression = "Id = :v_Id", ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_Id", new AttributeValue { N = "0" } } } });
        List<Document> document = new List<Document>();
        foreach (var item in response.Items)
        {
            context.Logger.LogInformation(Document.FromAttributeMap(item).ToJsonPretty());
            var documentToAdd = Document.FromAttributeMap(item);
            document.Add(documentToAdd);
            context.Logger.LogInformation("document is next");
            context.Logger.LogInformation(document.ToJsonPretty());
            //lista dokumenttei, mitkä pistetään stringiksi ja lähettetään toteuta
        }
        return document.ToJsonPretty();
    }

    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The API Gateway response.</returns>
    public async Task<APIGatewayProxyResponse> GetAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get Request\n");

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await QueryPollsAsync(context),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public APIGatewayProxyResponse GetById(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("GetById Request\n");
        var id = request.PathParameters.ToList()[0].Value;
        context.Logger.LogInformation(id.ToString());

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "Get by Id endpoint" + id.ToString(),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public APIGatewayProxyResponse PostOption(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Post option Request\n");

        var pathParameters = request.PathParameters.ToList();
        var id = pathParameters[0].Value;
        var optionId = pathParameters[1].Value;

        context.Logger.LogInformation(id + optionId);
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
        context.Logger.LogInformation("create Poll Request\n");

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "CreatePoll endpoint",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}