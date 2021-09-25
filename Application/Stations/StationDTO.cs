namespace Application.Stations
{
	public class StationDTO
	{
		public int? Id { get; set; }
		public int? AreaId { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Coordinate { get; set; }
		public bool? IsDeleted { get; set; }
	}
}