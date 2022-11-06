using System.Text;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;
using SleekFlow.Todo.Application.Middleware;

namespace SleekFlow.Todo.IntegrationTest;

public static class HttpClientTestHelperExtensions
{

    public static async Task<IEnumerable<GetTodoResponse>> GetAllAsync(this HttpClient client)
    {
        var getResp = await client.GetAsync($"/Todo");

        var getResponseBody =
            await getResp.Content.ReadAsStringAsync();
        var todos = JsonConvert.DeserializeObject<IEnumerable<GetTodoResponse>>(getResponseBody);
        return todos;
    }

    public static async Task<GeneralPostTodoResponse> CreateTodoAsync(this HttpClient client)
    {
        return JsonConvert.DeserializeObject<GeneralPostTodoResponse>(
            await (await client.PostAsync("/Todo/create", null))
                .Content.ReadAsStringAsync());
    }

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        InsertTodoNameTextAsync(this HttpClient client, Guid id, long expectedVersion, string text, int position, string? contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new InsertTodoNameTextRequest(
            expectedVersion, text, position));

        if (contentOverride != null) content = contentOverride;

        var insertRespMsg =
            await client.PostAsync($"/Todo/{id}/name/inserttext",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (insertRespMsg.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await insertRespMsg.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await insertRespMsg.Content
                .ReadAsStringAsync());
        }


        return (resp, errResp, insertRespMsg);
    }

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        DeleteTodoNameTextAsync(this HttpClient client, Guid id, long expectedVersion, int position, int length, string? contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new DeleteTodoNameTextRequest(
            expectedVersion, position, length));

        if (contentOverride != null) content = contentOverride;

        var httpMessage =
            await client.PostAsync($"/Todo/{id}/name/deletetext",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (httpMessage.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await httpMessage.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await httpMessage.Content
                .ReadAsStringAsync());
        }

        return (resp, errResp, httpMessage);
    }

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        InsertTodoDescriptionTextAsync(this HttpClient client, Guid id, long expectedVersion, string text, int position, string? contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new InsertTodoDescriptionTextRequest(
            expectedVersion, text, position));

        if (contentOverride != null) content = contentOverride;

        var insertRespMsg =
            await client.PostAsync($"/Todo/{id}/description/inserttext",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (insertRespMsg.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await insertRespMsg.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await insertRespMsg.Content
                .ReadAsStringAsync());
        }


        return (resp, errResp, insertRespMsg);
    }

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        DeleteTodoDescriptionTextAsync(this HttpClient client, Guid id, long expectedVersion, int position, int length, string? contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new DeleteTodoDescriptionTextRequest(
            expectedVersion, position, length));

        if (contentOverride != null) content = contentOverride;

        var httpMessage =
            await client.PostAsync($"/Todo/{id}/description/deletetext",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (httpMessage.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await httpMessage.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await httpMessage.Content
                .ReadAsStringAsync());
        }

        return (resp, errResp, httpMessage);
    }
}