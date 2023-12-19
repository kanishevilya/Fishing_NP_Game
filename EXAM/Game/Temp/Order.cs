using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

namespace homework7_8.Problem_02
{
    public class Order
    {
        public List<Product> products;

        public Order()
        {
            products=new List<Product>();
        }

        public Order(List<Product> products)
        {
            this.products = products;
        }

        public void addProduct(Product product)
        {
            products.Add(product);
        }
        public void setProductStatuc(int idx, ProductStatus status)
        {
            products[idx].ProductStatus = status;
        }
        public void randomStatusSet()
        {
            foreach (var prod in products)
            {
                prod.ProductStatus = (ProductStatus)alg.RAND(0, Enum.GetValues(typeof(ProductStatus)).Length - 1);
            }
        }
        public void Print()
        {
            foreach (var item in products)
            {
                Console.WriteLine($"Product Status: {item.ProductStatus}");
                item.Print();
                Console.WriteLine("-----------------------------------");
            }
        }

    }
}
