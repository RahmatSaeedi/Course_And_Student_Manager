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
using Word = Microsoft.Office.Interop.Word;
using Microsoft.VisualBasic;

namespace WPF_Frame_LinqToSql.Views
{
    /// <summary>
    /// Interaction logic for QuestionsView.xaml
    /// </summary>
    /// 

    public partial class QuestionsView : UserControl
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["WPF_Frame_LinqToSql.Properties.Settings.dbConnectionString"].ConnectionString;
        Linq2SqlDataClassesDataContext dataContext;
        public QuestionsView()
        {
            InitializeComponent();
            dataContext = new Linq2SqlDataClassesDataContext(connectionString);

        }

        private void btnStudents_Click(object sender, RoutedEventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Word.Application studentsDocApplication = null;
            Word.Document studentDoc = null;

            try
            {
                studentsDocApplication = new Word.Application();
                studentsDocApplication.Visible = true;
                studentsDocApplication.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                studentDoc = studentsDocApplication.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (studentsDocApplication != null && studentDoc != null)
            {
                studentDoc.PageSetup.TopMargin = 0;
                studentDoc.PageSetup.BottomMargin = 0;
                studentDoc.PageSetup.LeftMargin = 0;
                studentDoc.PageSetup.RightMargin = 0;

                Word.Table studentsTable;
                Word.Range wrdRang = studentsDocApplication.ActiveDocument.Bookmarks.get_Item(ref oEndOfDoc).Range;
                studentsTable = studentDoc.Tables.Add(wrdRang, 1, 4, ref oMissing, ref oMissing);

                studentsTable.Rows[1].Cells[1].Range.Text = "Student ID";
                studentsTable.Rows[1].Cells[2].Range.Text = "First Name";
                studentsTable.Rows[1].Cells[3].Range.Text = "Last Name";
                studentsTable.Rows[1].Cells[4].Range.Text = "Company Name";
                studentsTable.Rows[1].Shading.BackgroundPatternColor = (Word.WdColor)0x47AD70;
                studentsTable.Rows[1].Range.Font.Color = Word.WdColor.wdColorWhite;
                studentsTable.Rows[1].Range.Font.Bold = 1;
                studentsTable.Rows[1].Range.Font.Italic = 1;
                studentsTable.Rows[1].Range.Font.Size = 16;
                studentsTable.Borders[Word.WdBorderType.wdBorderLeft].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                studentsTable.Borders[Word.WdBorderType.wdBorderRight].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                studentsTable.Borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                studentsTable.Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;

                studentsTable.Borders[Word.WdBorderType.wdBorderLeft].Color = (Word.WdColor)0x47AD70; //WdColor is BlueGreenRed rather than RGB
                studentsTable.Borders[Word.WdBorderType.wdBorderRight].Color = (Word.WdColor)0x47AD70;
                studentsTable.Borders[Word.WdBorderType.wdBorderTop].Color = (Word.WdColor)0x47AD70;
                studentsTable.Borders[Word.WdBorderType.wdBorderBottom].Color = (Word.WdColor)0x47AD70;

                int rowCount = 2;
                IQueryable students = from c in dataContext.Clients
                                      join comp in dataContext.Companies on c.CompanyId equals comp.Id
                                      select new Students(c.Id.ToString(), c.FirstName, c.LastName, comp.Name);




                foreach (Students s in students)
                {
                    studentsTable.Rows.Add(ref oMissing);
                    if (rowCount == 2)
                    {
                        studentsTable.Rows[rowCount].Range.Font.Bold = 0;
                        studentsTable.Rows[rowCount].Range.Font.Italic = 0;
                        studentsTable.Rows[rowCount].Range.Font.Size = 12;
                        studentsTable.Rows[rowCount].Range.Font.Underline = 0;
                        studentsTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                        studentsTable.Rows[rowCount].Range.Font.Color = Word.WdColor.wdColorBlack;
                    }
                    else if (rowCount % 2 == 0)
                    {
                        studentsTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                    }
                    else
                    {
                        studentsTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xEFFFEF;
                    }
                    studentsTable.Rows[rowCount].Cells[1].Range.Text = s.StudentId;
                    studentsTable.Rows[rowCount].Cells[2].Range.Text = s.FirstName;
                    studentsTable.Rows[rowCount].Cells[3].Range.Text = s.LastName;
                    studentsTable.Rows[rowCount].Cells[4].Range.Text = s.CompanyName;

                    rowCount++;
                }
                try
                {
                    studentDoc.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatXMLDocument);
                } catch
                {
                    MessageBox.Show("File was not saved. Make sure the file name is alphanumeric value.");
                }
                try
                {
                    //studentDoc.Close();
                    //studentsDocApplication.Quit();
                } catch
                {
                    MessageBox.Show("Word document did not close successfully.");
                }
            }
        }
        private class Students
        {
            internal Students(string StudentId, string FirstName, string LastName, string CompanyName)
            {
                this.StudentId = StudentId;
                this.FirstName = FirstName;
                this.LastName = LastName;
                this.CompanyName = CompanyName;
            }
            public string StudentId { set; get; }
            public string FirstName { set; get; }
            public string LastName { set; get; }
            public string CompanyName { set; get; }
        }

