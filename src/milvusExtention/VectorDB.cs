// using Milvus.Client;
// namespace Omni.VectorDB;
// public class VectorDB
// {
//     // private readonly MilvusClient dbclient;
//     // public VectorDB(string host = "11.11.11.160", int port = 19530, bool UseSsl = false)
//     // {
//     //     string Host = host;
//     //     int Port = port;// This is Milvus's default port
//     //     // Default value is false
//     //     //  string Database = "devtest"; // Defaults to the default Milvus database
//     //     // See documentation for other constructor paramters
//     //     dbclient = new MilvusClient(Host, Port, UseSsl);
//     //     // ...
//     // }
//     // public MilvusClient client { get;  }
//     // /// <summary>
//     // /// 检查数据库在线/转FP
//     // /// </summary>
//     // /// <returns></returns>
//     // public async Task<bool> CheckConnection()
//     // {
//     //     MilvusHealthState result = await dbclient.HealthAsync();
//     //     return result.IsHealthy;
//     // }
//     // /// <summary>
//     // /// 创建集合//已经转FP
//     // /// </summary>
//     // /// <param name="collName"></param>
//     // /// <param name="fields"></param>
//     // /// <returns></returns>
//     // public async Task CreateCollection(string collName, FieldSchema[] fields)
//     // {
//     //     //  await v_vectorDBconnAsync();
//     //     MilvusCollection collection = dbclient.GetCollection(collName);
//     //     var hasCollection = await dbclient.HasCollectionAsync(collName);
//     //     if (hasCollection)
//     //     {
//     //         await collection.DropAsync();
//     //         //TODO: add Log output.
//     //         Console.WriteLine("Drop collection {0}", collName);
//     //     }
//     //     await dbclient.CreateCollectionAsync(collName, fields);
//     // }
//     // public MilvusCollection GetCollection(string collName) => dbclient.GetCollection(collName);
//     // public async Task InsertData(string collname, FieldData[] datas)
//     // {
//     //     var coll = dbclient.GetCollection(collname);
//     //     await coll.InsertAsync(datas);
//     //     // ...
//     // }
//     // /// <summary>
//     // /// Marked with Static Ext
//     // /// </summary>
//     // /// <param name="collName"></param>
//     // /// <param name="fieldName"></param>
//     // /// <param name="indexType"></param>
//     // /// <returns></returns>
//     // public async Task CreateIndexAndLoad(string collName, string fieldName, IndexType indexType)
//     // {
//     //     var coll = this.GetCollection(collName);//dbclient.GetCollection(collName);
//     //     await coll.CreateIndexAsync(fieldName, indexType);
//     //     await coll.LoadAsync();
//     // }
//     // public async Task SearchQuery(string collname)
//     // {
//     //     dbclient.GetCollection(collname);
//     // }

// }
