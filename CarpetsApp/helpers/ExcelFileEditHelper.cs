﻿using CarpetsApp.model;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetsApp.helpers
{
    public class ExcelFileEditHelper
    {
        public static List<string> columns = new List<string>
        {
                "B", "C", "D", "E", "F", "G", "H", "J", "K"
        };

        public static String ITEM_NAME = "Zamena otiraca po ugovoru";
        public static String UNIT_NAME = "m" + "\xB2";
        public static String DISPATCHER = "OTPREMNICA";


        public static void editExcelFile(Company company, bool print)
        {
            Bill b = company.Bill;
            int dateYvalue = 40 + b.Items.Count - 1;

            //Load Workbook
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(ApplicationA.FILE_NAME_BILL);

            //Edit Text
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Range["H3"].Text = company.Name;
            sheet.Range["H4"].Text = company.Address;
            sheet.Range["H5"].Text = company.City;
            sheet.Range["H3"].Style.Font.IsBold = true;
            sheet.Range["H4"].Style.Font.IsBold = true;
            sheet.Range["H5"].Style.Font.IsBold = true;

            sheet.Range["J7"].Text = company.Pib;

            sheet.Range["H13"].Text = b.BillNumForYear + "-" + MonthHelper.getIntFromMonth(b.TrafficMonth) + "-" + (b.TrafficYear - 2000);

            sheet.Range["H16"].Text = b.TrafficMonth;

            if(b.Items.Count > 1)
            {
                sheet.InsertRow(28, b.Items.Count - 1);
                sheet.Copy(sheet.Range["A28"], sheet.Range["A28:A" + (26 + b.Items.Count - 1)], true);
                sheet.Range["B40" + dateYvalue].Text = b.BillDate.Day + "-" + b.BillDate.Month + "-" + b.BillDate.Year;

                setBorders(27, b.Items.Count + 1, sheet);

                for(int i = 0; i < b.Items.Count; i++)
                {
                    fillBillItem(b.Items[i], 28 + i, sheet);
                }

                sheet.Range["K" + (29 + b.Items.Count - 1)].Text = BillMaxHelper.getBillValue(b).ToString();
                sheet.Range["B" + dateYvalue].Text = b.BillDate.Day + "-" + b.BillDate.Month + "-" + b.BillDate.Year;
            }
            else
            {
                Billitem item = b.Items[0];
                double sum = item.Carpet.Width * item.Carpet.Length * item.Price;

                fillBillItem(b.Items[0], 28, sheet);
                sheet.Range["B40"].Text = b.BillDate.Day + "-" + b.BillDate.Month + "-" + b.BillDate.Year;
                sheet.Range["K29"].Text = Math.Round(sum, 2).ToString();
            }

            //Save and Launch
            //workbook.SaveToFile("EditSheet.xlsx", ExcelVersion.Version2010);
            //System.Diagnostics.Process.Start("EditSheet.xlsx");

            if (print)
            {
                if(b.Dispatcher != "")
                {
                    sheet.Range["F12"].Text = "";
                    sheet.Range["F13"].Text = DISPATCHER;
                }
                //PrintHelper.printExcelDoc(workbook);
            }

        }

        private static void fillBillItem(Billitem billitem, int start_num, Worksheet sheet)
        {
            double length = billitem.Carpet.Length;
            double width = billitem.Carpet.Width;
            double surface = length * width * billitem.Quantity;
            double sum = Math.Round(surface * billitem.Price, 2);

            List<string> items = new List<string> {
                ITEM_NAME, length.ToString(), width.ToString(), UNIT_NAME, billitem.Quantity.ToString(),
                Math.Round(billitem.Price, 2).ToString(), Math.Round(surface, 2).ToString(), sum.ToString(), sum.ToString()
            };
            
            for(int i = 0; i < items.Count; i++)
            {
                sheet.Range[columns[i] + start_num].Text = items[i];
            }

        }

        private static void setBorders(int row_number, int number_of_rows, Worksheet sheet)
        {
            int row = 0;

            for(int i = 0; i < number_of_rows; i++)
            {
                row = row_number + i;
                sheet.Range["A" + row + ":K" + row].BorderAround();
                sheet.Range["A" + row + ":K" + row].BorderInside();
            }
        }
    }
}
