using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Slack.NetStandard.EventsApi;
using Slack.NetStandard.Interaction;

namespace Slack.NetStandard.Endpoint.ApiGatewayLambdaProxy
{
    public class ApiGatewayEndpoint : SlackEndpoint<APIGatewayProxyRequest>
    {
        private readonly RequestVerifier Verifier;

        public ApiGatewayEndpoint(string signingSecret)
        {
            Verifier = new RequestVerifier(signingSecret);
        }

        protected virtual bool IsValid(RequestVerifier verifier, APIGatewayProxyRequest request)
        {
            if (!request.Headers.ContainsKey(RequestVerifier.SignatureHeaderName) ||
                !request.Headers.ContainsKey(RequestVerifier.TimestampHeaderName) ||
                !long.TryParse(request.Headers[RequestVerifier.TimestampHeaderName], out var timestamp))
            {
                return false;
            }

            return verifier.Verify(request.Headers[RequestVerifier.SignatureHeaderName], timestamp, request.Body);
        }

        public Task<SlackInformation> ProcessRequest(APIGatewayProxyRequest request)
        {
            return GenerateInformation(request);
        }

        protected override Task<SlackInformation> GenerateInformation(APIGatewayProxyRequest request)
        {
            if (!IsValid(Verifier, request))
            {
                return Task.FromResult(new SlackInformation(SlackRequestType.NotVerifiedRequest));
            }

            var result = request.Headers["Content-Type"] switch
            {
                "application/json" => new SlackInformation(JsonConvert.DeserializeObject<Event>(request.Body)),
                "application/x-www-form-urlencoded" => request.Body.StartsWith("payload=") ?
                    new SlackInformation(JsonConvert.DeserializeObject<InteractionPayload>(HttpUtility.UrlDecode(request.Body.Substring(8)))) :
                    new SlackInformation(new SlashCommand(request.Body)),
                _ => new SlackInformation(SlackRequestType.UnknownRequest)
            };

            return Task.FromResult(result);
        }
    }
}
