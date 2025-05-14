using System;
namespace Agri_Energy.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public int ProductId { get; set; }
        public DateTime DateBought { get; set; }
        public int Amount { get; set; }
        public double PriceAtSale { get; set; }

        public User Buyer { get; set; }
        public Product Product { get; set; }
    }

}

