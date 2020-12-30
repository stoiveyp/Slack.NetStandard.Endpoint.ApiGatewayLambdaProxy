using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Slack.NetStandard.EventsApi;
using Slack.NetStandard.Interaction;

namespace Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy.Tests
{
    public class TestEndpoint:ApiGatewayEndpoint
    {
        protected override bool IsValid(RequestVerifier verifier, APIGatewayProxyRequest request)
        {
            return true;
        }

        public TestEndpoint(string signingSecret) : base(signingSecret)
        {
        }

        public Func<SlashCommand, APIGatewayProxyResponse> Command { get; set; }
        public Func<InteractionPayload, APIGatewayProxyResponse> Interaction { get; set; }
        public Func<EventCallback, APIGatewayProxyResponse> Event { get; set; }
    }
}
