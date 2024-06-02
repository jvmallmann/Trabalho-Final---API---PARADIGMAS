namespace API_TF.Services.DTOs
{
    public class ProductUpDTO
    {
        public string Description { get; set; }
        public string Barcode { get; set; }
        public string Barcodetype { get; set; }
        public decimal Price { get; set; }
        public decimal Costprice { get; set; }
    }
}
