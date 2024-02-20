using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController: ControllerBase
    {
        [HttpGet]
        public ActionResult<CityDto> GetCities()
        {
            var result =   CitiesDataStore.Current.Cities;
            return Ok(result);

        }
        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCities(int id)
        {
            var response = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);

        }
    }
}
