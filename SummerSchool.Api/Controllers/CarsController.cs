using System.Collections.Generic;
using SummerSchool.Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using EF.Models.Entities;
using EF.Dal.Repos.Interfaces;
using SummerSchool.Services.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace SummerSchool.Api.Controllers
{
    [Route("api/[controller]")]
    public class CarsController : BaseCrudController<Car, CarsController>
    {
        public CarsController(ICarRepo carRepo, IAppLogging<CarsController> logger) :
            base(carRepo, logger)
        {
        }

        /// <summary>
        /// Gets all cars by make
        /// </summary>
        /// <returns>All cars for a make</returns>
        /// <param name="id">Primary key of the make</param>        
        [HttpGet("CarsBy/{id?}")]
        // Example: https://localhost:5021/api/cars/CarsBy/5
        public ActionResult<IEnumerable<Car>> GetCarsBy(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return Ok(((ICarRepo)MainRepo).GetAllBy(id.Value));
            }

            return Ok(MainRepo.GetAllIgnoreQueryFilters());
        }
    }
}
