using System;
using System.Collections.Generic;

namespace API_TF.DataBase.Models;

/// <summary>
/// Tabela de promoções
/// </summary>
public partial class TbPromotion
{
    /// <summary>
    /// Identificador unico da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Data e hora de inicio da promoção
    /// </summary>
    public DateTime Startdate { get; set; }

    /// <summary>
    /// date e hora final da promoção
    /// </summary>
    public DateTime Enddate { get; set; }

    /// <summary>
    /// Tipo de promoção\n0 - % de desconto\n1 - Valor fixo de desconto
    /// </summary>
    public int Promotiontype { get; set; }

    /// <summary>
    /// Código do produto em promoção
    /// </summary>
    public int Productid { get; set; }

    /// <summary>
    /// Valor da promoção (Se for tipo 0, é o % se for tipo 1, deve ser o valor monetário)
    /// </summary>
    public decimal Value { get; set; }

    public virtual TbProduct Product { get; set; }
}
