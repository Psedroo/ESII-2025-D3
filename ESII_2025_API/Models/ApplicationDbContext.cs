using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ESII_2025_API.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atividade> Atividades { get; set; }

    public virtual DbSet<Bilhete> Bilhetes { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Organizador> Organizadors { get; set; }

    public virtual DbSet<Participante> Participantes { get; set; }

    public virtual DbSet<Relatorio> Relatorios { get; set; }

    public virtual DbSet<Relatorioespecifico> Relatorioespecificos { get; set; }

    public virtual DbSet<Relatoriogeral> Relatoriogerals { get; set; }

    public virtual DbSet<Tipobilhete> Tipobilhetes { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    public virtual DbSet<ViewEventosAtividade> ViewEventosAtividades { get; set; }

    public virtual DbSet<ViewEventosOrganizadore> ViewEventosOrganizadores { get; set; }

    public virtual DbSet<ViewParticipantesBilhete> ViewParticipantesBilhetes { get; set; }

    public virtual DbSet<ViewRelatoriosEvento> ViewRelatoriosEventos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ES2;Username=postgres;Password=1");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atividade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("atividade_pkey");

            entity.ToTable("atividade");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Bilhete>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bilhete_pkey");

            entity.ToTable("bilhete");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.IdParticipante).HasColumnName("id_participante");
            entity.Property(e => e.Quantidade).HasColumnName("quantidade");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdParticipanteNavigation).WithMany(p => p.Bilhetes)
                .HasForeignKey(d => d.IdParticipante)
                .HasConstraintName("bilhete_id_participante_fkey");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("evento_pkey");

            entity.ToTable("evento");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CapacidadeMax).HasColumnName("capacidade_max");
            entity.Property(e => e.Categoria)
                .HasMaxLength(255)
                .HasColumnName("categoria");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.IdOrganizador).HasColumnName("id_organizador");
            entity.Property(e => e.Local)
                .HasMaxLength(255)
                .HasColumnName("local");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");
            entity.Property(e => e.PrecoIngresso)
                .HasPrecision(10, 2)
                .HasColumnName("preco_ingresso");

            entity.HasOne(d => d.IdOrganizadorNavigation).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.IdOrganizador)
                .HasConstraintName("evento_id_organizador_fkey");

            entity.HasMany(d => d.IdAtividades).WithMany(p => p.IdEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoAtividade",
                    r => r.HasOne<Atividade>().WithMany()
                        .HasForeignKey("IdAtividade")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_atividade_id_atividade_fkey"),
                    l => l.HasOne<Evento>().WithMany()
                        .HasForeignKey("IdEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_atividade_id_evento_fkey"),
                    j =>
                    {
                        j.HasKey("IdEvento", "IdAtividade").HasName("evento_atividade_pkey");
                        j.ToTable("evento_atividade");
                        j.IndexerProperty<int>("IdEvento").HasColumnName("id_evento");
                        j.IndexerProperty<int>("IdAtividade").HasColumnName("id_atividade");
                    });

            entity.HasMany(d => d.IdBilhetes).WithMany(p => p.IdEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoBilhete",
                    r => r.HasOne<Bilhete>().WithMany()
                        .HasForeignKey("IdBilhete")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_bilhete_id_bilhete_fkey"),
                    l => l.HasOne<Evento>().WithMany()
                        .HasForeignKey("IdEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_bilhete_id_evento_fkey"),
                    j =>
                    {
                        j.HasKey("IdEvento", "IdBilhete").HasName("evento_bilhete_pkey");
                        j.ToTable("evento_bilhete");
                        j.IndexerProperty<int>("IdEvento").HasColumnName("id_evento");
                        j.IndexerProperty<int>("IdBilhete").HasColumnName("id_bilhete");
                    });

            entity.HasMany(d => d.IdRelatorioespecificos).WithMany(p => p.IdEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoRelatorioespecifico",
                    r => r.HasOne<Relatorioespecifico>().WithMany()
                        .HasForeignKey("IdRelatorioespecifico")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_relatorioespecifico_id_relatorioespecifico_fkey"),
                    l => l.HasOne<Evento>().WithMany()
                        .HasForeignKey("IdEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_relatorioespecifico_id_evento_fkey"),
                    j =>
                    {
                        j.HasKey("IdEvento", "IdRelatorioespecifico").HasName("evento_relatorioespecifico_pkey");
                        j.ToTable("evento_relatorioespecifico");
                        j.IndexerProperty<int>("IdEvento").HasColumnName("id_evento");
                        j.IndexerProperty<int>("IdRelatorioespecifico").HasColumnName("id_relatorioespecifico");
                    });

            entity.HasMany(d => d.IdRelatoriogerals).WithMany(p => p.IdEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoRelatoriogeral",
                    r => r.HasOne<Relatoriogeral>().WithMany()
                        .HasForeignKey("IdRelatoriogeral")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_relatoriogeral_id_relatoriogeral_fkey"),
                    l => l.HasOne<Evento>().WithMany()
                        .HasForeignKey("IdEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("evento_relatoriogeral_id_evento_fkey"),
                    j =>
                    {
                        j.HasKey("IdEvento", "IdRelatoriogeral").HasName("evento_relatoriogeral_pkey");
                        j.ToTable("evento_relatoriogeral");
                        j.IndexerProperty<int>("IdEvento").HasColumnName("id_evento");
                        j.IndexerProperty<int>("IdRelatoriogeral").HasColumnName("id_relatoriogeral");
                    });
        });

        modelBuilder.Entity<Organizador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organizador_pkey");

            entity.ToTable("organizador");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .HasColumnName("contacto");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithMany(p => p.Organizadors)
                .HasForeignKey(d => d.IdUtilizador)
                .HasConstraintName("organizador_id_utilizador_fkey");
        });

        modelBuilder.Entity<Participante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("participante_pkey");

            entity.ToTable("participante");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .HasColumnName("contacto");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithMany(p => p.Participantes)
                .HasForeignKey(d => d.IdUtilizador)
                .HasConstraintName("participante_id_utilizador_fkey");
        });

        modelBuilder.Entity<Relatorio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("relatorio_pkey");

            entity.ToTable("relatorio");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Informacoes).HasColumnName("informacoes");
        });

        modelBuilder.Entity<Relatorioespecifico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("relatorioespecifico_pkey");

            entity.ToTable("relatorioespecifico");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.IdRelatorio).HasColumnName("id_relatorio");
            entity.Property(e => e.NumParticipantesAtiv).HasColumnName("num_participantes_ativ");
            entity.Property(e => e.Receita)
                .HasPrecision(10, 2)
                .HasColumnName("receita");

            entity.HasOne(d => d.IdRelatorioNavigation).WithMany(p => p.Relatorioespecificos)
                .HasForeignKey(d => d.IdRelatorio)
                .HasConstraintName("relatorioespecifico_id_relatorio_fkey");
        });

        modelBuilder.Entity<Relatoriogeral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("relatoriogeral_pkey");

            entity.ToTable("relatoriogeral");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdRelatorio).HasColumnName("id_relatorio");
            entity.Property(e => e.MaisPopul)
                .HasMaxLength(255)
                .HasColumnName("mais_popul");
            entity.Property(e => e.NumPorCat).HasColumnName("num_por_cat");
            entity.Property(e => e.TotalPart).HasColumnName("total_part");

            entity.HasOne(d => d.IdRelatorioNavigation).WithMany(p => p.Relatoriogerals)
                .HasForeignKey(d => d.IdRelatorio)
                .HasConstraintName("relatoriogeral_id_relatorio_fkey");
        });

        modelBuilder.Entity<Tipobilhete>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipobilhete_pkey");

            entity.ToTable("tipobilhete");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdBilhete).HasColumnName("id_bilhete");
            entity.Property(e => e.Normal).HasColumnName("normal");
            entity.Property(e => e.Vip).HasColumnName("vip");

            entity.HasOne(d => d.IdBilheteNavigation).WithMany(p => p.Tipobilhetes)
                .HasForeignKey(d => d.IdBilhete)
                .HasConstraintName("tipobilhete_id_bilhete_fkey");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("utilizador_pkey");

            entity.ToTable("utilizador");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .HasColumnName("contacto");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<ViewEventosAtividade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_eventos_atividades");

            entity.Property(e => e.AtividadeId).HasColumnName("atividade_id");
            entity.Property(e => e.AtividadeNome)
                .HasMaxLength(255)
                .HasColumnName("atividade_nome");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.EventoNome)
                .HasMaxLength(255)
                .HasColumnName("evento_nome");
        });

        modelBuilder.Entity<ViewEventosOrganizadore>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_eventos_organizadores");

            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .HasColumnName("contacto");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.EventoNome)
                .HasMaxLength(255)
                .HasColumnName("evento_nome");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.Local)
                .HasMaxLength(255)
                .HasColumnName("local");
            entity.Property(e => e.OrganizadorId).HasColumnName("organizador_id");
            entity.Property(e => e.OrganizadorNome)
                .HasMaxLength(255)
                .HasColumnName("organizador_nome");
            entity.Property(e => e.PrecoIngresso)
                .HasPrecision(10, 2)
                .HasColumnName("preco_ingresso");
        });

        modelBuilder.Entity<ViewParticipantesBilhete>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_participantes_bilhetes");

            entity.Property(e => e.BilheteId).HasColumnName("bilhete_id");
            entity.Property(e => e.Contacto)
                .HasMaxLength(20)
                .HasColumnName("contacto");
            entity.Property(e => e.Descricao).HasColumnName("descricao");
            entity.Property(e => e.ParticipanteId).HasColumnName("participante_id");
            entity.Property(e => e.ParticipanteNome)
                .HasMaxLength(255)
                .HasColumnName("participante_nome");
            entity.Property(e => e.Quantidade).HasColumnName("quantidade");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<ViewRelatoriosEvento>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_relatorios_eventos");

            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.Informacoes).HasColumnName("informacoes");
            entity.Property(e => e.MaisPopul)
                .HasMaxLength(255)
                .HasColumnName("mais_popul");
            entity.Property(e => e.NumParticipantesAtiv).HasColumnName("num_participantes_ativ");
            entity.Property(e => e.NumPorCat).HasColumnName("num_por_cat");
            entity.Property(e => e.Receita)
                .HasPrecision(10, 2)
                .HasColumnName("receita");
            entity.Property(e => e.RelatorioEspecificoId).HasColumnName("relatorio_especifico_id");
            entity.Property(e => e.RelatorioGeralId).HasColumnName("relatorio_geral_id");
            entity.Property(e => e.RelatorioId).HasColumnName("relatorio_id");
            entity.Property(e => e.TotalPart).HasColumnName("total_part");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
