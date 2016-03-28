/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ERHMS.WPF.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ERHMS.WPF.Model;
using System;

namespace ERHMS.WPF.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StartViewModel>();
            SimpleIoc.Default.Register<ResponderViewModel>();
            SimpleIoc.Default.Register<ResponderSearchViewModel>();
            //SimpleIoc.Default.Register<FormListViewModel>();
            SimpleIoc.Default.Register<TemplateListViewModel>();

        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the Start Page property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StartViewModel StartiewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartViewModel>(Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Gets the ResponderViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ResponderViewModel ResponderViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ResponderViewModel>(Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Gets the ResponderSearchViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ResponderSearchViewModel ResponderSearchViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ResponderSearchViewModel>(Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Gets the FormListViewModel property.
        /// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        //    "CA1822:MarkMembersAsStatic",
        //    Justification = "This non-static member is needed for data binding purposes.")]
        //public FormListViewModel FormListViewModel
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<FormListViewModel>(Guid.NewGuid().ToString());
        //    }
        //}

        /// <summary>
        /// Gets the TemplateListViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TemplateListViewModel TemplateListViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TemplateListViewModel>(Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}