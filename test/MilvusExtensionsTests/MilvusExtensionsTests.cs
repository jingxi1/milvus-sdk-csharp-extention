using Milvus.Client;
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
            var db = new Milvus.ClientProfile("11.11.11.160", 19530, false).BuildClient();
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
            var collection = await db.CreateCollectionAsync("test", colSchema);
            //建立資料
            var data = Enumerable.Range(0, 1000).Select(x => new
            {
                art_id = x,
                art_des = $"test{x}case",
                art_vector = ""
            }).ToList();

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
    }
}