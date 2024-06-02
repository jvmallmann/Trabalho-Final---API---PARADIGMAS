using System;
using System.Collections.Generic;

namespace API_TF.DataBase.Models;

/// <summary>
/// Tabela de logs de alteração de estoque de produtos
/// </summary>
public partial class TbStockLog
{
    /// <summary>
    /// Identificador único da tabela
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identificador do produto
    /// </summary>
    public int Productid { get; set; }

    /// <summary>
    /// Quantidade movimentada. Se estiver adicionando, deve ser positivo, se tiver retirando / vendendo, deve ser negativo
    /// </summary>
    public int Qty { get; set; }

    /// <summary>
    /// Data da movimentação
    /// </summary>
    public DateTime Createdat { get; set; }

    public virtual TbProduct Product { get; set; }
}