        private void tbFileName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbFileName.Text == "File Name")
            {
                tbFileName.Text = "";
            }
        }

        private void tbFileName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbFileName.Text == "")
            {
                tbFileName.Text = "File Name";
            }
        }

        private void btnCompanies_Click(object sender, RoutedEventArgs e)
        {
            object oEndOfDoc = "\\endofdoc";

            Word.Application companiesApplication = null;
            Word.Document companiesDocument = null;
            try
            {
                companiesApplication = new Word.Application();
                companiesApplication.Visible = true;
                companiesApplication.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                companiesDocument = companiesApplication.Documents.Add();
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message);
            }

            if (companiesApplication != null && companiesDocument != null)
            {
                companiesDocument.PageSetup.TopMargin = 0;
                companiesDocument.PageSetup.RightMargin = 0;
                companiesDocument.PageSetup.BottomMargin = 0;
                companiesDocument.PageSetup.LeftMargin = 0;

                Word.Table companiesTable;
                Word.Range wrdRange = companiesApplication.ActiveDocument.Bookmarks.get_Item(ref oEndOfDoc).Range;
                companiesTable = companiesDocument.Tables.Add(wrdRange, 1, 2);

                companiesTable.Rows[1].Cells[1].Range.Text = "Business ID";
                companiesTable.Rows[1].Cells[2].Range.Text = "Company Name";

                Word.Range tableHeaderRange = companiesTable.Rows[1].Range;

                tableHeaderRange.Shading.BackgroundPatternColor = (Word.WdColor)0x47AD70;
                tableHeaderRange.Font.Color = Word.WdColor.wdColorWhite;
                tableHeaderRange.Font.Bold = 1;
                tableHeaderRange.Font.Italic = 1;
                tableHeaderRange.Font.Size = 16;

                companiesTable.Borders[Word.WdBorderType.wdBorderLeft].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                companiesTable.Borders[Word.WdBorderType.wdBorderRight].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                companiesTable.Borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                companiesTable.Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;

                companiesTable.Borders[Word.WdBorderType.wdBorderLeft].Color = (Word.WdColor)0x47AD70;
                companiesTable.Borders[Word.WdBorderType.wdBorderRight].Color = (Word.WdColor)0x47AD70;
                companiesTable.Borders[Word.WdBorderType.wdBorderTop].Color = (Word.WdColor)0x47AD70;
                companiesTable.Borders[Word.WdBorderType.wdBorderBottom].Color = (Word.WdColor)0x47AD70;

                int rowCount = 2;

                foreach (Company c in dataContext.Companies)
                {
                    companiesTable.Rows.Add();
                    if (rowCount == 2)
                    {
                        companiesTable.Rows[rowCount].Range.Font.Bold = 0;
                        companiesTable.Rows[rowCount].Range.Font.Italic = 0;
                        companiesTable.Rows[rowCount].Range.Font.Size = 12;
                        companiesTable.Rows[rowCount].Range.Font.Underline = 0;
                        companiesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                        companiesTable.Rows[rowCount].Range.Font.Color = Word.WdColor.wdColorBlack;
                    }
                    else if (rowCount % 2 == 0)
                    {
                        companiesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                    }
                    else
                    {
                        companiesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xEFFFEF;
                    }
                    companiesTable.Rows[rowCount].Cells[1].Range.Text = c.Id.ToString();
                    companiesTable.Rows[rowCount].Cells[2].Range.Text = c.Name;

                    rowCount++;
                }
                try
                {
                    companiesDocument.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatXMLDocument);
                } catch (Exception exp)
                {
                    MessageBox.Show("Could not save the document. " + exp.Message);
                }
                try
                {
                    //companiesDocument.Close();
                    //companiesApplication.Quit();
                } catch (Exception exp)
                {
                    MessageBox.Show("Could not close and exit the application. " + exp.Message);
                }
            }

        }

        private void btnLectures_Click(object sender, RoutedEventArgs e)
        {
            object oEndOfDoc = "\\endofdoc";
            Word.Application lecturesApplication = null;
            Word.Document lecturesDocumnt = null;


            try
            {
                lecturesApplication = new Word.Application();
                lecturesApplication.Visible = true;
                lecturesApplication.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                lecturesDocumnt = lecturesApplication.Documents.Add();
            } catch (Exception exp)
            {
                MessageBox.Show("Could not create the document. " + exp.Message);
            }

            if (lecturesApplication != null && lecturesDocumnt != null)
            {
                lecturesDocumnt.PageSetup.TopMargin = 0;
                lecturesDocumnt.PageSetup.RightMargin = 0;
                lecturesDocumnt.PageSetup.BottomMargin = 0;
                lecturesDocumnt.PageSetup.LeftMargin = 0;

                Word.Table regTable;
                Word.Range wrdRange = lecturesApplication.ActiveDocument.Bookmarks.get_Item(ref oEndOfDoc).Range;
                regTable = lecturesDocumnt.Tables.Add(wrdRange, 1, 3);

                regTable.Rows[1].Cells[1].Range.Text = "Lecture ID";
                regTable.Rows[1].Cells[2].Range.Text = "Lecture Name";

                Word.Range tableHeaderRange = regTable.Rows[1].Range;

                tableHeaderRange.Shading.BackgroundPatternColor = (Word.WdColor)0x47AD70;
                tableHeaderRange.Font.Color = Word.WdColor.wdColorWhite;
                tableHeaderRange.Font.Bold = 1;
                tableHeaderRange.Font.Italic = 1;
                tableHeaderRange.Font.Size = 16;

                regTable.Borders[Word.WdBorderType.wdBorderLeft].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                regTable.Borders[Word.WdBorderType.wdBorderRight].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                regTable.Borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                regTable.Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;

                regTable.Borders[Word.WdBorderType.wdBorderLeft].Color = (Word.WdColor)0x47AD70;
                regTable.Borders[Word.WdBorderType.wdBorderRight].Color = (Word.WdColor)0x47AD70;
                regTable.Borders[Word.WdBorderType.wdBorderTop].Color = (Word.WdColor)0x47AD70;
                regTable.Borders[Word.WdBorderType.wdBorderBottom].Color = (Word.WdColor)0x47AD70;

                int rowCount = 2;

                foreach (Lecture l in dataContext.Lectures)
                {
                    regTable.Rows.Add();
                    if (rowCount == 2)
                    {
                        regTable.Rows[rowCount].Range.Font.Bold = 0;
                        regTable.Rows[rowCount].Range.Font.Italic = 0;
                        regTable.Rows[rowCount].Range.Font.Size = 12;
                        regTable.Rows[rowCount].Range.Font.Underline = 0;
                        regTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                        regTable.Rows[rowCount].Range.Font.Color = Word.WdColor.wdColorBlack;
                    }
                    else if (rowCount % 2 == 0)
                    {
                        regTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                    }
                    else
                    {
                        regTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xEFFFEF;
                    }
                    regTable.Rows[rowCount].Cells[1].Range.Text = l.Id.ToString();
                    regTable.Rows[rowCount].Cells[2].Range.Text = l.Name;

                    rowCount++;
                }
                try
                {
                    lecturesDocumnt.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatXMLDocument);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Could not save the document. " + exp.Message);
                }
                try
                {
                    //lecturesDocumnt.Close();
                    //lecturesApplication.Quit();
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Could not close and exit the application. " + exp.Message);
                }

            }

        }

        private void btnRegistrations_Click(object sender, RoutedEventArgs e)
        {
            object oEndOfDoc = "\\endofdoc";
            Word.Application regApplication = null;
            Word.Document regDocument = null;


            try
            {
                regApplication = new Word.Application();
                regApplication.Visible = true;
                regApplication.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                regDocument = regApplication.Documents.Add();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Could not create the document. " + exp.Message);
            }

            if (regApplication != null && regDocument != null)
            {
                regDocument.PageSetup.TopMargin = 0;
                regDocument.PageSetup.RightMargin = 0;
                regDocument.PageSetup.BottomMargin = 0;
                regDocument.PageSetup.LeftMargin = 0;

                Word.Table lecturesTable;
                Word.Range wrdRange = regApplication.ActiveDocument.Bookmarks.get_Item(ref oEndOfDoc).Range;
                lecturesTable = regDocument.Tables.Add(wrdRange, 1, 2);

                lecturesTable.Rows[1].Cells[1].Range.Text = "Full Name";
                lecturesTable.Rows[1].Cells[2].Range.Text = "Lecture Name";

                Word.Range tableHeaderRange = lecturesTable.Rows[1].Range;

                tableHeaderRange.Shading.BackgroundPatternColor = (Word.WdColor)0x47AD70;
                tableHeaderRange.Font.Color = Word.WdColor.wdColorWhite;
                tableHeaderRange.Font.Bold = 1;
                tableHeaderRange.Font.Italic = 1;
                tableHeaderRange.Font.Size = 16;

                lecturesTable.Borders[Word.WdBorderType.wdBorderLeft].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                lecturesTable.Borders[Word.WdBorderType.wdBorderRight].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                lecturesTable.Borders[Word.WdBorderType.wdBorderTop].LineStyle = Word.WdLineStyle.wdLineStyleSingle;
                lecturesTable.Borders[Word.WdBorderType.wdBorderBottom].LineStyle = Word.WdLineStyle.wdLineStyleSingle;

                lecturesTable.Borders[Word.WdBorderType.wdBorderLeft].Color = (Word.WdColor)0x47AD70;
                lecturesTable.Borders[Word.WdBorderType.wdBorderRight].Color = (Word.WdColor)0x47AD70;
                lecturesTable.Borders[Word.WdBorderType.wdBorderTop].Color = (Word.WdColor)0x47AD70;
                lecturesTable.Borders[Word.WdBorderType.wdBorderBottom].Color = (Word.WdColor)0x47AD70;

                int rowCount = 2;

                IQueryable registrations = from cl in dataContext.ClientLectures
                                           join c in dataContext.Clients on cl.ClientId equals c.Id
                                           join l in dataContext.Lectures on cl.LectureId equals l.Id
                                           select new Registration(c.FirstName, c.LastName, l.Name);

                foreach (Registration r in registrations)
                {
                    lecturesTable.Rows.Add();
                    if (rowCount == 2)
                    {
                        lecturesTable.Rows[rowCount].Range.Font.Bold = 0;
                        lecturesTable.Rows[rowCount].Range.Font.Italic = 0;
                        lecturesTable.Rows[rowCount].Range.Font.Size = 12;
                        lecturesTable.Rows[rowCount].Range.Font.Underline = 0;
                        lecturesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                        lecturesTable.Rows[rowCount].Range.Font.Color = Word.WdColor.wdColorBlack;
                    }
                    else if (rowCount % 2 == 0)
                    {
                        lecturesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xFFFFFF;
                    }
                    else
                    {
                        lecturesTable.Rows[rowCount].Shading.BackgroundPatternColor = (Word.WdColor)0xEFFFEF;
                    }
                    lecturesTable.Rows[rowCount].Cells[1].Range.Text = r.FullName;
                    lecturesTable.Rows[rowCount].Cells[2].Range.Text = r.LectureName;

                    rowCount++;
                }
                try
                {
                    regDocument.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatXMLDocument);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Could not save the document. " + exp.Message);
                }
                try
                {
                    //regDocument.Close();
                    //regApplication.Quit();
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Could not close and exit the application. " + exp.Message);
                }
            }
        }

        class Registration {
            internal Registration(string FirstName, string LastName, string LectureName)
            {
                this.FirstName = FirstName;
                this.LastName = LastName;
                this.LectureName = LectureName;
            }
            internal string FirstName { get; set; }
            internal string LastName { get; set; }
            internal string LectureName { get; set; }
            internal string FullName
            {
                get
                {
                    return (FirstName + " " + LastName);
                }
            }
        }
    }
}
