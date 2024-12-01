namespace TestContainerDemo.Model;

public class ProductModel
{
    public int ProductModelID { get; set; }
    public string Name { get; set; }
    public object CatalogDescription { get; set; }
    public string rowguid { get; set; }
    public string ModifiedDate { get; set; }
    
    public ProductDescription? ProductDescription { get; set; }
}