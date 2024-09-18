using Milvus.Client;

using OmniCode.Util;
namespace Milvus.Tests
{
    [TestClass()]
    public class MilvusExtensionsTests
    {
        [TestMethod()]
        public async Task AccessClientTest()
        {
            Random Random = new Random();
            //建立連線
            var db = new Milvus.ClientProfile("192.168.9.8", 19530, false).BuildClient();
            //檢查連線是否正常
            await db.IsHealthyAsync();
            //建立Collection
            var colSchema = new List<Milvus.Client.FieldSchema>();
            //設定欄位
            colSchema = colSchema
                .AddCreateCollectionFields(FieldSchema.Create<long>("art_id", isPrimaryKey: true))
                   .AddCreateCollectionFields(FieldSchema.CreateVarchar("art_des", 1500))
                   .AddCreateCollectionFields(FieldSchema.CreateFloatVector("art_vector", 6));

            //建立Collection
            var collection = await db.CreateCollectionAsync("test_" + DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), colSchema);
            //建立資料
            var data = Enumerable
                .Range(0, 1000)
                .Select(x => new
                {
                    art_id = x,
                    art_des = $"test{x}case",
                    art_vector = ""
                })
                .ToList();

            //將資料加入Collection
            var datawithEmb = colSchema
                .GetDataWithEmbeding(data, new("art_des", "art_vector"), embFunc);
            //將資料加入Collection
            await collection.InsertAsync(datawithEmb);
            //建立索引
            await collection.CreateIndexThenLoadAsync("art_vector", IndexType.AutoIndex);


            //模拟函数, 生成随机向量
            float[] embFunc(string q)
               => [Random.Next(), 2, 3, 4, 5, Random.Next()];

        }

        private static long[] embFunc(string q)
           => [new Random().Next(), 2, 3, 4, 5, new Random().Next()];
        [TestMethod]
        public async Task VDB_Access()
        {
            var db = new Milvus.ClientProfile("192.168.9.8", 19530, false).BuildClient();
            //檢查連線是否正常
            await db.IsHealthyAsync();
            var clname = await db.ListCollectionsAsync();
            clname.ToList().ForEach(x => x.Name.DebugWriteLine());

            var r = db.GetCollection("book");

            "-----------".DebugWriteLine();
            await VSearch(r);
            "-----------".DebugWriteLine();
            //  await VQuery(r);

        }
        private async Task VSearch(MilvusCollection collection)
        {
            //Search
            //  List<string> search_output_fields = new() { "book_id","book_name" };
            // List<List<float>> search_vectors = new() { new() { 0.5f, 0.2f } };
            SearchResults searchResult = await collection.SearchAsync(
                "book_intro",
                new ReadOnlyMemory<float>[] { new[] { 932108698f, 757030336f } },
                SimilarityMetricType.L2,
                limit: 5);
            searchResult.DebugJsonWriteLine();
        }

        private async Task VQuery(MilvusCollection collection)
        {
            // Query
            string expr = "book_id in [2,4,6,8,10,12]";

            QueryParameters queryParameters = new();
            queryParameters.OutputFields.Add("book_id");
            queryParameters.OutputFields.Add("word_count");
            queryParameters.OutputFields.Add("book_name");


            IReadOnlyList<FieldData> queryResult = await collection.QueryAsync(
                expr,
                queryParameters);

            queryResult.ToList().ForEach(x =>
            {
                //  Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(x));
                //   System.Text.Json.JsonSerializer.Serialize(x).DebugWriteLine();

                x.ToJsonNode().DebugWriteLine();

            });
        }
    }
}