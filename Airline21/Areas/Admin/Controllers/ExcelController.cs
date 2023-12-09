using Airline21.Models;
using System.Data;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;


namespace Airline21.Areas.Admin.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Admin/Excel       
            public ActionResult ExcelExport()
            {
            Entities1 db = new Entities1();
            List<Ticket> FileData = db.Tickets.ToList();

                try
                {
                    // Sample data (replace this with your actual data)
                    List<Ticket> data = GetDataFromDatabase();

                    // Create a new workbook and worksheet
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Sheet1");

                        // Add headers
                        worksheet.Cell(1, 1).Value = "ticketID";
                        worksheet.Cell(1, 2).Value = "IdFlight";
                        worksheet.Cell(1, 3).Value = "IdType";
                        worksheet.Cell(1, 4).Value = "price";
                        worksheet.Cell(1, 5).Value = "status";
                        worksheet.Cell(1, 6).Value = "status";
                        worksheet.Cell(1, 7).Value = "created_at";
                        worksheet.Cell(1, 8).Value = "updated_at";
                        worksheet.Cell(1, 9).Value = "luggage";
                        worksheet.Cell(1, 10).Value = "Handluggage";

                        // Add data
                        int row = 2;
                        foreach (var item in data)
                        {

                            worksheet.Cell(row, 1).Value = item.ticketID;
                            worksheet.Cell(row, 2).Value = item.IdFlight;
                            worksheet.Cell(row, 3).Value = item.IdType;
                            worksheet.Cell(row, 4).Value = item.price;

                            // Check for null before accessing the value
                            worksheet.Cell(row, 5).Value = item.status.HasValue ? (XLCellValue)item.status.Value : "N/A";

                            worksheet.Cell(row, 6).Value = item.luggage;
                            worksheet.Cell(row, 7).Value = item.HandLuggage;

                            row++;


                        }

                        // Set the content type and file name for the response
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=ExportedData.xlsx");

                        // Save the workbook to the response stream
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            memoryStream.Close();
                        }
                    }

                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    // Handle exception
                    return Content($"An error occurred: {ex.Message}");
                }
            }

            private List<Ticket> GetDataFromDatabase()
            {
                using (var db = new Entities1())
                {
                    // Replace this query with your actual data retrieval logic
                    return db.Tickets.ToList();
                }
            }

        }
    }

