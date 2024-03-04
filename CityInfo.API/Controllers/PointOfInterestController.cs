using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService _mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mailService = _mailService ?? throw new ArgumentNullException(nameof(_mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistAsync(cityId))
                {
                    logger.LogInformation($"City with Id {cityId} wasn't found when accessing the Point of Interest");
                    return NotFound();
                }
                var points = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(points));
            }
            catch (Exception ex)
            {

                logger.LogCritical($"{ex.Message}-{ex.InnerException}");
                return StatusCode(500, "A problem Happened while handling your request");
            }

        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult> GetPointOfInterest(int cityId, int pointofinterestid)
        {

            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestCreationDto>> CreatePointOfInterest(int cityId,
          [FromBody] PointOfInterestCreationDto pointOfInterest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointIfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointofinterestid = createdPointOfInterestToReturn.Id
            }, createdPointOfInterestToReturn);
        }


        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestFromStored = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestFromStored);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

          var pointOfInterestToPatch =   _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterest);



            patchDocument.ApplyTo(pointOfInterestToPatch, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(pointOfInterestToPatch, pointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointIfInterest(pointOfInterest);

            _mailService.Send("Point of Interest Deleted.", $"point of interest{pointOfInterest.Name} with Id {pointOfInterest.Id} was deleted");
            return NoContent();
        }
    }
}
