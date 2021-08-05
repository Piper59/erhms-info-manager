using ERHMS.Common.ComponentModel;
using ERHMS.Data.SqlServer;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace ERHMS.Desktop.ViewModels.Shared
{
    public abstract class ConnectionInfoViewModel : ObservableObject
    {
        public abstract bool Editable { get; }

        public abstract bool Validate();
        public abstract string GetConnectionString(string projectPath);

        public abstract class Access : ConnectionInfoViewModel
        {
            public override bool Editable => false;
            protected abstract string Provider { get; }
            protected abstract string FileExtension { get; }

            public override bool Validate()
            {
                return true;
            }

            public override string GetConnectionString(string projectPath)
            {
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
                {
                    Provider = Provider,
                    DataSource = Path.ChangeExtension(projectPath, FileExtension)
                };
                return builder.ConnectionString;
            }
        }

        public class Access2003 : Access
        {
            protected override string Provider => "Microsoft.Jet.OLEDB.4.0";
            protected override string FileExtension => ".mdb";
        }

        public class Access2007 : Access
        {
            protected override string Provider => "Microsoft.ACE.OLEDB.12.0";
            protected override string FileExtension => ".accdb";
        }

        public class SqlServer : ConnectionInfoViewModel
        {
            public override bool Editable => true;

            private string dataSource;
            public string DataSource
            {
                get { return dataSource; }
                set { SetProperty(ref dataSource, value); }
            }

            private string initialCatalog;
            public string InitialCatalog
            {
                get { return initialCatalog; }
                set { SetProperty(ref initialCatalog, value); }
            }

            private bool encrypt;
            public bool Encrypt
            {
                get { return encrypt; }
                set { SetProperty(ref encrypt, value); }
            }

            public ListCollectionView<AuthenticationMode> AuthenticationModes { get; }
            public bool CanHaveCredentials => AuthenticationModes.CurrentItem == AuthenticationMode.SqlServer;

            private string userID;
            public string UserID
            {
                get { return userID; }
                set { SetProperty(ref userID, value); }
            }

            private string password;
            public string Password
            {
                get { return password; }
                set { SetProperty(ref password, value); }
            }

            public SqlServer()
            {
                AuthenticationModes = new ListCollectionView<AuthenticationMode>(
                    AuthenticationMode.Windows,
                    AuthenticationMode.SqlServer);
                AuthenticationModes.CurrentChanged += AuthenticationModes_CurrentChanged;
            }

            private void AuthenticationModes_CurrentChanged(object sender, EventArgs e)
            {
                if (AuthenticationModes.CurrentItem == AuthenticationMode.Windows)
                {
                    UserID = null;
                    Password = null;
                }
                OnPropertyChanged(nameof(CanHaveCredentials));
            }

            public override bool Validate()
            {
                if (string.IsNullOrEmpty(DataSource) || string.IsNullOrEmpty(InitialCatalog))
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = Strings.Lead_InvalidConnectionInfo;
                    dialog.Body = Strings.Body_InvalidConnectionInfo_SqlServer;
                    dialog.Buttons = DialogButtonCollection.Close;
                    dialog.Show();
                    return false;
                }
                return true;
            }

            public override string GetConnectionString(string projectPath)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = DataSource,
                    InitialCatalog = InitialCatalog
                };
                if (Encrypt)
                {
                    builder.Encrypt = true;
                }
                if (AuthenticationModes.CurrentItem == AuthenticationMode.Windows)
                {
                    builder.IntegratedSecurity = true;
                }
                else if (AuthenticationModes.CurrentItem == AuthenticationMode.SqlServer)
                {
                    if (!string.IsNullOrEmpty(UserID))
                    {
                        builder.UserID = UserID;
                    }
                    if (!string.IsNullOrEmpty(Password))
                    {
                        builder.Password = Password;
                    }
                }
                return builder.ConnectionString;
            }
        }
    }
}
