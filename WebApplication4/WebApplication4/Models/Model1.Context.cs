﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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
        public virtual DbSet<Banco> Banco { get; set; }
        public virtual DbSet<Promociones> Promociones { get; set; }
        public virtual DbSet<TipoTarjeta> TipoTarjeta { get; set; }
        public virtual DbSet<Regalo> Regalo { get; set; }
        public virtual DbSet<RegaloXCuenta> RegaloXCuenta { get; set; }
        public virtual DbSet<TipoDeCambio> TipoDeCambio { get; set; }
        public virtual DbSet<Organizador> Organizador { get; set; }
        public virtual DbSet<PuntoVenta> PuntoVenta { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Categoria> Categoria { get; set; }
        public virtual DbSet<Pago> Pago { get; set; }
        public virtual DbSet<Tarjeta> Tarjeta { get; set; }
        public virtual DbSet<VentasXFuncion> VentasXFuncion { get; set; }
        public virtual DbSet<Turno> Turno { get; set; }
        public virtual DbSet<TurnoSistema> TurnoSistema { get; set; }
        public virtual DbSet<Politicas> Politicas { get; set; }
        public virtual DbSet<Asientos> Asientos { get; set; }
        public virtual DbSet<PrecioEvento> PrecioEvento { get; set; }
        public virtual DbSet<Ventas> Ventas { get; set; }
        public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }
        public virtual DbSet<AsientosXFuncion> AsientosXFuncion { get; set; }
        public virtual DbSet<Comentarios> Comentarios { get; set; }
        public virtual DbSet<Funcion> Funcion { get; set; }
        public virtual DbSet<PeriodoVenta> PeriodoVenta { get; set; }
        public virtual DbSet<ZonaEvento> ZonaEvento { get; set; }
        public virtual DbSet<Eventos> Eventos { get; set; }
        public virtual DbSet<ZonaxFuncion> ZonaxFuncion { get; set; }
        public virtual DbSet<HoraReserva> HoraReserva { get; set; }
        public virtual DbSet<Local> Local { get; set; }
    }
}
