using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Assignment : TableEntity
    {
        public override string Guid
        {
            get { return AssignmentId; }
            set { AssignmentId = value; }
        }

        public string AssignmentId
        {
            get { return GetProperty<string>("AssignmentId"); }
            set { SetProperty("AssignmentId", value); }
        }

        //public Epi View
        //{
        //    get { return GetProperty<string>("View"); }
        //    set { SetProperty("View", value); }
        //}
    }
}
