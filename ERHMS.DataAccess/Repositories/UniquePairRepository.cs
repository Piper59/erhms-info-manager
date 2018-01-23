using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class UniquePairRepository : EntityRepository<UniquePair>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(UniquePair))
            {
                TableName = "ERHMS_UniquePairs"
            };
            typeMap.Get(nameof(UniquePair.New)).SetComputed();
            typeMap.Get(nameof(UniquePair.Id)).SetComputed();
            typeMap.Get(nameof(UniquePair.Guid)).SetComputed();
            typeMap.Get(nameof(UniquePair.UniquePairId)).SetId();
            SqlMapper.SetTypeMap(typeof(UniquePair), typeMap);
        }

        public UniquePairRepository(DataContext context)
            : base(context.Database) { }

        public ILookup<string, string> SelectLookup()
        {
            return Select()
                .SelectMany(uniquePair => uniquePair.ToTuples())
                .ToLookup(tuple => tuple.Item1, tuple => tuple.Item2, StringComparer.OrdinalIgnoreCase);
        }
    }
}
