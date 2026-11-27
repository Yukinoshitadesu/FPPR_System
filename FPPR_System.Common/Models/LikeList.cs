namespace FPPR_System.Common.Models;

public class LikeList
{
    public int SN { get; set; }
    public int ProductNo { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal FeeRate { get; set; }
    public int PurchaseQuantity { get; set; }
    public string Account { get; set; } = string.Empty;
    public decimal TotalFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string Email { get; set; } = string.Empty;
}
