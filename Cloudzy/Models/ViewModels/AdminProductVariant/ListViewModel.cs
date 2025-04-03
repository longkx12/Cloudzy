namespace Cloudzy.Models.ViewModels.AdminProductVariant
{
    public class ListViewModel
    {
        public int STT { get; set; }
        public int VariantId { get; set; }
        public int? ProductId { get; set; }
        public string SizeName { get; set; }
        public string? HeightRange { get; set; }
        public string? WeightRange { get; set; }
        public int Stock { get; set; }

    }
}
