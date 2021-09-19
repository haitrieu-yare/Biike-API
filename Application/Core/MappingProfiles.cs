using Application.Routes;
using Application.Stations;
using Application.Trips;
using AutoMapper;
using Domain;

namespace Application.Core
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Station, StationDTO>();
			CreateMap<StationDTO, Station>()
				.ForMember(s => s.Id, opt => opt.Ignore());
			CreateMap<Route, RouteDTO>();
			CreateMap<RouteDTO, Route>()
				.ForMember(r => r.Id, opt => opt.Ignore());
			CreateMap<Trip, TripDTO>();
			CreateMap<TripDTO, Trip>()
				.ForMember(t => t.Id, opt => opt.Ignore());
		}
	}
}