using Cloudzy.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.SalesReport
{
    public class SalesReportFilterViewModel
    {
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
