using System;
using System.Collections.Generic;
using EF.Dal.Exceptions;
using EF.Models.Entities.Base;
using EF.Dal.Repos.Base;
using SummerSchool.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SummerSchool.Api.Controllers.Base
{
    // There isn’t a route defined on this class. It will be set using the derived classes.
    [ApiController]
    public abstract class BaseCrudController<T, TController> : ControllerBase
        where T : BaseEntity, new()
        where TController : BaseCrudController<T, TController>
    {
        protected readonly IRepo<T> MainRepo;
        protected readonly IAppLogging<TController> Logger;
        protected BaseCrudController(IRepo<T> repo, IAppLogging<TController> logger)
        {
            MainRepo = repo;
            Logger = logger;
        }

        /// <summary>
        /// Gets all records
        /// </summary>
        /// <returns>All records</returns>
        [HttpGet]
        public ActionResult<IEnumerable<T>> GetAll()
        {
            return Ok(MainRepo.GetAllIgnoreQueryFilters());
        }

        /// <summary>
        /// Gets a single record
        /// </summary>
        /// <param name="id">Primary key of the record</param>
        /// <returns>Single record</returns>   
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<T>> GetOne(int id)
        {
            var entity = MainRepo.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        /// <summary>
        /// Updates a single record
        /// </summary>        
        /// <param name="id">Primary key of the record to update</param>
        /// <returns>Single record</returns>
        [HttpPut("{id}")]
        // The route value is assigned to the id parameter (implicit [FromRoute]),
        // and the entity is assigned from the body of the request(implicit [FromBody])
        public IActionResult UpdateOne(int id, T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            try
            {
                MainRepo.Update(entity);
            }
            catch (CustomException ex)
            {
                //Just for example
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                //Just for example
                return BadRequest(ex);
            }

            return Ok(entity);
        }

        /// <summary>
        /// Adds a single record
        /// </summary>        
        /// <returns>Added record</returns>       
        [HttpPost]
        // This returns an HTTP 201 to the client, with the URL for the newly created entity as the Location header value.
        // The body of the response is the newly added entity as JSON.
        public ActionResult<T> AddOne(T entity)
        {
            try
            {
                MainRepo.Add(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, entity);
        }

        /// <summary>
        /// Deletes a single record
        /// </summary>        
        /// <returns>Nothing</returns>
        [HttpDelete("{id}")]
        public ActionResult<T> DeleteOne(int id, T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            try
            {
                MainRepo.Delete(entity);
            }
            catch (Exception ex)
            {
                // ?. (Null Conditional Operator)
                return new BadRequestObjectResult(ex.GetBaseException()?.Message);
            }

            return Ok();
        }
    }
}