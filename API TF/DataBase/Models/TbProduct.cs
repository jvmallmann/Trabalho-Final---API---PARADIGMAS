using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_TF.DataBase.Models;

/// <summary>
/// tabela de produtos
/// </summary>
public partial class TbProduct
{
    /// <summary>
    /// código único gerado pelo DB
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descrição do produto
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// código de barras
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Tipo de código de barras:\nEAN-13   Varejo - Número de 13 dígitos)\nDUN-14  Frete - Número de 14 dígitos) \nUPC - Varejo (América do Norte e Canadá) -​ Número de 12 dígitos\nCODE 11 - Telecomunicações - números de 0 a 9, – e *\nCODE 39 - Automotiva e Defesa - Letras (A a Z), numéros (0 a 9) e (-, ., $, /, +, %, e espaço). Um caractere adicional (denotado ‘*’) é usado para os delimitadores de início e parada.
    /// </summary>
    public string Barcodetype { get; set; }

    /// <summary>
    /// Quantidade em estoque
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Preço de venda
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Preço de custo
    /// </summary>
    public decimal Costprice { get; set; }

    public virtual ICollection<TbPromotion> TbPromotions { get; set; } = new List<TbPromotion>();
    [JsonIgnore]
    public virtual ICollection<TbSale> TbSales { get; set; } = new List<TbSale>();

    public virtual ICollection<TbStockLog> TbStockLogs { get; set; } = new List<TbStockLog>();
}
