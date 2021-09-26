using System;
using Domain;

namespace Application.Redemptions.DTOs
{
	public class RedemptionCreateDTO
	{
		public int WalletId { get; set; }
		public int VoucherId { get; set; }
		public DateTime RedemptionDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}