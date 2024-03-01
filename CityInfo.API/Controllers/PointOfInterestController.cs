using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointOfInterestController: ControllerBase
    {
        private readonly ILogger<PointOfInterestController> logger;

        public PointOfInterestController(ILogger<PointOfInterestController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<PointOfInterestDto> GetPointsOfInterest(int cityId)
        {
            throw new ArgumentException("EXception Sample");
            try
            {
              
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
                if (city == null)
                {
                    logger.LogInformation($"City with Id {cityId} wasn't found when accessing the Point of Interest");
                    return NotFound();
                }
                var points = city.PointsOfInterest.ToList();
                return Ok(points);
            }
            catch (Exception ex)
            {

                logger.LogCritical($"{ex.Message}-{ex.InnerException}");
                return StatusCode(500, "A problem Happened while handling your request");
            }
  
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
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

        [HttpPost]
        public ActionResult<PointOfInterestCreationDto> CreatePointOfInterest(int cityId, 
          [FromBody]  PointOfInterestCreationDto pointOfInterestCreationDto)
        {
            if (ModelState.IsValid)
            {
                return BadRequest();
            }
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p =>  p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterestCreationDto.Name,
                Description = pointOfInterestCreationDto.Description,
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new {
                cityId = cityId,
                pointofinterestid = finalPointOfInterest.Id
            }, finalPointOfInterest);
        }


        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterestFromStored = city.PointsOfInterest.FirstOrDefault(x => x.Id.Equals(pointOfInterestId));

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            pointOfInterestFromStored.Name = pointOfInterest.Name;
            pointOfInterest.Description = pointOfInterest.Description;

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id.Equals(pointOfInterestId));

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!TryValidateModel(pointOfInterestToPatch))
            { 
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id.Equals(pointOfInterestId));

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterest);
            return NoContent();
        }
    }
}
