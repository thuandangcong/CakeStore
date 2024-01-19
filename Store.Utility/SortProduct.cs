using MailKit.Search;
using Store.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Utility
{
    public class SortProduct
    {
        public static List<Product> Sort(List<Product> productList, String sortOrder)
        {
            List<Product> sordList = new List<Product>();
            switch (sortOrder)
            {
                case SD.NameAS:
                    sordList = productList.OrderBy(u => u.Name).ToList();
                    break;
                case SD.NameDS:
                    sordList = productList.OrderByDescending(u => u.Name).ToList();
                    break;
                case SD.PriceAS:
                    sordList = productList.OrderBy(u => u.Price).ToList();
                    break;
                case SD.PriceDS:
                    sordList = productList.OrderByDescending(u => u.Price).ToList();
                    break;
                case SD.DateAS:
                    sordList = productList.OrderByDescending(u => u.UpdatedDate).ToList();               
                    break;
                case SD.DateDS:
                    sordList = productList.OrderBy(u => u.UpdatedDate).ToList();

                    break;


            }
            return sordList;
        }
    }
}
