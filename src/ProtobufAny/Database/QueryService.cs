using DynamicSearch.Core;
using DynamicSearch.Extenstions;
using Mapster;
using ProtobufAny.Database.Configurations;
using ProtobufAny.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufAny.Database
{
    public class QueryService
    {
        private readonly ChinookContext _chinookContext;
        public QueryService(ChinookContext chinookContex)
        {
            _chinookContext = chinookContex;
        }

        public QueryReply Query<TInput, TOutput>(SearchParameters searchParameters) where TInput : class
        {
            var result = _chinookContext.Set<TInput>().AsQueryable().Search(searchParameters).ToList();
            var output = result.Adapt<List<TOutput>>();

            var reply = new QueryReply();
            reply.List.AddRange(output.Select(x => x.Pack()));
            return reply;
        }
    }
}
