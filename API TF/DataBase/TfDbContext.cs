using System;
using System.Collections.Generic;
using API_TF.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace API_TF.DataBase;

public partial class TfDbContext : DbContext
{
    public TfDbContext()
    {
    }

    public TfDbContext(DbContextOptions<TfDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbPromotion> TbPromotions { get; set; }

    public virtual DbSet<TbSale> TbSales { get; set; }

    public virtual DbSet<TbStockLog> TbStockLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=TF_DB;Username=postgres;Password=masterkey");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_tb_products");

            entity.ToTable("tb_products", tb => tb.HasComment("tabela de produtos"));

            entity.Property(e => e.Id)
                .HasComment("código único gerado pelo DB")
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Barcode)
                .IsRequired()
                .HasMaxLength(40)
                .HasComment("código de barras")
                .HasColumnName("barcode");
            entity.Property(e => e.Barcodetype)
                .IsRequired()
                .HasMaxLength(10)
                .HasComment("Tipo de código de barras:\\nEAN-13   Varejo - Número de 13 dígitos)\\nDUN-14  Frete - Número de 14 dígitos) \\nUPC - Varejo (América do Norte e Canadá) -​ Número de 12 dígitos\\nCODE 11 - Telecomunicações - números de 0 a 9, – e *\\nCODE 39 - Automotiva e Defesa - Letras (A a Z), numéros (0 a 9) e (-, ., $, /, +, %, e espaço). Um caractere adicional (denotado ‘*’) é usado para os delimitadores de início e parada.")
                .HasColumnName("barcodetype");
            entity.Property(e => e.Costprice)
                .HasPrecision(15, 2)
                .HasComment("Preço de custo")
                .HasColumnName("costprice");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Descrição do produto")
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasComment("Preço de venda")
                .HasColumnName("price");
            entity.Property(e => e.Stock)
                .HasDefaultValue(0)
                .HasComment("Quantidade em estoque")
                .HasColumnName("stock");
        });

        modelBuilder.Entity<TbPromotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_tb_promotions");

            entity.ToTable("tb_promotions", tb => tb.HasComment("Tabela de promoções"));

            entity.HasIndex(e => new { e.Startdate, e.Enddate, e.Productid }, "idx_tb_promotions_period");

            entity.Property(e => e.Id)
                .HasComment("Identificador unico da tabela")
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Enddate)
                .HasComment("date e hora final da promoção")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enddate");
            entity.Property(e => e.Productid)
                .HasComment("Código do produto em promoção")
                .HasColumnName("productid");
            entity.Property(e => e.Promotiontype)
                .HasDefaultValue(0)
                .HasComment("Tipo de promoção\\n0 - % de desconto\\n1 - Valor fixo de desconto")
                .HasColumnName("promotiontype");
            entity.Property(e => e.Startdate)
                .HasComment("Data e hora de inicio da promoção")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("startdate");
            entity.Property(e => e.Value)
                .HasPrecision(15, 2)
                .HasComment("Valor da promoção (Se for tipo 0, é o % se for tipo 1, deve ser o valor monetário)")
                .HasColumnName("value");

            entity.HasOne(d => d.Product).WithMany(p => p.TbPromotions)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("fk_tb_promotions_tb_promotions");
        });

        modelBuilder.Entity<TbSale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_tb_sales");

            entity.ToTable("tb_sales", tb => tb.HasComment("tabela dos documentos de venda"));

            entity.Property(e => e.Id)
                .HasComment("código único da tabela (Gerado automaticamente)")
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(40)
                .HasComment("Código da venda (Um código único da venda, onde todos os items de uma venda, terão o mesmo código). Deve ser uma chave guid.")
                .HasColumnName("code");
            entity.Property(e => e.Createat)
                .HasComment("data de criação do registro")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createat");
            entity.Property(e => e.Discount)
                .HasPrecision(15, 2)
                .HasComment("Valor de desconto unitário (Valor em reais)")
                .HasColumnName("discount");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasComment("Preço unitário de venda")
                .HasColumnName("price");
            entity.Property(e => e.Productid)
                .HasComment("Código do produto")
                .HasColumnName("productid");
            entity.Property(e => e.Qty)
                .HasDefaultValue(1)
                .HasComment("Quantidade vendida")
                .HasColumnName("qty");

            entity.HasOne(d => d.Product).WithMany(p => p.TbSales)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tb_sales_tb_products");
        });

        modelBuilder.Entity<TbStockLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_tb_stock_logs");

            entity.ToTable("tb_stock_logs", tb => tb.HasComment("Tabela de logs de alteração de estoque de produtos"));

            entity.Property(e => e.Id)
                .HasComment("Identificador único da tabela")
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasComment("Data da movimentação")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Productid)
                .HasComment("Identificador do produto")
                .HasColumnName("productid");
            entity.Property(e => e.Qty)
                .HasComment("Quantidade movimentada. Se estiver adicionando, deve ser positivo, se tiver retirando / vendendo, deve ser negativo")
                .HasColumnName("qty");

            entity.HasOne(d => d.Product).WithMany(p => p.TbStockLogs)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("fk_tb_stock_logs_tb_products");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
