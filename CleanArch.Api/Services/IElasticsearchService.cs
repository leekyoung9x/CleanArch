using Nest;
using System.Threading.Tasks;

namespace CleanArch.Api.Services 
{
    public interface IElasticsearchService
    {
        Task<IndexResponse> IndexDocumentAsync<T>(T document) where T : class;
        Task<UpdateResponse<T>> UpdateDocumentAsync<T>(string id, T document) where T : class;
        Task<DeleteResponse> DeleteDocumentAsync<T>(string id) where T : class;
        Task<T> GetDocumentByIdAsync<T>(string id) where T : class;
        Task<BulkResponse> InsertDataELK<T>(List<T> listData) where T : class;
    }
}

