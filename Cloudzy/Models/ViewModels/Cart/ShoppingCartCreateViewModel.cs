using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.Cart
{
    public class ShoppingCartCreateViewModel
    {
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}
