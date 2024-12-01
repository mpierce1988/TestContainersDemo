namespace TestContainerDemo.Model;

public class SalesOrderDetail
{
    public int SalesOrderID { get; set; }
    public int SalesOrderDetailID { get; set; }
    public int OrderQty { get; set; }
    public int ProductID { get; set; }
    public double UnitPrice { get; set; }
    public double UnitPriceDiscount { get; set; }
    public double LineTotal { get; set; }
    public string rowguid { get; set; }
    public string ModifiedDate { get; set; }
}