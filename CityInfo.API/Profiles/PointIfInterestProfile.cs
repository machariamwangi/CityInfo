using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointIfInterestProfile:Profile
    {
        public PointIfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
        }
    }
}
