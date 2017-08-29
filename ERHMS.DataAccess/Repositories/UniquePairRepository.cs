using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
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
            typeMap.Get(nameof(UniquePair.UniquePairId)).SetId();
            typeMap.Get(nameof(UniquePair.New)).SetComputed();
            SqlMapper.SetTypeMap(typeof(UniquePair), typeMap);
        }

        public UniquePairRepository(DataContext context)
            : base(context) { }

        private IEnumerable<Tuple<string, string>> SelectTuples()
        {
            foreach (UniquePair uniquePair in Select())
            {
                yield return new Tuple<string, string>(uniquePair.Responder1Id, uniquePair.Responder2Id);
                yield return new Tuple<string, string>(uniquePair.Responder2Id, uniquePair.Responder1Id);
            }
        }

        public ILookup<string, string> SelectLookup()
        {
            return SelectTuples().ToLookup(tuple => tuple.Item1, tuple => tuple.Item2, StringComparer.OrdinalIgnoreCase);
        }
    }
}
