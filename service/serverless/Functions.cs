using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ePollApplication;

public class Functions
{
    public Functions()
    {

    }
    private async Task<string> QueryPollsAsync()
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        var response = await client.QueryAsync(new QueryRequest
        {
            TableName = "polls",
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
            TableName = "polls",
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
            TableName = "polls",
            Key = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { N = "0" } }, { "PollId", new AttributeValue { S = id } } },
            UpdateExpression = "Add Options[" + optionId + "].votes :inc",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":inc", new AttributeValue { N = "1" } } }
        });
        Document document = Document.FromAttributeMap(response.Attributes);
        return document.ToJsonPretty();
    }
    private async Task<string> CreatePollAsync(Dictionary<string, AttributeValue> itemAsAttributes)
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        string tableName = "polls";
        var request = new PutItemRequest
        {
            TableName = tableName,
            Item = itemAsAttributes
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
            Headers = getHeaders()
        };

        return response;
    }
    public Dictionary<string, string> getHeaders()
    {
        var headers = new Dictionary<string, string> { { "Content-Type", "*" }, { "Access-Control-Allow-Headers", "Content-Type" }, { "Access-Control-Allow-Origin", "*" }, { "Access-Control-Allow-Methods", "*" } };
        return headers;
    }
    public async Task<APIGatewayProxyResponse> GetById(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("GetById Request\n");
        var id = request.PathParameters.ToList()[0].Value;
        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await QueryPollByIdAsync(id),
            Headers = getHeaders()
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> PostOption(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Post option Request\n");
        var pathParameters = request.PathParameters.ToList();
        var id = pathParameters[0].Value;
        var optionId = pathParameters[1].Value;
        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await PostOptionAsync(id, optionId),
            Headers = getHeaders()
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> CreatePoll(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var poll = Document.FromJson(request.Body).ToAttributeMap();
        poll.Remove("PollId");
        poll.Add("PollId", new AttributeValue { S = Guid.NewGuid().ToString() });
        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = await CreatePollAsync(poll!),
            Headers = getHeaders()
        };
        return response;
    }
}