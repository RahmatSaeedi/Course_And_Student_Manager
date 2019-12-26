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
    /// Interaction logic for LectureView.xaml
    /// </summary>
    public partial class LectureView : UserControl
    {
        Linq2SqlDataClassesDataContext dataContext;
        private string connectionString = ConfigurationManager.ConnectionStrings["WPF_Frame_LinqToSql.Properties.Settings.dbConnectionString"].ConnectionString;

        private void RefreshTable()
        {
            dataContext = new Linq2SqlDataClassesDataContext(connectionString);
            cbStudent.ItemsSource = dataContext.Clients;
            cbLecture.ItemsSource = dataContext.Lectures;
            lecturesDataGrid.ItemsSource = dataContext.Lectures;
            registrationDataGrid.ItemsSource = joinTables();
        }

        private IQueryable joinTables(string type = "AllRegistrations", int id = 0, int id2 = 0)
        {
            switch (type)
            {
                case "AllRegistrations":
                    return (from cl in dataContext.ClientLectures
                            join c in dataContext.Clients on cl.ClientId equals c.Id
                            join l in dataContext.Lectures on cl.LectureId equals l.Id
                            join comp in dataContext.Companies on c.CompanyId equals comp.Id
                            select new { FirstName = c.FirstName, LastName = c.LastName, LectureName = l.Name, CompanyName = comp.Name, ClientId = c.Id, LectureId = l.Id, CompanyId = comp.Id, ClientLectureId = cl.Id });
                    break;
                case "RegistrationsByClientId":
                    return (from cl in dataContext.ClientLectures
                            join c in dataContext.Clients on cl.ClientId equals c.Id
                            join l in dataContext.Lectures on cl.LectureId equals l.Id
                            join comp in dataContext.Companies on c.CompanyId equals comp.Id
                            where c.Id == id
                            select new { FirstName = c.FirstName, LastName = c.LastName, LectureName = l.Name, CompanyName = comp.Name, ClientId = c.Id });
                    break;
                case "RegistrationsByLectureId":
                    return (from cl in dataContext.ClientLectures
                            join c in dataContext.Clients on cl.ClientId equals c.Id
                            join l in dataContext.Lectures on cl.LectureId equals l.Id
                            join comp in dataContext.Companies on c.CompanyId equals comp.Id
                            where l.Id == id
                            select new { FirstName = c.FirstName, LastName = c.LastName, LectureName = l.Name, CompanyName = comp.Name, ClientId = c.Id });
                    break;
                case "RegistrationsByClientIdByLectureId":
                    return (from cl in dataContext.ClientLectures
                            join c in dataContext.Clients on cl.ClientId equals c.Id
                            join l in dataContext.Lectures on cl.LectureId equals l.Id
                            join comp in dataContext.Companies on c.CompanyId equals comp.Id
                            where c.Id == id
                            where l.Id == id2
                            select new { FirstName = c.FirstName, LastName = c.LastName, LectureName = l.Name, CompanyName = comp.Name, ClientId = c.Id });
                    break;
                default:
                    return null;
            }
        }


        public LectureView()
        {
            InitializeComponent();
            RefreshTable();
        }

        private void tbLectureName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbLectureName.Text == "Lecture Name")
            {
                tbLectureName.Text = "";
            }
        }

        private void tbLectureName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbLectureName.Text == "")
            {
                if (lecturesDataGrid.SelectedItem != null)
                {
                    tbLectureName.Text = ((Lecture)lecturesDataGrid.SelectedItem).Name;
                }
                else
                {
                    tbLectureName.Text = "Lecture Name";
                }
            }
        }

        private void btnAddLecture_Click(object sender, RoutedEventArgs e)
        {
            Lecture newLecture = new Lecture();
            newLecture.Name = tbLectureName.Text;
            if (newLecture.Name != null || newLecture.Name != "" || newLecture.Name != "Lecture Name")
            {
                dataContext.Lectures.InsertOnSubmit(newLecture);
                dataContext.SubmitChanges();

                RefreshTable();
            }
        }


        private void lecturesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lecturesDataGrid.SelectedItem != null)
            {
                tbLectureName.Text = ((Lecture)lecturesDataGrid.SelectedItem).Name;
            }
            else
            {
                tbLectureName.Text = "Lecture Name";
            }

        }

        private void btnUpdateLecture_Click(object sender, RoutedEventArgs e)
        {
            if (lecturesDataGrid.SelectedItem != null)
            {
                Lecture lecture = (Lecture)lecturesDataGrid.SelectedItem;
                lecture.Name = tbLectureName.Text;
                dataContext.SubmitChanges();
            }
            else
            {
                MessageBox.Show("Please select a lecture to update first.");
            }
        }

        private void btnDeleteLecture_Click(object sender, RoutedEventArgs e)
        {
            if (lecturesDataGrid.SelectedItem != null)
            {
                foreach (Lecture l in lecturesDataGrid.SelectedItems)
                {
                    dataContext.Lectures.DeleteOnSubmit(l);
                }
                dataContext.SubmitChanges();
                RefreshTable();
            }
        }

        private void btnRegisterClient_Click(object sender, RoutedEventArgs e)
        {
            if (cbStudent.SelectedItem != null && cbLecture.SelectedItem != null)
            {
                ClientLecture cl = new ClientLecture();
                cl.Client = (Client)cbStudent.SelectedItem;
                cl.Lecture = (Lecture)cbLecture.SelectedItem;

                IQueryable<ClientLecture> duplicateCL = from dcl in dataContext.ClientLectures where dcl.Client == cl.Client where dcl.Lecture == cl.Lecture select dcl;
                if (duplicateCL.Count() == 0)
                {
                    dataContext.ClientLectures.InsertOnSubmit(cl);
                    dataContext.SubmitChanges();
                    RefreshTable();
                }
                else
                {
                    MessageBox.Show("Student is already registered in that lecture.");
                }
            }
            else
            {
                MessageBox.Show("Please select a student and a lecture to register.");
            }
        }

        private void btnUnregisterClient_Click(object sender, RoutedEventArgs e)
        {
            if (cbStudent.SelectedItem != null && cbLecture.SelectedItem != null)
            {
                try
                {
                    ClientLecture cl = (from cL in dataContext.ClientLectures where cL.ClientId == (int)cbStudent.SelectedValue where cL.LectureId == (int)cbLecture.SelectedValue select cL).First();

                    dataContext.ClientLectures.DeleteOnSubmit(cl);
                    dataContext.SubmitChanges();
                    RefreshTable();
                }
                catch
                {
                    MessageBox.Show("Student not found in that lecture.");
                }
            }
            else
            {
                MessageBox.Show("Please select a student and a lecture to unregister.");
            }
        }

        private void cbStudent_cbLecture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLecture.SelectedItem == null && cbStudent.SelectedItem == null)
            {
                registrationDataGrid.ItemsSource = joinTables();
            }
            else if (cbLecture.SelectedItem == null && cbStudent.SelectedItem != null)
            {
                registrationDataGrid.ItemsSource = joinTables("RegistrationsByClientId", (int)cbStudent.SelectedValue);
            }
            else if (cbLecture.SelectedItem != null && cbStudent.SelectedItem == null)
            {
                registrationDataGrid.ItemsSource = joinTables("RegistrationsByLectureId", (int)cbLecture.SelectedValue);
            }
            else if (cbLecture.SelectedItem != null && cbStudent.SelectedItem != null)
            {
                registrationDataGrid.ItemsSource = joinTables("RegistrationsByClientIdByLectureId", (int)cbStudent.SelectedValue, (int)cbLecture.SelectedValue);
            }
        }

        private void btnResetClient_Click(object sender, RoutedEventArgs e)
        {
            cbStudent.SelectedItem = null;
            cbLecture.SelectedItem = null;
        }

        private void registrationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (registrationDataGrid.SelectedValue != null)
            {
                cbStudent.SelectedValue = registrationDataGrid.SelectedValue;
            }
        }
    }
}
