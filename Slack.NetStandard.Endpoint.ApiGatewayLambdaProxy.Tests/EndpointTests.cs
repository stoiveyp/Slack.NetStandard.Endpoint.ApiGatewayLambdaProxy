using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Newtonsoft.Json;
using Slack.NetStandard.EventsApi;
using Slack.NetStandard.EventsApi.CallbackEvents;
using Slack.NetStandard.Interaction;
using Xunit;

namespace Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy.Tests
{
    public class EndpointTests
    {
        [Fact]
        public async Task UnverifiedTests()
        {
            var request = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string>()
            };
            var fake = new SlackApp();
            var response = await fake.Process(request);
            Assert.Equal(SlackRequestType.NotVerifiedRequest, response.Type);
        }

        [Fact]
        public async Task TestSlashCommand()
        {
            var request = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type","application/x-www-form-urlencoded"}
                },
                Body = "team_id=teamx"
            };
            var fake = new TestEndpoint("xxx");
            var response = await fake.Process(request);
            Assert.NotNull(response.Command);
            Assert.Equal("teamx", response.Command.TeamId);
        }

        [Fact]
        public async Task TestInteractionPayload()
        {
            var request = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type","application/x-www-form-urlencoded"}
                },
                Body = "payload="+HttpUtility.UrlEncode(JsonConvert.SerializeObject(new ViewClosedPayload{Type = InteractionType.ViewClosed}))
            };
            var fake = new TestEndpoint("xxx");
            var response = await fake.Process(request);
            Assert.IsType<ViewClosedPayload>(response.Interaction);
        }

        [Fact]
        public async Task TestEventPayload()
        {
            var request = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type","application/json"}
                },
                Body = JsonConvert.SerializeObject(new EventCallback<AppHomeOpened>{Type = EventCallbackBase.EventType, Event = new AppHomeOpened
                {
                    Type = AppHomeOpened.EventType
                }})
            };
            var fake = new TestEndpoint("xxx");
            var response = await fake.Process(request);
            Assert.IsType<AppHomeOpened>(Assert.IsType<EventCallback>(response.Event).Event);
        }
    }
}
