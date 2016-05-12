﻿using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceListViewModel : ListViewModelBase<ProjectInfo>
    {
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AddExistingCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public DataSourceListViewModel()
        {
            Title = "Data Sources";
            Refresh();
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            };
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            AddNewCommand = new RelayCommand(AddNew);
            AddExistingCommand = new RelayCommand(AddExisting);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<ProjectInfo>>(this, OnRefreshDataSourceList);
        }

        protected override ICollectionView GetItems()
        {
            ICollection<ProjectInfo> items = new List<ProjectInfo>();
            foreach (string path in Settings.Default.DataSources)
            {
                ProjectInfo projectInfo;
                if (ProjectInfo.TryRead(new FileInfo(path), out projectInfo))
                {
                    items.Add(projectInfo);
                }
            }
            return CollectionViewSource.GetDefaultView(items);
        }

        protected override IEnumerable<string> GetFilteredValues(ProjectInfo item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Open()
        {
            Locator.Main.OpenDataSource(SelectedItem.File);
        }

        public void AddNew()
        {
            // TODO: Implement
        }

        public void AddExisting()
        {
            // TODO: Implement
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Remove?",
                "Are you sure you want to remove this data source?",
                "Remove",
                "Don't Remove");
            msg.Confirmed += (sender, e) =>
            {
                int index = Settings.Default.DataSources.FindIndex(
                    dataSource => dataSource.Equals(SelectedItem.File.FullName, StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    Settings.Default.DataSources.Remove(SelectedItem.File.FullName);
                    Settings.Default.Save();
                }
                Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshDataSourceList(RefreshListMessage<ProjectInfo> msg)
        {
            Refresh();
        }
    }
}
