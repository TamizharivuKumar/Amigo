namespace Amigo.Models
{
    public class Product
    {
        public int Productid { get; set; }
        public string? Productname { get; set; }
        public int ProductQuantity { get; set; }
        public int ProductPrize { get; set; }
        public string? SoldBy { get; set; }
    }
}