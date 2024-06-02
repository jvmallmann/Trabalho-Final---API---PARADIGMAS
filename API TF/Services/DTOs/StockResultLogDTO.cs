using System;

namespace API_TF.Services.DTOs
{
    public class StockResultLogDTO
    {
        public DateTime Date { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
