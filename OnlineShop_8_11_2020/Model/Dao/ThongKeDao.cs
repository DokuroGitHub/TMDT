using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ThongKeDao
    {
        OnlineShopDbContext db = null;
        public ThongKeDao()
        {
            db = new OnlineShopDbContext();
        }

        //ListOrder1Year
        public List<OrderViewModel> ListOrder1Year(int year)
        {
            var model1 = db.Orders.Where(x=>x.CreatedDate.Value.Year==year).ToList();
            var model2 = db.OrderDetails.ToList();
            var model3 = db.OrderCoupons.ToList();
            var model = new List<OrderViewModel>();
            for(int i =0;i<model1.Count();i++)
            {
                var orderViewModel = new OrderViewModel();
                orderViewModel.OrderID = model1[i].ID;
                orderViewModel.CreatedDate = model1[i].CreatedDate;
                orderViewModel.Status = model1[i].Status;
                foreach (var item2 in model2)
                {
                    if (item2.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice += item2.Price.GetValueOrDefault(0);
                    }
                }
                foreach (var item3 in model3)
                {
                    if (item3.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice -= item3.DiscountAmount.GetValueOrDefault(0);
                    }
                }
                model.Add(orderViewModel);
            }
            return model;
        }

        //ListOrder1Month
        public List<OrderViewModel> ListOrder1Month(int month, int year)
        {
            var model1 = db.Orders.Where(x=>x.CreatedDate.Value.Year==year && x.CreatedDate.Value.Month == month).ToList();
            var model2 = db.OrderDetails.ToList();
            var model3 = db.OrderCoupons.ToList();
            var model = new List<OrderViewModel>();
            for (int i = 0; i < model1.Count(); i++)
            {
                var orderViewModel = new OrderViewModel();
                orderViewModel.OrderID = model1[i].ID;
                orderViewModel.CreatedDate = model1[i].CreatedDate;
                orderViewModel.Status = model1[i].Status;
                foreach (var item2 in model2)
                {
                    if (item2.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice += item2.Price.GetValueOrDefault(0);
                    }
                }
                foreach (var item3 in model3)
                {
                    if (item3.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice -= item3.DiscountAmount.GetValueOrDefault(0);
                    }
                }
                model.Add(orderViewModel);
            }
            return model;
        }

        //ListOrderLastNdays
        public List<OrderViewModel> ListOrderLastNdays(int nDays)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var NdaysBefore = DateTime.Now.AddDays(-nDays);
            var model1 = db.Orders.Where(x => x.CreatedDate >= NdaysBefore).ToList();
            var model2 = db.OrderDetails.ToList();
            var model3 = db.OrderCoupons.ToList();
            var model = new List<OrderViewModel>();
            for (int i = 0; i < model1.Count(); i++)
            {
                var orderViewModel = new OrderViewModel();
                orderViewModel.OrderID = model1[i].ID;
                orderViewModel.CreatedDate = model1[i].CreatedDate;
                orderViewModel.Status = model1[i].Status;
                foreach (var item2 in model2)
                {
                    if (item2.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice += item2.Price.GetValueOrDefault(0);
                    }
                }
                foreach (var item3 in model3)
                {
                    if (item3.OrderID == model1[i].ID)
                    {
                        orderViewModel.TotalPrice -= item3.DiscountAmount.GetValueOrDefault(0);
                    }
                }
                model.Add(orderViewModel);
            }
            return model;
        }

        //ListProductCategoryView
        public List<ProductCategoryViewModel> ListProductCategoryView()
        {
            //tạm thời chỉ lấy productCategory con
            var model1 = db.ProductCategories.Where(x => x.ParentID !=null).ToList();
            var model2 = db.Products.Where(x => x.Status == true).ToList();
            var model3 = db.OrderDetails.ToList();

            var model4 = (from a in db.OrderDetails
                          join b in db.Products on a.ProductID equals b.ID
                          join c in db.ProductCategories on b.CategoryID equals c.ID
                          select new
                          {
                              ID = c.ID,
                              TotalProductsSold = a.Quantity!=null? a.Quantity : 0,
                              TotalSold = a.Quantity.GetValueOrDefault(0) * a.Price.GetValueOrDefault(0),
                          }).AsEnumerable().Select(x => new ProductCategoryViewModel()
                          {
                              ID = x.ID,
                              TotalProductsSold = (long)x.TotalProductsSold,
                              TotalSold = x.TotalSold,
                          });
            var model = new List<ProductCategoryViewModel>();

            for (int i = 0; i < model1.Count(); i++)
            {
                var productCategoryViewModel = new ProductCategoryViewModel();
                productCategoryViewModel.ID = model1[i].ID;
                productCategoryViewModel.Name = model1[i].Name;
                productCategoryViewModel.MetaName = model1[i].MetaTitle;
                foreach (var item2 in model2)
                {
                    if (item2.CategoryID == model1[i].ID)
                    {
                        productCategoryViewModel.TotalProducts += item2.Quantity.GetValueOrDefault(0);
                        productCategoryViewModel.TotalCost += item2.Quantity.GetValueOrDefault(0) * item2.Price.GetValueOrDefault(0);
                    }
                }
                model.Add(productCategoryViewModel);
            }
            //thêm TotalProducts, TotalCost
            foreach (var item3 in model3)
            {
                foreach (var item2 in model2)
                {
                    if (item3.ProductID == item2.ID)
                    {
                        foreach (var item in model)
                        {
                            if (item2.CategoryID == item.ID)
                            {
                                item.TotalProductsSold += item3.Quantity.GetValueOrDefault(0);
                                item.TotalSold += item3.Quantity.GetValueOrDefault(0) * item3.Price.GetValueOrDefault(0);
                            }
                        }
                    }
                }
            }

            return model;
        }

    }
}
