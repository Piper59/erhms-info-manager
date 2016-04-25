using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using ERHMS.EpiInfo.Domain;
using ERHMS.DataAccess;
using ERHMS.Domain;
using System.Windows.Controls;
using ERHMS.EpiInfo.DataAccess;
using System.Windows.Data;
using ERHMS.EpiInfo.Enter;

namespace ERHMS.Presentation.ViewModel
{
    public class FormResponseListViewModel : ViewModelBase
    {
        private Epi.View CurrentView;

        private ViewEntityRepository<ViewEntity> viewEntityRepository { get; set; }

        public ObservableCollection<DataGridColumn> ColumnCollection { get; private set; }

        private IEnumerable<ViewEntity> responses;
        public IEnumerable<ViewEntity> Responses
        {
            get { return responses; }
            set { Set(() => Responses, ref responses, value); }
        }

        private ViewEntity selectedResponse;
        public ViewEntity SelectedResponse
        {
            get { return selectedResponse; }
            set { Set(() => SelectedResponse, ref selectedResponse, value); }
        }
        
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand UndeleteCommand { get; private set; }

        public FormResponseListViewModel(Epi.View view)
        {
            ViewEntityRepository<ViewEntity> viewEntityRepository = new ViewEntityRepository<ViewEntity>(App.GetDataContext().Driver, view);

            ColumnCollection = new ObservableCollection<DataGridColumn>();
            
            EditCommand = new RelayCommand(() => Enter.OpenRecord(view, (int)SelectedResponse.UniqueKey), HasSelectedResponse);
            //DeleteCommand = new RelayCommand(() => Responses.Delete(SelectedResponse), HasDeletableSelectedResponse);
            //UndeleteCommand = new RelayCommand(() => Responses.Undelete(SelectedResponse), HasUndeletableSelectedResponse);

            CurrentView = view;

            Responses = viewEntityRepository.Select();

            foreach (string name in view.Fields.DataFields.Names)
            {
                ColumnCollection.Add(new DataGridTextColumn
                {
                    Header = name,
                    Binding = new Binding(name)
                });
            }
        }

        private bool HasSelectedResponse()
        {
            return SelectedResponse != null;
        }

        private bool HasDeletableSelectedResponse()
        {
            return HasSelectedResponse() && !SelectedResponse.Deleted;
        }

        private bool HasUndeletableSelectedResponse()
        {
            return HasSelectedResponse() && SelectedResponse.Deleted;
        }
    }
}