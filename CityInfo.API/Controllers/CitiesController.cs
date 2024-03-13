using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController: ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));    
        }

        [HttpGet]
        public async Task<ActionResult<CityWithoutPointsOfInterestDTO>> GetCities( string? name, string? searchQuery, int pageNumber = 1 , int pageSize = 10)
        { 
            if(pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }
            //var cityEntities  =   await _cityInfoRepository.GetCitiesAsync();
            var ( cityEntities, paganitionMetadata  ) =   await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var response = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (response == null)
            {
                return NotFound();
            }
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(response));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDTO>(response));

        }
    }
}
