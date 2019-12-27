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
                studentsTable.Rows[1].Range.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
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
                    studentDoc.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatPDF);
                    studentDoc.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatHTML);
                    studentDoc.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatDocument);
                    studentDoc.SaveAs2(tbFileName.Text, Word.WdSaveFormat.wdFormatXMLDocument);
                } catch
                {
                    MessageBox.Show("File was not saved. Make sure the file name is alphanumeric value.");
                } finally
                {

                }
                try
                {
                    studentDoc.Close();
                    studentsDocApplication.Quit();
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
            if(tbFileName.Text == "File Name")
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
    }
}
