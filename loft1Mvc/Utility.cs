using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StockManagement
{
    public static class Utility
    {
        internal static async Task<MemoryStream> GetFileContent(dynamic list, Type tipoLista, string sWebRootFolder, string sFileName)
        {
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Differenza ordinato venduto");
                IRow row = excelSheet.CreateRow(0);

                var columns = tipoLista.GetProperties()
                        .Select(property => property.Name)
                        .ToList();

                for (int i = 0; i < columns.Count - 1; i++)
                {
                    row.CreateCell(i).SetCellValue(columns[i]);
                }

                foreach (var articolo in list)
                {
                    row = excelSheet.CreateRow(list.IndexOf(articolo) + 1);
                    var arrayOfProperty = new List<PropertyInfo>(articolo.GetType().GetProperties());

                    foreach (PropertyInfo prop in arrayOfProperty)
                    {
                        if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                        {
                            row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(articolo).ToString());
                        }
                    }
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }
        internal static async Task<MemoryStream> GetFilesCombinedContent(dynamic list, Type tipoLista, dynamic list2, Type tipoLista2, string sWebRootFolder, string sFileName)
        {
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Report");
                IRow row = excelSheet.CreateRow(0);

                var columns = tipoLista.GetProperties()
                        .Select(property => property.Name)
                        .ToList();

                for (int i = 0; i < columns.Count - 1; i++)
                {
                    row.CreateCell(i).SetCellValue(columns[i]);
                }

                foreach (var item in list)
                {
                    row = excelSheet.CreateRow(list.IndexOf(item) + 1);
                    var arrayOfProperty = new List<PropertyInfo>(item.GetType().GetProperties());

                    foreach (PropertyInfo prop in arrayOfProperty)
                    {
                        if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                        {
                            row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(item).ToString());
                        }
                    }
                }

                row = excelSheet.CreateRow(list.Count + 2);

                var columns2 = tipoLista2.GetProperties()
                        .Select(property => property.Name)
                        .ToList();

                for (int i = 0; i < columns2.Count - 1; i++)
                {
                    row.CreateCell(i).SetCellValue(columns[i]);
                }

                int rowIndex = list.Count + 3;
                foreach (var item in list2)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    var arrayOfProperty = new List<PropertyInfo>(item.GetType().GetProperties());

                    foreach (PropertyInfo prop in arrayOfProperty)
                    {
                        if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                        {
                            row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(item).ToString());
                        }
                    }
                    rowIndex++;
                }


                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }

    }
}
