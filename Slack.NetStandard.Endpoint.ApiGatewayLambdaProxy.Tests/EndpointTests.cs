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
            Assert.Equal((int)HttpStatusCode.BadRequest,response.StatusCode);
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
            var parsed = false;
            var fake = new TestEndpoint("xxx")
            {
                Command = command =>
                {
                    Assert.Equal("teamx",command.TeamId);
                    parsed = true;
                    return null;
                }
            };
            var response = await fake.Process(request);
            Assert.True(parsed);
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
            var parsed = false;
            var fake = new TestEndpoint("xxx")
            {
                Interaction = payload =>
                {
                    Assert.IsType<ViewClosedPayload>(payload);
                    parsed = true;
                    return null;
                }
            };
            var response = await fake.Process(request);
            Assert.True(parsed);
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
            var parsed = false;
            var fake = new TestEndpoint("xxx")
            {
                Event = eventResult =>
                {
                    Assert.IsType<AppHomeOpened>(eventResult.Event);
                    parsed = true;
                    return null;
                }
            };
            var response = await fake.Process(request);
            Assert.True(parsed);
        }
    }
}
