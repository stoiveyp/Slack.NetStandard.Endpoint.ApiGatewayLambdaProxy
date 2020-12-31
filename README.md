# Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy
Slack Endpoint for AWS Lambda using API Gateway proxy

## Usage

1. Subclass the ApiGatewayEndpoint and implement your slack functionality

```csharp
private class SlackApp : ApiGatewayEndpoint
{
    public SlackApp(string signingSecret) : base(signingSecret)
    {
    
    }
}
```

2. Add it to your lambda

```csharp
public Function(){
    var signingSecret = //retrieve signing secret...
    Endpoint = new APIGatewayEndpoint(signingSecret);
}

public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input)
{
    var slackInfo = AppEndpoint.Process(input);
    if(slackInfo.Command != null && slackInfo.Command.Command == "/weather"){
       ...
    }
}
```
