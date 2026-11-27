using System.ComponentModel.DataAnnotations;

namespace FPPR_System.Common.Models;

public class LikeListInput
{
    public int SN { get; set; }

    [Required]
    public string UserID { get; set; } = string.Empty;

    [Display(Name = "產品名稱")]
    [Required(ErrorMessage = "請輸入產品名稱")]
    [StringLength(100, ErrorMessage = "產品名稱最多 100 字")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "產品價格")]
    [Range(0.01, 999999999, ErrorMessage = "產品價格需大於 0")]
    public decimal Price { get; set; }

    [Display(Name = "手續費率")]
    [Range(0, 1, ErrorMessage = "手續費率需介於 0 ~ 1（例：0.01 = 1%）")]
    public decimal FeeRate { get; set; }

    [Display(Name = "購買數量")]
    [Range(1, int.MaxValue, ErrorMessage = "購買數量至少為 1")]
    public int PurchaseQuantity { get; set; }

    [Display(Name = "扣款帳號")]
    [Required(ErrorMessage = "請輸入扣款帳號")]
    [RegularExpression(@"^\d{1,20}$", ErrorMessage = "扣款帳號需為 1~20 位數字")]
    public string Account { get; set; } = string.Empty;
}
