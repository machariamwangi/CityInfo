using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _cityInfoContext;
        
        public CityInfoRepository(CityInfoContext cityInfoContext)
        {
            _cityInfoContext = cityInfoContext?? throw new ArgumentNullException(nameof(cityInfoContext));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await GetCitiesAsync();
            }
            name = name.Trim();
            return await _cityInfoContext.Cities
                .Where(c => c.Name == name)
                .OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<City?> GetCityAsync(int cityId, bool icludePointOfInterest)
        {
            if (icludePointOfInterest)
            {
                return await _cityInfoContext.Cities.Include(x => x.PointOfInterests).Where(x => x.Id == cityId).FirstOrDefaultAsync();
            }
            else
            {
                return await _cityInfoContext.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();  
            }
        }
        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _cityInfoContext.Cities.AnyAsync(p => p.Id == cityId);
        }
        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId)
        {
           return await _cityInfoContext.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityInfoContext.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task AddPointIfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if(city != null)
            {
                city.PointOfInterests.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityInfoContext.SaveChangesAsync() >= 0);
        }

        public void DeletePointIfInterest(PointOfInterest pointOfInterest)
        {
           _cityInfoContext.PointOfInterests.Remove(pointOfInterest);
        }
    }
}
