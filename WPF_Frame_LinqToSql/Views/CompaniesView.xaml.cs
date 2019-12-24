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
    /// Interaction logic for CompaniesView.xaml
    /// </summary>
    public partial class CompaniesView : UserControl
    {
        Linq2SqlDataClassesDataContext dataContext;
        private string connectionString = ConfigurationManager.ConnectionStrings["WPF_Frame_LinqToSql.Properties.Settings.dbConnectionString"].ConnectionString;

        public CompaniesView()
        {
            InitializeComponent();
            RefreshTable();
        }

        private void RefreshTable()
        {
            dataContext = new Linq2SqlDataClassesDataContext(connectionString);
            mainDataGrid.ItemsSource = dataContext.Companies;
        }

        private void tbCompanyName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbCompanyName.Text == "Company Name")
            {
                tbCompanyName.Text = "";
            }
        }

        private void tbCompanyName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbCompanyName.Text == "")
            {
                tbCompanyName.Text = "Company Name";
            }
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            Company newCompany = new Company();
            newCompany.Name = tbCompanyName.Text;
            if (newCompany.Name != null || newCompany.Name != "" || newCompany.Name != "Company Name")
            {
                dataContext.Companies.InsertOnSubmit(newCompany);
                dataContext.SubmitChanges();

                RefreshTable();
            }

        }

        private void btnRefreshCompany_Click(object sender, RoutedEventArgs e)
        {
            RefreshTable();
        }

        private void mainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainDataGrid.SelectedItem != null)
            {
                tbCompanyName.Text = ((Company)mainDataGrid.SelectedItem).Name;
            }

        }

        private void btnUpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            if(((Company)mainDataGrid.SelectedItem).Name != tbCompanyName.Text && tbCompanyName.Text != null && tbCompanyName.Text != "")
            {
                IQueryable<Company> company = from c in dataContext.Companies where c.Id == (int)mainDataGrid.SelectedValue select c;
                company.First().Name = tbCompanyName.Text;
                dataContext.SubmitChanges();
            }
        }

        private void btnDeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            if(mainDataGrid.SelectedItem != null)
            {
                foreach (Company c in mainDataGrid.SelectedItems)
                {
                    dataContext.Companies.DeleteOnSubmit(c);
                }
                dataContext.SubmitChanges();
                RefreshTable();
            }
        }
    }
}
