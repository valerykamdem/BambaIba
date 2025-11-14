
using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Features.Videos.GetVideos;
public sealed record GetVideosRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
}


//// Ce record gère tous les paramètres nécessaires pour la pagination
//// Vous pouvez le réutiliser dans n'importe quelle Minimal API qui utilise [FromQuery].
//public record GetVideosRequest(int Page, int PageSize, string Search, string Category, string Sortby)
//{
//    // Valeurs par défaut pour la pagination si non fournies
//    public const int DefaultPage = 1;
//    public const int DefaultPageSize = 20;

//    /// <summary>
//    /// Méthode statique obligatoire pour le Model Binding des objets complexes 
//    /// dans les Minimal APIs. Elle remplace [FromQuery] pour les objets complets.
//    /// </summary>
//    /// <param name="context">Le contexte HTTP actuel.</param>
//    /// <returns>Une tâche asynchrone retournant l'objet PaginationRequest construit.</returns>
//    public static ValueTask<GetVideosRequest> BindAsync(HttpContext context)
//    {
//        IQueryCollection query = context.Request.Query;

//        // --- 1. Extraction des entiers avec des valeurs par défaut ---

//        // Tenter de récupérer et de parser Page
//        int page = DefaultPage;
//        if (query.TryGetValue("Page", out Microsoft.Extensions.Primitives.StringValues pageValue) && int.TryParse(pageValue, out int parsedPage))
//        {
//            page = parsedPage > 0 ? parsedPage : DefaultPage; // Assurer Page >= 1
//        }

//        // Tenter de récupérer et de parser PageSize
//        int pageSize = DefaultPageSize;
//        if (query.TryGetValue("PageSize", out Microsoft.Extensions.Primitives.StringValues pageSizeValue) && int.TryParse(pageSizeValue, out int parsedPageSize))
//        {
//            pageSize = parsedPageSize > 0 ? parsedPageSize : DefaultPageSize; // Assurer PageSize >= 1
//        }

//        // --- 2. Extraction du string de recherche ---

//        // Récupérer la chaîne de recherche, category, searchby, null si non présente
//        string search = query["Search"].ToString() ?? string.Empty;
//        string category = query["Category"].ToString() ?? string.Empty;
//        string sortby = query["Sortby"].ToString() ?? string.Empty;

//        // 3. Créer et retourner le record
//        var request = new GetVideosRequest(page, pageSize, search, category, sortby);
//        return ValueTask.FromResult(request);
//    }
//}

