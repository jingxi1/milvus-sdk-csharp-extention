# VectorDBExtensions.cs

## 中文描述

`VectorDBExtensions.cs` 是一个包含多个扩展方法的静态类，这些方法主要用于操作和管理 Milvus 向量数据库。这些方法包括：

- 建立 Milvus 客户端连接
- 对集合创建索引并加载
- 检查当前连接是否正常
- 创建测试集合，如果集合已经存在则删除集合
- 向字段列表添加字段
- 根据结构和 JSON 集合数据生成可用的向量字段数据
- 获取向向量库添加的数据，通过字段列表，对向列表，进行转换
- 输入向量数据，及对应的字段，进行向量查询
- 查询结果，根据向量查询结果，对结果对应索引数据进行查询
- 将搜索及对应查询进行合并

## English Description

`VectorDBExtensions.cs` is a static class containing multiple extension methods mainly for operating and managing the Milvus vector database. These methods include:

- Establishing a Milvus client connection
- Creating an index for a collection and loading it
- Checking if the current connection is healthy
- Creating a test collection, if the collection already exists, it is dropped
- Adding fields to a field list
- Generating usable vector field data based on structure and JSON collection data
- Getting data to be added to the vector library, converting through the field list and vector list
- Inputting vector data and corresponding fields for vector query
- Querying results, based on vector query results, querying corresponding index data for results
- Combining search and corresponding query
