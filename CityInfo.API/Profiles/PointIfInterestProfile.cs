﻿using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointIfInterestProfile:Profile
    {
        public PointIfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap< Models.PointOfInterestCreationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap< Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
        }
    }
}
