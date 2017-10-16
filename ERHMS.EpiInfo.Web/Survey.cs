using Epi;
using Epi.SurveyManagerService;
using ERHMS.EpiInfo.Wrappers;
using System;
using System.ComponentModel;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo.Web
{
    public class Survey : INotifyPropertyChanged
    {
        private string surveyId;
        public string SurveyId
        {
            get { return surveyId; }
            set { SetProperty(nameof(SurveyId), ref surveyId, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(nameof(Title), ref title, value); }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(nameof(StartDate), ref startDate, value.Date); }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(nameof(EndDate), ref endDate, value.Date.Add(new TimeSpan(1, 0, 0, -1))); }
        }

        private ResponseType responseType;
        public ResponseType ResponseType
        {
            get { return responseType; }
            set { SetProperty(nameof(ResponseType), ref responseType, value); }
        }

        private string intro;
        public string Intro
        {
            get { return intro; }
            set { SetProperty(nameof(Intro), ref intro, value); }
        }

        private string outro;
        public string Outro
        {
            get { return outro; }
            set { SetProperty(nameof(Outro), ref outro, value); }
        }

        private bool draft;
        public bool Draft
        {
            get { return draft; }
            set { SetProperty(nameof(Draft), ref draft, value); }
        }

        private Guid publishKey;
        public Guid PublishKey
        {
            get { return publishKey; }
            set { SetProperty(nameof(PublishKey), ref publishKey, value); }
        }

        public Survey() { }

        internal Survey(SurveyInfoDTO info)
        {
            SurveyId = info.SurveyId;
            Title = info.SurveyName;
            StartDate = info.StartDate;
            EndDate = info.ClosingDate;
            ResponseType = ResponseTypeExtensions.EpiInfoValues.Reverse(info.SurveyType);
            Intro = info.IntroductionText;
            Outro = info.ExitText;
            Draft = info.IsDraftMode;
            PublishKey = info.UserPublishKey;
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

        private bool SetProperty<T>(string name, ref T field, T value)
        {
            if (Equals(value, field))
            {
                return false;
            }
            else
            {
                field = value;
                OnPropertyChanged(name);
                return true;
            }
        }

        internal SurveyInfoDTO GetInfo(View view)
        {
            Wrapper wrapper = MakeView.CreateWebTemplate.Create(view.Project.FilePath, view.Name);
            wrapper.Invoke();
            return new SurveyInfoDTO
            {
                OrganizationName = Settings.Default.OrganizationName,
                OrganizationKey = new Guid(Settings.Default.OrganizationKey),
                SurveyId = SurveyId,
                SurveyName = Title,
                StartDate = StartDate,
                ClosingDate = EndDate,
                SurveyType = ResponseTypeExtensions.EpiInfoValues.Forward(ResponseType),
                IntroductionText = Intro,
                ExitText = Outro,
                IsDraftMode = Draft,
                UserPublishKey = PublishKey,
                XML = wrapper.ReadToEnd()
            };
        }
    }
}
