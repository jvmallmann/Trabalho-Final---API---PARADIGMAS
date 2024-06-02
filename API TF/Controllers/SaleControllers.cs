﻿using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using API_TF.Services;
using ApiWebDB.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using API_TF.Services.Exceptions;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace API_TF.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as vendas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : Controller
    {
        private readonly SaleService _service;
        public readonly IValidator<SaleDTO> _validator;

        public SalesController(SaleService service, IValidator<SaleDTO> validator)
        {
            _service = service;
            _validator = validator;

        }


        /// <summary>
        /// Insere uma nova venda.
        /// </summary>
        /// <param name="sale">A venda a ser inserida</param>
        /// <returns>A venda inserida.</returns>
        /// <response code="200">Indica que a venda foi inserida com sucesso.</response>
        /// <response code="400">Indica que houve um erro de validação nos dados da venda ou que o estoque é insuficiente.</response>
        /// <response code="404">Indica que o produto com o ID especificado não foi encontrado.</response>
        /// <response code="422">Indica que os dados são inválidos.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        /// 
        [HttpPost()]
        public ActionResult<TbSale> Insert(SaleDTO sale)
        {
            try
            {
                var validationResult = _validator.Validate(sale);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var entity = _service.Insert(sale);
                return Ok(entity);
            }
            catch (InvalidEntity ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex.Message);
            }
        }


        /// <summary>
        /// Busca uma venda pelo código da venda.
        /// </summary>
        /// <param name="code">O código da venda.</param>
        /// <returns>A venda correspondente ao código fornecido.</returns>
        /// <response code="200">Indica que a venda foi encontrada com sucesso.</response>
        /// <response code="404">Indica que a venda especificada pelo código não foi encontrada.</response>
        /// <response code="400">Indica que ocorreu um erro ao tentar buscar a venda.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("{code}")]
        public ActionResult<TbSale> GetSaleByCode(string code)
        {
            try
            {
                var entity = _service.GetSaleByCode(code);
                return Ok(entity);
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (System.Exception e)
            {
                return new ObjectResult(new { error = e.Message })
                {
                    StatusCode = 500
                };
            }
        }


        /// <summary>
        /// Obtém um relatório de vendas por período.
        /// </summary>
        /// <param name="startDate">A data de início do período.</param>
        /// <param name="endDate">A data de fim do período.</param>
        /// <returns>Uma lista de relatórios de vendas agrupados por código da venda.</returns>
        /// <response code="200">Indica que o relatório de vendas foi retornado com sucesso.</response>
        /// <response code="400">Indica que as datas de início e fim não foram fornecidas ou são inválidas.</response>
        /// <response code="404">Indica que não foram encontradas vendas no período especificado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("report")]
        public ActionResult<List<SalesReportDTO>> GetSalesReport(DateTime startDate, DateTime endDate)
        {

            if (startDate == default || endDate == default)
            {
                return BadRequest("As datas de início e fim são obrigatórias.");
            }

            try
            {
                var report = _service.GetSalesReportByPeriod(startDate, endDate);
                return Ok(report);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}