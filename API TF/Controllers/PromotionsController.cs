using API_TF.DataBase.Models;
using API_TF.Services;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_TF.Controllers
{
    /// <summary>
    /// Controlador que gerencia as Promoções.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : Controller
    {
        private readonly PromotionService _service;
        private readonly ILogger _logger;

        public PromotionsController(PromotionService service, ILogger<PromotionsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        /// <summary>
        /// Adiciona uma nova promoção.
        /// </summary>
        /// <param name="promo">O DTO da promoção a ser inserida.</param>
        /// <returns>A promoção inserida.</returns>
        /// <response code="201">Indica que a promoção foi inserida com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o id do produto passado não existe.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TbPromotion), 201)]
        [ProducesResponseType(500)]
        public ActionResult<TbPromotion> Insert(PromotionDTO promo)
        {
            try
            {
                var entity = _service.Insert(promo);
                return CreatedAtAction(nameof(Insert), new { id = entity.Id }, entity);
            }
            catch (InvalidDataException ex)
            {
                var errors = ex.ValidationErrors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Dados inválidos", Errors = errors });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Atualiza uma promoção existente.
        /// </summary>
        /// <param name="id">O ID da promoção a ser atualizada.</param>
        /// <param name="promo">O DTO da promoção com os dados atualizados.</param>
        /// <returns>A promoção atualizada.</returns>
        /// <response code="200">Indica que a promoção foi atualizada com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que a promoção com o ID especificado não foi encontrada ou o ID do produto não existe.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPatch("{id}")]
        public ActionResult<TbPromotion> Update(int id, PromotionDTO promo)
        {
            try
            {
                var entity = _service.Update(promo, id);
                return Ok(entity);
            }
            catch (InvalidDataException ex)
            {
                var errors = ex.ValidationErrors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Dados inválidos", Errors = errors });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }


        /// <summary>
        /// Busca todas as promoções de um produto em um determinado período.
        /// </summary>
        /// <param name="productId">O ID do produto.</param>
        /// <param name="startDate">Data de início do período.</param>
        /// <param name="endDate">Data de fim do período.</param>
        /// <returns>Lista de promoções.</returns>
        /// <response code="200">Indica que a busca foi realizada com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o ID do produto não foi encontrado ou nenhuma promoção foi encontrada para o período especificado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("product/{productId}/period")]
        public ActionResult<IEnumerable<TbPromotion>> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entity = _service.GetPromotionsByProductAndPeriod(productId, startDate, endDate);
                return Ok(entity);
            }
            catch (InvalidEntity ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, ex.Message);
            }
        }
    }
}
