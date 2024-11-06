namespace AngularServer01.Classes
{
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }

    }
}
