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
    private async Task<string> CreatePollAsync(Poll pollItems)
    {
        using var client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
        string tableName = "Polls";
        var pollOptionItems = new List<AttributeValue>();
        for (int i = 0; i < pollItems?.Options?.Count; i++)
        {
            pollOptionItems.Add(
                new AttributeValue { M = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { N = pollItems.Options[i]?.Id?["Id"] } } } }
            );
            pollOptionItems.Add(
                new AttributeValue { M = new Dictionary<string, AttributeValue>() { { "Title", new AttributeValue { N = pollItems.Options[i]?.Title?["Title"] } } } }
            );
            pollOptionItems.Add(
                new AttributeValue { M = new Dictionary<string, AttributeValue>() { { "Votes", new AttributeValue { N = pollItems.Options[i]?.Votes?["Votes"] } } } }
            );
        }
        var request = new PutItemRequest
        {
            TableName = tableName,
            Item = new Dictionary<string, AttributeValue>() {
                {"PollId", new AttributeValue {N= pollItems?["PollId"]}},
                {"Id", new AttributeValue { N = "0"}},
                {"Title", new AttributeValue { S = pollItems?["Title"]}},
                {"Options", new AttributeValue { L = pollOptionItems   }},
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
            Headers = getHeaders()
        };

        return response;
    }
    public Dictionary<string, string> getHeaders()
    {
        var headers = new Dictionary<string, string> { { "Content-Type", "text/plain" }, { "Access-Control-Allow-Headers", "Content-Type" }, { "Access-Control-Allow-Origin", "*" }, { "Access-Control-Allow-Methods", "*" } };
        return headers;
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

        context.Logger.LogInformation(id + optionId);
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
        context.Logger.LogInformation("create Poll Request\n");
        context.Logger.LogInformation(request.Body.ToString());
        context.Logger.LogInformation("body deserialisoituna");

        Document document = Document.FromJson(request.Body);
        context.Logger.LogInformation(document.ToJsonPretty());

        var poll = Document.FromJson(request.Body) as Poll;

        context.Logger.LogInformation(poll?.ToString());

        var response = new APIGatewayProxyResponse
        {
            StatusCode = poll != null ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest,
            Body = poll != null ? await CreatePollAsync(poll) : "Bad Request Body",
            Headers = getHeaders()
        };

        return response;
    }

    public class Poll : Document
    {
        public Dictionary<string, string>? PollId { get; set; }
        public Dictionary<string, string>? Id { get; set; }
        public Dictionary<string, string>? Title { get; set; }
        public List<PollOption>? Options { get; set; }
    }
    public class PollOption
    {
        public Dictionary<string, string>? Id { get; set; }
        public Dictionary<string, string>? Title { get; set; }
        public Dictionary<string, string>? Votes { get; set; }
    }
}