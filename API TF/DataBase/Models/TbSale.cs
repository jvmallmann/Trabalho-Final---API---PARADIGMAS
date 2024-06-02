using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_TF.DataBase.Models;

/// <summary>
/// tabela dos documentos de venda
/// </summary>
public partial class TbSale
{
    /// <summary>
    /// código único da tabela (Gerado automaticamente)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código da venda (Um código único da venda, onde todos os items de uma venda, terão o mesmo código). Deve ser uma chave guid.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// data de criação do registro
    /// </summary>
    public DateTime Createat { get; set; }

    /// <summary>
    /// Código do produto
    /// </summary>
    public int Productid { get; set; }

    /// <summary>
    /// Preço unitário de venda
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Quantidade vendida
    /// </summary>
    public int Qty { get; set; }

    /// <summary>
    /// Valor de desconto unitário (Valor em reais)
    /// </summary>
    public decimal Discount { get; set; }
    [JsonIgnore]
    public virtual TbProduct Product { get; set; }
}
