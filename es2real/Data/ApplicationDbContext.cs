using ES2Real.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for each model
    public DbSet<Atividade> Atividades { get; set; }
    public DbSet<Bilhete> Bilhetes { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Bilhete_Participante> BilheteParticipante { get; set; } 
    public DbSet<Evento_RelatorioEspecifico> EventoRelatoriosEspecificos { get; set; }
    public DbSet<Evento_RelatorioGeral> EventoRelatoriosGerais { get; set; }
    public DbSet<Evento_Atividade> EventoAtividades { get; set; }
    public DbSet<Organizador> Organizadores { get; set; }
    public DbSet<Participante> Participantes { get; set; }
    public DbSet<RelatorioEspecifico> RelatoriosEspecificos { get; set; }
    public DbSet<RelatorioGeral> RelatoriosGerais { get; set; }
    public DbSet<UtilizadorAuth> UtilizadorAuth { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Chaves Compostas e Relacionamentos Many-to-Many

        // Many-to-Many: Evento <-> Atividade
        modelBuilder.Entity<Evento_Atividade>()
            .HasKey(ea => new { ea.IdEvento, ea.IdAtividade });

        modelBuilder.Entity<Evento_Atividade>()
            .HasOne(ea => ea.Evento)
            .WithMany(e => e.EventoAtividades)
            .HasForeignKey(ea => ea.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Evento_Atividade>()
            .HasOne(ea => ea.Atividade)
            .WithMany(a => a.EventoAtividades)
            .HasForeignKey(ea => ea.IdAtividade)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-Many: Participante <-> Bilhete
        modelBuilder.Entity<Bilhete_Participante>()
            .HasKey(bp => new { bp.IdParticipante, bp.IdBilhete });

        modelBuilder.Entity<Bilhete_Participante>()
            .HasOne(bp => bp.Participante)
            .WithMany(p => p.BilheteParticipante)
            .HasForeignKey(bp => bp.IdParticipante)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bilhete_Participante>()
            .HasOne(bp => bp.Bilhete)
            .WithMany(b => b.BilheteParticipante)
            .HasForeignKey(bp => bp.IdBilhete)
            .OnDelete(DeleteBehavior.Cascade);


        // Many-to-Many: Evento <-> RelatorioEspecifico
        modelBuilder.Entity<Evento_RelatorioEspecifico>()
            .HasKey(ere => new { ere.IdEvento, ere.IdRelatorioEspecifico });

        modelBuilder.Entity<Evento_RelatorioEspecifico>()
            .HasOne(ere => ere.Evento)
            .WithMany(e => e.EventoRelatoriosEspecificos)
            .HasForeignKey(ere => ere.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Evento_RelatorioEspecifico>()
            .HasOne(ere => ere.RelatorioEspecifico)
            .WithMany(re => re.EventoRelatoriosEspecificos)
            .HasForeignKey(ere => ere.IdRelatorioEspecifico)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-Many: Evento <-> RelatorioGeral
        modelBuilder.Entity<Evento_RelatorioGeral>()
            .HasKey(erg => new { erg.IdEvento, erg.IdRelatorioGeral });

        modelBuilder.Entity<Evento_RelatorioGeral>()
            .HasOne(erg => erg.Evento)
            .WithMany(e => e.EventoRelatoriosGerais)
            .HasForeignKey(erg => erg.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Evento_RelatorioGeral>()
            .HasOne(erg => erg.RelatorioGeral)
            .WithMany(rg => rg.EventoRelatoriosGerais)
            .HasForeignKey(erg => erg.IdRelatorioGeral)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Key: Organizador -> UsuarioAuth
        modelBuilder.Entity<Organizador>()
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Key: Participante -> UsuarioAuth
        modelBuilder.Entity<Participante>()
            .HasOne(p => p.Utilizador)
            .WithMany()
            .HasForeignKey(p => p.IdUtilizador)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Bilhete>()
            .Property(b => b.Tipo)
            .HasConversion<string>();
    }
}
