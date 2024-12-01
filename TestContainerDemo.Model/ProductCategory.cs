namespace TestContainerDemo.Model;

public class ProductCategory
{
    public int ProductCategoryID { get; set; }
    public object ParentProductCategoryID { get; set; }
    public string Name { get; set; }
    public string rowguid { get; set; }
    public string ModifiedDate { get; set; }
}