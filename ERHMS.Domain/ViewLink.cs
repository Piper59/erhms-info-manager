﻿using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class ViewLink : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("ViewLinkId");
            }
            set
            {
                if (!SetProperty("ViewLinkId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string ViewLinkId
        {
            get { return GetProperty<string>("ViewLinkId"); }
            set { SetProperty("ViewLinkId", value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>("ViewId"); }
            set { SetProperty("ViewId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
