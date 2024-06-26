﻿using API_TF.Services;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace API_TF.Controllers
{
    /// <summary>
    /// Controlador que gerencia os Logs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly ILogger _logger;

        public LogsController(LogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém os logs de um determinado produto.
        /// </summary>
        /// <param name="productId">O ID do produto a ser obtido os logs.</param>
        /// <returns>A lista de logs do produto.</returns>
        /// <response code="200">Indica que a operação foi bem-sucedida e retorna logs correspondentes ao produto.</response>
        /// <response code="404">Indica que o ID do produto informado não existe ou nenhum log foi encontrado para o mesmo.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("{productId}/logs")]
        public ActionResult<List<StockResultLogDTO>> GetStockLogs(int productId)
        {
            try
            {
                var logs = _logService.GetLogsByProduct(productId);
                return logs;
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
