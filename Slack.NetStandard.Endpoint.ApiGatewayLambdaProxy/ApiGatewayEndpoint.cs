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
    public abstract class ApiGatewayEndpoint : SlackRequestHandler<APIGatewayProxyRequest, APIGatewayProxyResponse>
    {
        private readonly RequestVerifier Verifier;

        protected ApiGatewayEndpoint(string signingSecret)
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

        protected override Task<APIGatewayProxyResponse> DefaultOKResponse(string body = null)
        {
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = body
            });
        }

        protected override Task<APIGatewayProxyResponse> InvalidRequestResponse(SlackInformation info, APIGatewayProxyRequest request)
        {
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = info.Type.ToString()
            });
        }

        protected override Task<APIGatewayProxyResponse> HandleCommand(SlashCommand infoCommand)
        {
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = "Command not supported"
            });
        }

        protected override Task<APIGatewayProxyResponse> HandleInteraction(InteractionPayload infoInteraction)
        {
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = "Interaction not supported"
            });
        }

        protected override Task<APIGatewayProxyResponse> HandleEventCallback(EventCallback infoEvent)
        {
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = "Event callback not supported"
            });
        }
    }
}
