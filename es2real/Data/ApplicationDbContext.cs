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
    public DbSet<Evento_Bilhete> EventoBilhetes { get; set; } 
    public DbSet<Evento_RelatorioEspecifico> EventoRelatoriosEspecificos { get; set; }
    public DbSet<Evento_RelatorioGeral> EventoRelatoriosGerais { get; set; }
    public DbSet<Evento_Atividade> EventoAtividades { get; set; }
    public DbSet<Organizador> Organizadores { get; set; }
    public DbSet<Participante> Participantes { get; set; }
    public DbSet<RelatorioEspecifico> RelatoriosEspecificos { get; set; }
    public DbSet<RelatorioGeral> RelatoriosGerais { get; set; }
    public DbSet<UsuarioAuth> UsuariosAuth { get; set; }

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

        // Many-to-Many: Evento <-> Bilhete
        modelBuilder.Entity<Evento_Bilhete>()
            .HasKey(eb => new { eb.IdEvento, eb.IdBilhete });

        modelBuilder.Entity<Evento_Bilhete>()
            .HasOne(eb => eb.Evento)
            .WithMany(e => e.EventoBilhetes)
            .HasForeignKey(eb => eb.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Evento_Bilhete>()
            .HasOne(eb => eb.Bilhete)
            .WithMany(b => b.EventoBilhetes)
            .HasForeignKey(eb => eb.IdBilhete)
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
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Bilhete>()
            .Property(b => b.Tipo)
            .HasConversion<string>();
    }
}
