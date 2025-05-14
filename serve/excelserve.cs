using Insertion_back_loss.SQL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Insertion_back_loss.serve
{
    class excelserve
    {
        public excelserve()
        {
            ExcelPackage.License.SetNonCommercialPersonal("My Name");//个人
        }
        Dao dao = new Dao();

        public void ExportToExcel()
        {

            using (SqlConnection conn = dao.Connection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM t_ILRL", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // 创建Excel文件
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

                        // 写入列头
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = reader.GetName(i);
                            if (reader.GetFieldType(i) == typeof(DateTime))
                            {
                                worksheet.Column(i + 1).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                            }
                        }

                        // 写入数据行
                        int row = 2;
                        while (reader.Read())
                        {
                            for (int col = 0; col < reader.FieldCount; col++)
                            {
                                worksheet.Cells[row, col + 1].Value = reader[col];
                            }
                            row++;
                        }

                        // 自动调整列宽
                        worksheet.Cells.AutoFitColumns();
                        string lujing = AaveExcel();

                        // 保存文件
                        FileInfo file = new FileInfo(lujing+@"\插回损测试数据.xlsx");
                        try
                        {
                            package.SaveAs(file);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("文件保存失败，请关闭文件后重试！" + ex.Message);
                            return;
                        }
                        MessageBox.Show("数据导出成功，文件路径：" + file.FullName);
                    }
                }
            }
        }
        private string AaveExcel()
        {

            string folderPath = "";
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "选择License文件夹";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog.SelectedPath;
                return folderPath;
            }
            else
            {
                return null;
            }


        }
    }
}

