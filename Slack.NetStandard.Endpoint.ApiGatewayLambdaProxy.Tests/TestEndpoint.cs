using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Slack.NetStandard.EventsApi;
using Slack.NetStandard.Interaction;

namespace Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy.Tests
{
    public class BypassValidationEndpoint:ApiGatewayEndpoint
    {
        protected override bool IsValid(RequestVerifier verifier, APIGatewayProxyRequest request)
        {
            return true;
        }

        public BypassValidationEndpoint(string signingSecret) : base(signingSecret)
        {
        }

        protected override Task<APIGatewayProxyResponse> HandleCommand(SlashCommand infoCommand)
        {
            throw new NotImplementedException();
        }

        protected override Task<APIGatewayProxyResponse> HandleInteraction(InteractionPayload infoInteraction)
        {
            throw new NotImplementedException();
        }

        protected override Task<APIGatewayProxyResponse> HandleEventCallback(EventCallback infoEvent)
        {
            throw new NotImplementedException();
        }
    }
}
