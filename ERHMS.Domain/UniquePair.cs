using ERHMS.EpiInfo.Domain;
using System;
using System.Collections.Generic;

namespace ERHMS.Domain
{
    public class UniquePair : GuidEntity
    {
        public override string Guid
        {
            get { return UniquePairId; }
            set { UniquePairId = value; }
        }

        public string UniquePairId
        {
            get { return GetProperty<string>(nameof(UniquePairId)); }
            set { SetProperty(nameof(UniquePairId), value); }
        }

        public string Responder1Id
        {
            get { return GetProperty<string>(nameof(Responder1Id)); }
            set { SetProperty(nameof(Responder1Id), value); }
        }

        public string Responder2Id
        {
            get { return GetProperty<string>(nameof(Responder2Id)); }
            set { SetProperty(nameof(Responder2Id), value); }
        }

        public UniquePair(bool @new)
            : base(@new) { }

        public UniquePair()
            : this(false) { }

        public IEnumerable<Tuple<string, string>> ToTuples()
        {
            yield return Tuple.Create(Responder1Id, Responder2Id);
            yield return Tuple.Create(Responder2Id, Responder1Id);
        }
    }
}
