using FPPR_System.Common.Models;

namespace FPPR_System.Models;

public class LikeListIndexViewModel
{
    public IReadOnlyList<User> Users { get; set; } = Array.Empty<User>();
    public User? SelectedUser { get; set; }
    public IReadOnlyList<LikeList> LikeLists { get; set; } = Array.Empty<LikeList>();

    public decimal TotalAmountSum => LikeLists.Sum(l => l.TotalAmount);
    public decimal TotalFeeSum => LikeLists.Sum(l => l.TotalFee);
}
