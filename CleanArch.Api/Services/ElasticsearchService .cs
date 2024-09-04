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

        public async Task<BulkResponse> InsertDataELK<T>(List<T> listData, string index) where T : class
        {
            BulkResponse result = null;

            result = await _elasticClient.BulkAsync(b => b.Index(index ?? "transaction").CreateMany(listData));

            return result;
        }

        public async Task BulkUpsertDocuments<T>(IEnumerable<T> documents, Func<T, string> idSelector, string indexName = "transaction") where T : class
        {
            var bulkDescriptor = new BulkDescriptor();

            foreach (var document in documents)
            {
                var id = idSelector(document);
                bulkDescriptor.Update<T>(u => u
                    .Index(indexName)
                    .Id(id)                              // Chỉ định ID cho mỗi tài liệu
                    .Doc(document)                       // Chỉ định tài liệu sẽ được cập nhật
                    .DocAsUpsert(true)                   // Nếu không tồn tại, sẽ chèn tài liệu này
                );
            }

            var bulkResponse = await _elasticClient.BulkAsync(bulkDescriptor);

            if (bulkResponse.Errors)
            {
                foreach (var itemWithError in bulkResponse.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to upsert document ID {itemWithError.Id}: {itemWithError.Error.Reason}");
                }
            }
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