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
        }
        private void RefreshTable()
        {
            dataContext = new Linq2SqlDataClassesDataContext(connectionString);
            mainDataGrid.ItemsSource = dataContext.Clients;
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
            if (tbLastName.Text == null || tbLastName.Text == "")
            {
                tbLastName.Text = "First Name";
            }
        }

        private void tbLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbLastName.Text == null || tbLastName.Text == "")
            {
                tbLastName.Text = "Last Name";
            }
        }
    }
}
