using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAssignment;

public static class Extensions
{
    public static bool IsValid(this ModelStateDictionary ms, string key)
    {
        return ms.GetFieldValidationState(key) == ModelValidationState.Valid;
    }

    public static bool IsAjax(this HttpRequest request)
    {
        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }
}
