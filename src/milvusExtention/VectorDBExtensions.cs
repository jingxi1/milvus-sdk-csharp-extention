using System.Text.Json;
using System.Text.Json.Nodes;

using Milvus.Client;
namespace Milvus;
public record ClientProfile(string host, int port, bool UseSsl);
public static class MilvusExtensions
{
    /// <summary>
    /// 建立数据连接,建立访问客户端
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="UseSsl"></param>
    /// <returns></returns>
    public static MilvusClient BuildClient(string host = "11.11.11.160", int port = 19530, bool UseSsl = false)
    {
        return new MilvusClient(host, port, UseSsl);
    }
    public static MilvusClient BuildClient(this ClientProfile profile)
    {
        return new MilvusClient(profile.host, profile.port, profile.UseSsl);
    }
    /// <summary>
    /// 针对集合创建索引并加载
    /// </summary>
    /// <param name="coll"></param>
    /// <param name="collName"></param>
    /// <param name="fieldName"></param>
    /// <param name="indexType"></param>
    /// <returns></returns>
    public static async Task<MilvusCollection> CreateIndexThenLoadAsync(this MilvusCollection coll, string fieldName, IndexType indexType)
    {
        await coll.CreateIndexAsync(fieldName, indexType);
        await coll.LoadAsync();
        return coll;
    }
    /// <summary>
    /// 检查当前连接是否正常
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<bool> IsHealthyAsync(this MilvusClient client)
    {
        return (await client.HealthAsync()).IsHealthy;
    }
    // /// <summary>
    // /// 插入数据
    // /// </summary>
    // /// <param name="coll"></param>
    // /// <param name="datas"></param>
    // /// <returns></returns>
    // public static async Task<MutationResult> InsertData(this MilvusCollection coll, FieldData[] datas)
    //     {
    //        // var coll = dbclient.GetCollection(collname);
    //       return  await coll.InsertAsync(datas);
    //         // ...
    //     }
    /// <summary>
    /// 创建集合,如果集合已经存在则删除集合
    /// </summary>
    /// <param name="client"></param>
    /// <param name="collName"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static async Task<MilvusCollection> CreateTestCollectionAsync(this MilvusClient client, string collName, FieldSchema[] fields)
    {
        var hasCollection = await client.HasCollectionAsync(collName);
        if (hasCollection)
        {
            MilvusCollection collection = client.GetCollection(collName);
            await collection.DropAsync();
            //TODO: add Log output.
            Console.WriteLine("Drop collection {0}", collName);
        }
        return await client.CreateCollectionAsync(collName, fields);
    }
    /// <summary>
    /// 以字段为基础,向列表添加,如果无列表,则建立列表
    /// </summary>
    /// <param name="fs"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static List<FieldSchema> AddCreateCollectionFields(this FieldSchema fs, List<FieldSchema> fields = null)
    {
        return AddField(fields, fs);
    }
    /// <summary>
    /// 列表中添加字断
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="fs"></param>
    /// <returns></returns>
    public static List<FieldSchema> AddCreateCollectionFields(this List<FieldSchema> fields, FieldSchema fs)
    {
        return AddField(fields, fs);
    }
    private static List<FieldSchema> AddField(List<FieldSchema> fields, FieldSchema fs)
    {
        if (fields == null)
        {
            fields = new List<FieldSchema>();
        }
        fields.Add(fs);
        return fields;
    }
    //    private static Dictionary<MilvusDataType, Func<string, object>>? parsers = new Dictionary<MilvusDataType, Func<string, object>>
    //{
    //    { MilvusDataType.FloatVector, x => new ReadOnlyMemory<float>(JsonSerializer.Deserialize<float[]>(x)) },
    //    { MilvusDataType.BinaryVector, x => new ReadOnlyMemory<byte>(JsonSerializer.Deserialize<byte[]>(x)) },
    //    { MilvusDataType.Bool,x=> bool.Parse(x) },
    //    { MilvusDataType.Int64, x=>Int64.Parse(x) },
    //    { MilvusDataType.Int32, x=>int.Parse(x) },
    //    { MilvusDataType.Int8, x=>Int16.Parse(x) },
    //    { MilvusDataType.Int16, x=>Int16.Parse(x) },
    //    { MilvusDataType.Float, x=>float.Parse(x) },
    //    { MilvusDataType.Double, x=>double.Parse(x) },
    //    { MilvusDataType.String, x => x },
    //    { MilvusDataType.VarChar, x => x },
    //    { MilvusDataType.Json, x => x },
    //    { MilvusDataType.Array, x => JsonSerializer.Deserialize<List<object>>(x) },
    //};
    /// <summary>
    /// 根据结构及json集合数据生成可以使用的向量字段数据
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static List<FieldData> GetData(this List<FieldSchema> fields, JsonArray data)
    {
        var dl = new List<FieldData>();
        fields.AsParallel().ForAll(x =>
        {
            //var rdata = jn.Select(xf => xf[x.Name].ToString()).ToArray();
            //if (parsers.TryGetValue(x.DataType, out var parser))
            //{
            //    var f = rdata.Select(x => parser(x)).ToList();
            //    dl.Add(FieldData.Create(x.Name, f));
            //}
            //else
            //{
            //    //TODO: 处理数据类型转换
            //    dl.Add(FieldData.Create(x.Name, rdata));
            //}
            var rdata = data.Select(xf => xf[x.Name].ToString()).ToArray();
            if (x.DataType == MilvusDataType.FloatVector)
            {
                var f = rdata.Select(x => new ReadOnlyMemory<float>(JsonSerializer.Deserialize<float[]>(x))).ToList();
                dl.Add(FieldData.CreateFloatVector(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.BinaryVector)
            {
                var f = rdata.Select(x => new ReadOnlyMemory<byte>(JsonSerializer.Deserialize<byte[]>(x))).ToList();
                dl.Add(FieldData.CreateBinaryVectors(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Bool)
            {
                var f = rdata.Select(x => bool.Parse(x)).ToList();
                dl.Add(FieldData.Create<bool>(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Int64)
            {
                var f = rdata.Select(x => Int64.Parse(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Int32)
            {
                var f = rdata.Select(x => int.Parse(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Int8 || x.DataType == MilvusDataType.Int16)
            {
                var f = rdata.Select(x => Int16.Parse(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Float)
            {
                var f = rdata.Select(x => float.Parse(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Double)
            {
                var f = rdata.Select(x => Double.Parse(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.String || x.DataType == MilvusDataType.VarChar || x.DataType == MilvusDataType.Json)
            {
                var f = rdata.Select(x => x).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else if (x.DataType == MilvusDataType.Array)
            {
                var f = rdata.Select(x => JsonSerializer.Deserialize<List<object>>(x)).ToList();
                dl.Add(FieldData.Create(x.Name, f));
            }
            else
            {
                //TODO: 处理数据类型转换
                dl.Add(FieldData.Create(x.Name, rdata));
            }
        });
        return dl;
    }
    /// <summary>
    /// 获取向向量库添加的数据,通过字段列表,对向列表,进行转换,如果提供字段间对向量生成的关系,可以对应将向量数据进行整合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fields"></param>
    /// <param name="data"></param>
    /// <param name="embdingField"></param>
    /// <param name="embedingFunc"></param>
    /// <returns></returns>
    public static List<FieldData> GetDataWithEmbeding<T>(this List<FieldSchema> fields, List<T> data, KeyValuePair<string, string>? embdingField = null, Func<string, float[]> embedingFunc = null)
    {
        //FIXED: 考虑将嵌入字段的对应关系,和embeding函数合成一个参数
        var jn = JsonNode.Parse(JsonSerializer.Serialize(data)).AsArray();
        if (embedingFunc != null && embdingField != null)
        {
            jn.ToList().ForEach(x =>
            {
                x[embdingField.GetValueOrDefault().Value] =
                   JsonSerializer
                   .Serialize(embedingFunc(x[embdingField.GetValueOrDefault().Key].ToString()));
            });
        }
        return GetData(fields, jn);
    }
    /// <summary>
    /// 输入向量数据,及对应的字段,进行向量查询
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="vField"></param>
    /// <param name="vectorData"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public static Task<SearchResults> SearchResultsAsync(this MilvusCollection collection, string vField, float[] vectorData, int limit = 1)
    {
        return collection.SearchAsync(vField,
              new ReadOnlyMemory<float>[] { vectorData }, SimilarityMetricType.Ip, limit);
    }
    /// <summary>
    /// 查询结果,根据向量查询结果,对结果对应索引数据进行查询
    /// </summary>
    /// <param name="searchResults"></param>
    /// <param name="idField"></param>
    /// <param name="desField"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static async Task<List<string>> QueryResultsAsync(this Task<SearchResults> searchResults, string idField, string desField, MilvusCollection collection)
    {
        //TODO: 尝试去掉集合参数
        //  var l = new List<string>();
        var sr = await searchResults;
        var qstr = sr.Ids.TojsonNode()["LongIds"].ToString();
        string expr = $"{idField} in {qstr}";
        QueryParameters queryParameters = new();
        queryParameters.OutputFields.Add(desField);
        var queryResult = await collection.QueryAsync(
          expr,
          queryParameters);
        return queryResult
        .SingleOrDefault(c => c.FieldName == desField)
         .TojsonNode()["Data"].AsArray()
          .Select(x => x.ToString()).ToList();
    }
    /// <summary>
    /// 将搜索及对应查询进行合并
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="vField"></param>
    /// <param name="idField"></param>
    /// <param name="desField"></param>
    /// <param name="vectorData"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public static async Task<List<string>> SearchAndQueryResultsAsync(this MilvusCollection collection, string vField, string idField, string desField, float[] vectorData, int limit = 1)
    {
        return await collection.SearchResultsAsync(vField, vectorData, limit).QueryResultsAsync(idField, desField, collection);
    }

    private static JsonNode TojsonNode(this object obj)
    {
        return JsonNode.Parse(JsonSerializer.Serialize(obj));
    }
}
