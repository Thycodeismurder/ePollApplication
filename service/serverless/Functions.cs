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
    private async Task<string> QueryPollsAsync()
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        var response = await client.QueryAsync(new QueryRequest
        {
            TableName = "Polls",
            KeyConditionExpression = "Id = :v_Id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_Id", new AttributeValue { N = "0" } } }
        });
        List<Document> document = new List<Document>();
        foreach (var item in response.Items)
        {
            var documentToAdd = Document.FromAttributeMap(item);
            document.Add(documentToAdd);
        }
        return document.ToJsonPretty();
    }
    private async Task<string> QueryPollByIdAsync(string id)
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        var response = await client.QueryAsync(new QueryRequest
        {
            TableName = "Polls",
            KeyConditionExpression = "Id = :v_Id and PollId = :v_PollId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_Id", new AttributeValue { N = "0" } }, { ":v_PollId", new AttributeValue { N = id } } }
        });
        List<Document> document = new List<Document>();
        foreach (var item in response.Items)
        {
            var documentToAdd = Document.FromAttributeMap(item);
            document.Add(documentToAdd);
        }
        return document.ToJsonPretty();
    }

    private async Task<string> PostOptionAsync(string id, string optionId)
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        var response = await client.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = "Polls",
            Key = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { N = "0" } }, { "PollId", new AttributeValue { N = id } } },
            UpdateExpression = "Add Options[" + optionId + "].Votes :inc",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":inc", new AttributeValue { N = "1" } } }
        });
        Document document = Document.FromAttributeMap(response.Attributes);
        return document.ToJsonPretty();
    }
    private async Task<string> CreatePollAsync()
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        string tableName = "Polls";
        var request = new PutItemRequest
        {
            TableName = tableName,
            Item = new Dictionary<string, AttributeValue>() {
                {"PollId", new AttributeValue {N= "2"}},
                {"Id", new AttributeValue { N = "0"}},
                {"Title", new AttributeValue { S = "Paras kysymys"}},
                {"Options", new AttributeValue { L = new List<AttributeValue>() {
                    new AttributeValue {M = new Dictionary<string, AttributeValue>() {{"Id", new AttributeValue {N= "0"}}} },
                    new AttributeValue {M = new Dictionary<string, AttributeValue>() {{"Title", new AttributeValue {S= "hiphei"}}} },
                    new AttributeValue {M = new Dictionary<string, AttributeValue>() {{"Votes", new AttributeValue {N= "0"}}} }
                }   }},
            }
        };
        var response = await client.PutItemAsync(request);
        Document document = Document.FromAttributeMap(response.Attributes);
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
            Body = await QueryPollsAsync(),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> GetById(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("GetById Request\n");
        var id = request.PathParameters.ToList()[0].Value;
        context.Logger.LogInformation(id.ToString());

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await QueryPollByIdAsync(id),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> PostOption(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Post option Request\n");

        var pathParameters = request.PathParameters.ToList();
        var id = pathParameters[0].Value;
        var optionId = pathParameters[1].Value;

        context.Logger.LogInformation(id + optionId);
        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await PostOptionAsync(id, optionId),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> CreatePoll(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("create Poll Request\n");

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await CreatePollAsync(),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}