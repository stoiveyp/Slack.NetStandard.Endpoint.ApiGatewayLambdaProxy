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

    protected override Task<APIGatewayProxyResponse> HandleCommand(SlashCommand infoCommand)
    {
        return base.HandleCommand(infoCommand);
    }

    protected override Task<ApiGatewayProxyResponse> HandleInteraction(InteractionPayload infoInteraction){
        return infoInteraction switch {
            GlobalShortcutPayload { CallbackId: "adventure_create" } adventurecreate => await CreateAdventure.Shortcut(adventurecreate),
            ViewSubmissionPayload viewSubmission => await ViewSubmission(client, viewSubmission),
        }
    }
}
```

2. Add it to your lambda

```csharp
public Function(){
    var signingSecret = //retrieve signing secret...
    AppEndpoint = new SlackApp(signingSecret);
}

public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input)
{
    return AppEndpoint.Process(input);
}
```