using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;


namespace WPF_Frame_LinqToSql.Views
{
    /// <summary>
    /// Interaction logic for ClientsView.xaml
    /// </summary>
    public partial class ClientsView : UserControl
    {
        Linq2SqlDataClassesDataContext dataContext;
        private string connectionString = ConfigurationManager.ConnectionStrings["WPF_Frame_LinqToSql.Properties.Settings.dbConnectionString"].ConnectionString;

        public ClientsView()
        {
            InitializeComponent();
            RefreshTable();
            PopulateCompanies();
        }

        private void PopulateCompanies()
        {
            cbCompanies.ItemsSource = dataContext.Companies;
        }

        private void RefreshTable()
        {
            dataContext = new Linq2SqlDataClassesDataContext(connectionString);
            mainDataGrid.ItemsSource = dataContext.Clients;
        }
        private void ResetForm()
        {
            tbFirstName.Text = "First Name";
            tbLastName.Text = "Last Name";
            cbCompanies.SelectedItem = null;
        }

        private void tbFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbFirstName.Text == "First Name")
            {
                tbFirstName.Text = "";
            }
        }

        private void tbLastName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbLastName.Text == "Last Name")
            {
                tbLastName.Text = "";
            }
        }

        private void tbFirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbFirstName.Text == "")
            {
                if (mainDataGrid.SelectedItem != null)
                {
                    tbFirstName.Text = ((Client)mainDataGrid.SelectedItem).FirstName;
                }
                else
                {
                    tbFirstName.Text = "First Name";
                }
            }
        }

        private void tbLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbLastName.Text == "")
            {
                if(mainDataGrid.SelectedItem != null)
                {
                    tbLastName.Text = ((Client)mainDataGrid.SelectedItem).LastName;
                } else
                {
                    tbLastName.Text = "Last Name";
                }
            }
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            client.FirstName = tbFirstName.Text;
            client.LastName = tbLastName.Text;
            if(client.FirstName != "First Name" && client.LastName != "Last Name")
            {
                if(cbCompanies.SelectedValue != null)
                {
                    client.CompanyId = (int) cbCompanies.SelectedValue;
                    dataContext.Clients.InsertOnSubmit(client);
                    dataContext.SubmitChanges();

                    RefreshTable();
                    ResetForm();

                } else
                {
                    MessageBox.Show("Please select a company.");
                }
            } else
            {
                MessageBox.Show("Please input the correct first and last names.");
            }

            
        }

        private void mainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mainDataGrid.SelectedItem != null)
            {
                Client client = (Client) mainDataGrid.SelectedItem;

                tbFirstName.Text = client.FirstName;
                tbLastName.Text = client.LastName;
                cbCompanies.SelectedValue = client.Company.Id;
            } else
            {
                ResetForm();
            }

        }

        private void btnUpdateClient_Click(object sender, RoutedEventArgs e)
        {
            if(mainDataGrid.SelectedItem != null)
            {
                Client client = (Client) mainDataGrid.SelectedItem;
                client.FirstName = (string) tbFirstName.Text;
                client.LastName = (string) tbLastName.Text;
                client.Company = dataContext.Companies.Single(c => c.Id == (int)cbCompanies.SelectedValue);
                dataContext.SubmitChanges();
                RefreshTable();
            } else
            {
                MessageBox.Show("Please select a row first.");
            }
        }

        private void btnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if(mainDataGrid.SelectedItems != null)
            {
                foreach (Client c in mainDataGrid.SelectedItems)
                {
                    dataContext.Clients.DeleteOnSubmit(c);
                }
                dataContext.SubmitChanges();
                RefreshTable();
            } else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void btnRefreshClient_Click(object sender, RoutedEventArgs e)
        {
            RefreshTable();
            ResetForm();
        }

        private void cbCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCompanies.SelectedItem != null)
            {
                mainDataGrid.ItemsSource = from c in dataContext.Clients
                                           where c.CompanyId == (int)cbCompanies.SelectedValue
                                           select c;
            }
        }
    }
}
