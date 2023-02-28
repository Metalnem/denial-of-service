using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.Net;

public class Input
{
    public Input A;
}

public class StackOverflow
{
    [Function(nameof(StackOverflow))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData request)
    {
        using (var streamReader = new StreamReader(request.Body))
        {
            var body = await streamReader.ReadToEndAsync();
            JsonConvert.DeserializeObject<Input>(body);
        }

        return request.CreateResponse(HttpStatusCode.OK);
    }
}
