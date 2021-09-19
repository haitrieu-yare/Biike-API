using Application.Stations;
using AutoMapper;
using Domain;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			// CreateMap<Station, Station>()
			// 	.ForMember(s => s.Id, opt => opt.Ignore())
			// 	.ForMember(s => s.Area, opt => opt.Ignore())
			// 	.ForMember(s => s.DepartureRoutes, opt => opt.Ignore())
			// 	.ForMember(s => s.DestinationRoutes, opt => opt.Ignore());
			CreateMap<Station, StationDTO>();
			CreateMap<StationDTO, Station>()
				.ForMember(s => s.Id, opt => opt.Ignore());
		}
	}
}