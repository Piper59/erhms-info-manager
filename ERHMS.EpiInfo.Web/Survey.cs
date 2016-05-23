using Epi;
using Epi.SurveyManagerServiceV2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo.Web
{
    public class Survey : INotifyPropertyChanged
    {
        internal static Survey FromServiceObject(SurveyInfoDTO serviceObject)
        {
            return new Survey
            {
                ServiceObject = serviceObject,
                SurveyId = serviceObject.SurveyId,
                Title = serviceObject.SurveyName,
                StartDate = serviceObject.StartDate,
                EndDate = serviceObject.ClosingDate,
                ResponseType = ResponseTypeExtensions.FromEpiInfoValue(serviceObject.SurveyType),
                Intro = serviceObject.IntroductionText,
                Outro = serviceObject.ExitText,
                Draft = serviceObject.IsDraftMode,
                PublishKey = serviceObject.UserPublishKey,
            };
        }

        private SurveyInfoDTO ServiceObject { get; set; }

        private string surveyId;
        public string SurveyId
        {
            get { return surveyId; }
            set { Set(nameof(SurveyId), ref surveyId, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(nameof(Title), ref title, value); }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { Set(nameof(StartDate), ref startDate, value.Date); }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set { Set(nameof(EndDate), ref endDate, value.Date.Add(new TimeSpan(1, 0, 0, -1))); }
        }

        private ResponseType responseType;
        public ResponseType ResponseType
        {
            get { return responseType; }
            set { Set(nameof(ResponseType), ref responseType, value); }
        }

        private string intro;
        public string Intro
        {
            get { return intro; }
            set { Set(nameof(Intro), ref intro, value); }
        }

        private string outro;
        public string Outro
        {
            get { return outro; }
            set { Set(nameof(Outro), ref outro, value); }
        }

        private bool draft;
        public bool Draft
        {
            get { return draft; }
            set { Set(nameof(Draft), ref draft, value); }
        }

        private Guid publishKey;
        public Guid PublishKey
        {
            get { return publishKey; }
            set { Set(nameof(PublishKey), ref publishKey, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        private void OnPropertyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        private bool Set<T>(string propertyName, ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            else
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        internal SurveyInfoDTO ToServiceObject(View view)
        {
            if (ServiceObject == null)
            {
                ServiceObject = new SurveyInfoDTO();
            }
            ServiceObject.OrganizationName = Settings.Default.OrganizationName;
            ServiceObject.OrganizationKey = Settings.Default.WebSurveyKey.Value;
            ServiceObject.SurveyId = SurveyId;
            ServiceObject.SurveyName = Title;
            ServiceObject.StartDate = StartDate;
            ServiceObject.ClosingDate = EndDate;
            ServiceObject.SurveyType = ResponseType.ToEpiInfoValue();
            ServiceObject.IntroductionText = Intro;
            ServiceObject.ExitText = Outro;
            ServiceObject.IsDraftMode = Draft;
            ServiceObject.UserPublishKey = PublishKey;
            ServiceObject.XML = MakeView.MakeView.CreateWebTemplate(view);
            return ServiceObject;
        }
    }
}
