namespace TestContainerDemo.Model;

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; }
    public string ProductNumber { get; set; }
    public string Color { get; set; }
    public double StandardCost { get; set; }
    public double ListPrice { get; set; }
    public string Size { get; set; }
    public double Weight { get; set; }
    public int ProductCategoryID { get; set; }
    public int ProductModelID { get; set; }
    public string SellStartDate { get; set; }
    public object SellEndDate { get; set; }
    public object DiscontinuedDate { get; set; }
    public string ThumbNailPhoto { get; set; }
    public string ThumbnailPhotoFileName { get; set; }
    public string rowguid { get; set; }
    public string ModifiedDate { get; set; }
    
    public ProductModel? ProductModel { get; set; }
}