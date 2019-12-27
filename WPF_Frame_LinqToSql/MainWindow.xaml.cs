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
using WPF_Frame_LinqToSql.ViewModels;

namespace WPF_Frame_LinqToSql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new QuestionsViewModel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new HomeViewModel();
        }

        private void btnCompanies_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new CompaniesViewModel();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new ClientsViewModel();
        }

        private void btnLectures_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new LecturesViewModel();
        }

        private void btnQuestions_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new QuestionsViewModel();
        }
    }
}
