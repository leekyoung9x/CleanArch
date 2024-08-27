using Nest;
using System.Threading.Tasks;

namespace CleanArch.Api.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticsearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<BulkResponse> InsertDataELK<T>(List<T> listData) where T : class
        {
            BulkResponse result = null;

            result = await _elasticClient.BulkAsync(b => b.CreateMany(listData));

            return result;
        }

        public async Task<IndexResponse> IndexDocumentAsync<T>(T document) where T : class
        {
            return await _elasticClient.IndexDocumentAsync(document);
        }

        public async Task<UpdateResponse<T>> UpdateDocumentAsync<T>(string id, T document) where T : class
        {
            return await _elasticClient.UpdateAsync<T>(id, u => u.Doc(document));
        }

        public async Task<DeleteResponse> DeleteDocumentAsync<T>(string id) where T : class
        {
            return await _elasticClient.DeleteAsync<T>(id);
        }

        public async Task<T> GetDocumentByIdAsync<T>(string id) where T : class
        {
            var response = await _elasticClient.GetAsync<T>(id);
            return response.Source;
        }
    }
}