﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace WebApplication4.Models
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class inf245netsoft : DbContext
{
    public inf245netsoft()
        : base("name=inf245netsoft")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<CuentaUsuario> CuentaUsuario { get; set; }

    public virtual DbSet<Perfiles> Perfiles { get; set; }

    public virtual DbSet<Banco> Banco { get; set; }

    public virtual DbSet<Promociones> Promociones { get; set; }

    public virtual DbSet<TipoTarjeta> TipoTarjeta { get; set; }

    public virtual DbSet<Regalo> Regalo { get; set; }

    public virtual DbSet<RegaloXCuenta> RegaloXCuenta { get; set; }

    public virtual DbSet<TipoDeCambio> TipoDeCambio { get; set; }

    public virtual DbSet<Local> Local { get; set; }

    public virtual DbSet<Organizador> Organizador { get; set; }

    public virtual DbSet<PuntoVenta> PuntoVenta { get; set; }

    public virtual DbSet<Region> Region { get; set; }

    public virtual DbSet<Turno> Turno { get; set; }

    public virtual DbSet<TurnoSistema> TurnoSistema { get; set; }

    public virtual DbSet<Eventos> Eventos { get; set; }

    public virtual DbSet<EventosPrueba> EventosPrueba { get; set; }

    public virtual DbSet<Preferencias> Preferencias { get; set; }

}

}

