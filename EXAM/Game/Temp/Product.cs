using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework7_8.Problem_02
{
    public abstract class Product
    {

        protected string name;
        protected float price;
        protected int quantity;

        protected ProductType productType;
        public ProductStatus ProductStatus { get; set; }
        public Product(string name, float price, int quantity, ProductStatus productStatus, ProductType productType)
        {
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.ProductStatus = productStatus;
            this.productType = productType;
        }

        public virtual void Print()
        {
            Console.WriteLine($"Name          = {name}");
            Console.WriteLine($"Price         = {price}");
            Console.WriteLine($"Quantity      = {quantity}");
            Console.WriteLine($"ProductType   = {productType}");
            //Console.WriteLine($"ProductStatus = {ProductStatus.GetType().Name}");
        }
    }
    public enum ProductType
    {
        Foodstuffs,
        HouseholdChemicals,
        Furniture,
        Electronics
    }
    public enum ProductStatus
    {
        Came, Implemented, WrittenOff, Transferred
    }
}
