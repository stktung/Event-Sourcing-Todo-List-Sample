using System.Text;
using Newtonsoft.Json;
using SleekFlow.Todo.Application.Controllers;
using SleekFlow.Todo.Application.Middleware;

namespace SleekFlow.Todo.IntegrationTest;

public static class HttpClientTestHelperExtensions
{

    public static async Task<IEnumerable<GetTodoResponse>> GetAllAsync(this HttpClient client, bool? isCompleted = null,
        DateTime? dueDateBefore = null, DateTime? dueDateAfter = null, TodoController.SortByField? sortByField = null,
        bool? sortAsc = null)
    {
        var requestUri = $"/Todo?";

        if (isCompleted != null)
        {
            requestUri += $"&isCompleted={isCompleted}";
        }

        if (dueDateBefore != null)
        {
            requestUri += $"&dueDateIsBefore={dueDateBefore}";
        }

        if (dueDateAfter != null)
        {
            requestUri += $"&dueDateIsAfter={dueDateAfter}";
        }

        if (sortByField != null)
        {
            requestUri += $"&sortByField={sortByField.Value}";
        }

        if (sortAsc != null)
        {
            requestUri += $"&sortByAsc={sortAsc}";
        }

        var getResp = await client.GetAsync(requestUri);

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

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        UpdateTodoDueDateAsync(this HttpClient client, Guid id, long expectedVersion, DateTime? dueDate, string contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new UpdateTodoDueDateRequest(
            expectedVersion, dueDate));

        if (contentOverride != null) content = contentOverride;

        var respMsg =
            await client.PutAsync($"/Todo/{id}/duedate",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (respMsg.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await respMsg.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await respMsg.Content
                .ReadAsStringAsync());
        }


        return (resp, errResp, respMsg);
    }

    public static async Task<(GeneralPostTodoResponse? Response, ErrorResponse? ErrorResponse, HttpResponseMessage HttpResponse)>
        UpdateTodoIsCompletedAsync(this HttpClient client, Guid id, long expectedVersion, bool isCompleted, string contentOverride = null)
    {
        var content = JsonConvert.SerializeObject(new UpdateTodoIsCompletedRequest(
            expectedVersion, isCompleted));

        if (contentOverride != null) content = contentOverride;

        var respMsg =
            await client.PutAsync($"/Todo/{id}/completed",
                new StringContent(content, Encoding.UTF8, "application/json"));

        GeneralPostTodoResponse resp = null;
        ErrorResponse errResp = null;
        if (respMsg.IsSuccessStatusCode)
        {
            resp = JsonConvert.DeserializeObject<GeneralPostTodoResponse>(await respMsg.Content
                .ReadAsStringAsync());
        }
        else
        {
            errResp = JsonConvert.DeserializeObject<ErrorResponse>(await respMsg.Content
                .ReadAsStringAsync());
        }


        return (resp, errResp, respMsg);
    }

}