﻿using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistAsync(int cityId);
        Task AddPointIfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointIfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
