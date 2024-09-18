// See https://aka.ms/new-console-template for more information
using Milvus.Client;

string Host = "192.168.9.8";
int Port = 19530; // This is Milvus's default port
bool UseSsl = false; // Default value is false
string Database = "my_database"; // Defaults to the default Milvus database
string collectionName = "book";
// See documentation for other constructor paramters
var milvusClient = new MilvusClient(Host, Port, UseSsl);
MilvusHealthState result = await milvusClient.HealthAsync();
Console.WriteLine("Hello, World!");
//await CreateCollection();
//await IntertData();
var ccc = await CreateLoad();
await searchQuery(ccc);

async Task CreateCollection()
{

    MilvusCollection collection = milvusClient.GetCollection(collectionName);

    //Check if this collection exists
    var hasCollection = await milvusClient.HasCollectionAsync(collectionName);

    if (hasCollection)
    {
        await collection.DropAsync();
        Console.WriteLine("Drop collection {0}", collectionName);
    }

    await milvusClient.CreateCollectionAsync(
                collectionName,
                new[] {
                FieldSchema.Create<long>("book_id", isPrimaryKey:true),
                FieldSchema.Create<long>("word_count"),
                FieldSchema.CreateVarchar("book_name", 256),
                FieldSchema.CreateFloatVector("book_intro", (int)2L)
                }
            );
}

async Task IntertData()
{
    Random ran = new();
    List<long> bookIds = new();
    List<long> wordCounts = new();
    List<ReadOnlyMemory<float>> bookIntros = new();
    List<string> bookNames = new();
    for (long i = 0L; i < 2000; ++i)
    {
        bookIds.Add(i);
        wordCounts.Add(i + 10000);
        bookNames.Add($"Book Name {i}");

        float[] vector = new float[2];
        for (int k = 0; k < 2; ++k)
        {
            vector[k] = ran.Next();
        }
        bookIntros.Add(vector);
    }

    MilvusCollection collection = milvusClient.GetCollection(collectionName);

    MutationResult result = await collection.InsertAsync(
        new FieldData[]
        {
        FieldData.Create("book_id", bookIds),
        FieldData.Create("word_count", wordCounts),
        FieldData.Create("book_name", bookNames),
        FieldData.CreateFloatVector("book_intro", bookIntros),
        });

    // Check result
    Console.WriteLine("Insert count:{0},", result.InsertCount);
}
async Task<MilvusCollection> CreateLoad()
{
    MilvusCollection collection = milvusClient.GetCollection(collectionName);
    await collection.CreateIndexAsync(
        "book_intro",
        //MilvusIndexType.IVF_FLAT,//Use MilvusIndexType.IVF_FLAT.
        IndexType.AutoIndex,//Use MilvusIndexType.AUTOINDEX when you are using zilliz cloud.
        SimilarityMetricType.L2);

    // Check index status
    IList<MilvusIndexInfo> indexInfos = await collection.DescribeIndexAsync("book_intro");

    foreach (var info in indexInfos)
    {
        Console.WriteLine("FieldName:{0}, IndexName:{1}, IndexId:{2}", info.FieldName, info.IndexName, info.IndexId);
    }

    // Then load it
    // await milvusClient.LoadCollectionAsync(collectionName);
    var r = milvusClient.GetCollection(collectionName);
    return r;
}
async Task searchQuery(MilvusCollection collection)
{
    List<string> search_output_fields = new() { "book_id" };
    List<List<float>> search_vectors = new() { new() { 0.1f, 0.2f } };
    SearchResults searchResult = await collection.SearchAsync(
        "book_intro",
        new ReadOnlyMemory<float>[] { new[] { 0.1f, 0.2f } },
        SimilarityMetricType.L2,
        limit: 2);

    // Query
    string expr = "book_id in [2,4,6,8]";

    QueryParameters queryParameters = new();
    queryParameters.OutputFields.Add("book_id");
    queryParameters.OutputFields.Add("word_count");

    IReadOnlyList<FieldData> queryResult = await collection.QueryAsync(
        expr,
        queryParameters);

    queryResult.ToList().ForEach(x => { Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(x)); });
}