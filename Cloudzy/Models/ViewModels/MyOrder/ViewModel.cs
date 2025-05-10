using System;
using System.Collections.Generic;

namespace Cloudzy.Models.ViewModels.MyOrder
{
    public class ViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
        public string DiscountCode { get; set; }
        public string ReturnReason { get; set; }
        public List<ItemViewModel> Items { get; set; }
    }
}