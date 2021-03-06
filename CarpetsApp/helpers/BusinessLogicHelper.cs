﻿using CarpetsApp.dao;
using CarpetsApp.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CarpetsApp.helpers
{
    public class BusinessLogicHelper
    {
        public bool createBills(DataGrid dg, DateTime bill_date, DateTime traffic_month_and_year, bool print)
        {
            bool valid = false;

            foreach (Company c in dg.SelectedItems)
            {
                Bill newBill = (Bill)c.Bill.Clone();
                newBill.BillDate = bill_date;
                newBill.BillNumForYear = BillMaxHelper.findMaxBillNumForYear(traffic_month_and_year.Year) + 1;
                newBill.TrafficMonth = MonthHelper.getMonthFromInt(traffic_month_and_year.Month);
                newBill.TrafficYear = traffic_month_and_year.Year;
                newBill.Id = ApplicationA.Instance.Bills[ApplicationA.Instance.Bills.Count - 1].Id + 1;

                valid = BillDao.Add(newBill);
                if (valid)
                {
                    ApplicationA.Instance.Bills.Add(newBill);
                    c.Bill = newBill;
                    ApplicationA.WriteToLogActions(newBill.Id, "racun");
                }

                MessageBox.Show(newBill.Items.Count.ToString());

                foreach (Billitem item in newBill.Items)
                {
                    valid = BillitemDao.Add(item, newBill);
                    if (valid)
                    {
                        item.Id = BillMaxHelper.findMaxBillItemId(newBill);
                        ApplicationA.WriteToLogActions(item.Id, "stavka racuna");
                    }
                }

                if (print)
                {
                    ExcelFileEditHelper.editExcelFile(c, print);
                }
            }

            return valid;
        }

        public double calculateBillAmount(Bill b)
        {
            double amount = 0;

            foreach(Billitem item in b.Items)
            {
                amount += item.Price * item.Carpet.Width * item.Carpet.Length * item.Quantity;
            }

            return amount;
        }

    }
}
