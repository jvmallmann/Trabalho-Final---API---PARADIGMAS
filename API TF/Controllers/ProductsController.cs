using API_TF.DataBase.Models;
using API_TF.Services;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using API_TF.Services.Validate;
using ApiWebDB.Services.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace API_TF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ProductService _service;
        public readonly IValidator<ProductUpDTO> _validatorUpProduct;


        public ProductsController(ProductService service, IValidator<ProductUpDTO> validatorUpProduct)
        {
            _service = service;
            _validatorUpProduct = validatorUpProduct;
        }

        /// <summary>
        /// Adicionar um novo Produto.
        /// </summary>
        /// <param name="product">O Produto a ser inserido.</param>
        /// <returns>O Produto inserido com sucesso.</returns>
        /// <response code="200">Indica que o produto foi inserido com sucesso.</response>
        /// <response code="422">Indica que os dados são inválidos.</response>
        /// <response code="400">Indica que ocorreu um erro ao tentar inserir o produto.</response>
        [HttpPost()]
        public ActionResult<TbProduct> Insert(ProductDTO product)
        {
            try
            {
                var entity = _service.Insert(product);
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
        /// Obtém um Produto com base no BARCODE.
        /// </summary>
        /// <param name="barcode">O BARCODE do produto a ser buscado.</param>
        /// <returns>O produto correspondente ao BARCODE fornecido.</returns>
        /// <response code="200">Indica que o produto foi encontrado com sucesso.</response>
        /// <response code="404">Indica que o produto especificado pelo BARCODE não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno ao tentar buscar o produto.</response>
        [HttpGet("{barcode}")]
        public ActionResult<TbProduct> Get(string barcode)
        {
            try
            {
                var entity = _service.GetByBarcode(barcode);
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

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">O ID do produto a ser atualizado.</param>
        /// <param name="dto">Os novos dados do produto.</param>
        /// <returns>O produto atualizado.</returns>
        /// <response code="200">Indica que o produto foi atualizado com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o produto com o ID especificado não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPatch("{id}")]
        public ActionResult<TbProduct> Update(int id, ProductUpDTO product)
        {
            try
            {
                var ProductById = _service.GetById(id);

                if (ProductById == null)
                {
                    return NotFound("Produto com o ID especificado não foi encontrado");
                }

                var validationResult = _validatorUpProduct.Validate(product);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Message = "Dados inválidos", Errors = validationResult.Errors });
                }

                var entity = _service.Update(product, id);

                return Ok(entity);
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o estoque de um produto pelo ID.
        /// </summary>
        /// <remarks>
        /// O método permite aumentar ou diminuir o estoque de um produto existente especificado pelo seu ID.
        /// </remarks>
        /// <param name="id">O ID do produto a ser atualizado o estoque.</param>
        /// <param name="stockUpdate">Os dados de atualização do estoque, contendo a quantidade a ser adicionada ou removida.</param>
        /// <returns>Um código de status HTTP que indica o resultado da operação.</returns>
        /// <response code="200">Indica que o estoque do produto foi atualizado com sucesso.</response>
        /// <response code="400">Indica que os dados são inválidos.</response>
        /// <response code="404">Indica que o produto especificado pelo ID não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno ao tentar atualizar o estoque.</response>
        [HttpPatch("{id}/stock")]
        public IActionResult UpdateStock(int id, [FromBody] StockUpdateDTO stockUpdate)
        {
            try
            {
                var product = _service.GetById(id);
                if (product == null)
                {
                    return NotFound("Produto não encontrado.");
                }

                if (stockUpdate.Quantity == 0)
                {
                    return BadRequest("A quantidade a atualizar deve ser diferente de zero.");
                }

                _service.AjustarStock(id, stockUpdate.Quantity);

                return Ok("Estoque atualizado com sucesso.");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Obtém produtos com base na descrição.
        /// </summary>
        /// <param name="description">A descrição dos produtos a serem buscados.</param>
        /// <returns>Uma lista de produtos que correspondem à descrição.</returns>
        /// <response code="200">Indica que os produtos foram encontrados com sucesso.</response>
        /// <response code="404">Indica que nenhum produto correspondente à descrição foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno ao tentar buscar os produtos.</response>
        [HttpGet("description")]
        public ActionResult<IEnumerable<TbProduct>> GetByDescription([FromQuery] string description)
        {
            try
            {
                var products = _service.GetByDescription(description);
                return Ok(products);
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
