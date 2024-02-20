using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointOfInterestController: ControllerBase
    {
        [HttpGet]
        public ActionResult<PointOfInterestDto> GetPointsOfInterest(int cityId)
        { 
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null )
            {
                return NotFound();
            }
            var points = city.PointsOfInterest.ToList();
            return Ok(points);
        }

        [HttpGet("{pointofinterestid}")]
        public ActionResult GetPointOfInterest(int cityId, int pointofinterestid)
        {
            var city  = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if(city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id.Equals(pointofinterestid));

            if (pointOfInterest == null)
            {
                return NotFound() ;
            }

            return Ok(pointOfInterest);
        }
    }
}
