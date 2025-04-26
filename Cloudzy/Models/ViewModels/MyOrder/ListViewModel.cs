namespace Cloudzy.Models.ViewModels.MyOrder
{
    public class ListViewModel
    {
        public List<ViewModel> Orders { get; set; } = new List<ViewModel>();
        public string CurrentStatus { get; set; } = "all";
    }
}
