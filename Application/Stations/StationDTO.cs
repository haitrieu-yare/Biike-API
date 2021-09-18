namespace Application.Stations
{
	public class StationDTO
	{
		public int Id { get; set; }
		public int AreaId { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public bool IsDeleted { get; set; }
	}
}