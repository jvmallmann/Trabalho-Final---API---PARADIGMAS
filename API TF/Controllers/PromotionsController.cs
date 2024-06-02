using API_TF.DataBase.Models;
using API_TF.Services;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using ApiWebDB.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API_TF.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as Promoções.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : Controller
    {
        private readonly PromotionService _service;

        public PromotionsController(PromotionService service)
        {
            _service = service;
        }


        /// <summary>
        /// Adiciona uma nova promoção.
        /// </summary>
        /// <param name="promo">O DTO da promoção a ser inserida.</param>
        /// <returns>A promoção inserida.</returns>
        /// <response code="200">Retorna a promoção inserida.</response>
        /// <response code="422">Se a validação da entidade falhar.</response>
        /// <response code="400">Se ocorrer um erro inesperado.</response>
        [HttpPost]
        public ActionResult<TbPromotion> Insert(PromotionDTO promo)
        {
            try
            {
                var entity = _service.Insert(promo);
                return Ok(entity);
            }
            catch (InvalidEntity e)
            {
                return new ObjectResult(new { error = e.Message })
                {
                    StatusCode = 422
                };
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Atualiza uma promoção existente.
        /// </summary>
        /// <param name="id">O ID da promoção a ser atualizada.</param>
        /// <param name="promo">O DTO da promoção com os dados atualizados.</param>
        /// <returns>A promoção atualizada.</returns>
        /// <response code="200">Retorna a promoção atualizada.</response>
        /// <response code="404">Se a promoção com o ID especificado não for encontrada.</response>
        /// <response code="400">Se ocorrer um erro inesperado.</response>
        [HttpPatch("{id}")]
        public ActionResult<TbPromotion> Update(int id, PromotionDTO promo)
        {
            try
            {
                var entity = _service.Update(promo, id);
                return Ok(entity);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Busca todas as promoções de um produto em um determinado período.
        /// </summary>
        /// <param name="productId">O ID do produto.</param>
        /// <param name="startDate">Data de início do período.</param>
        /// <param name="endDate">Data de fim do período.</param>
        /// <returns>Lista de promoções.</returns>
        /// <response code="200">Retorna a lista de promoções.</response>
        /// <response code="404">Se nenhuma promoção for encontrada para o período especificado.</response>
        /// <response code="500">Se ocorrer um erro inesperado.</response>
        [HttpGet("product/{productId}/period")]
        public ActionResult<IEnumerable<TbPromotion>> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entity = _service.GetPromotionsByProductAndPeriod(productId, startDate, endDate);
                return Ok(entity);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return new ObjectResult(new { error = e.Message })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
