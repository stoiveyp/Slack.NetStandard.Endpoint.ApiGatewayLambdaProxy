using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Slack.NetStandard.Interaction;

namespace Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy.Tests
{
    internal class SlackApp : ApiGatewayEndpoint
    {
        public SlackApp() : base("signingSecret")
        {
        }
    }
}