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
        public virtual DbSet<Perfiles> Perfiles { get; set; }
        public virtual DbSet<Banco> Banco { get; set; }
        public virtual DbSet<Promociones> Promociones { get; set; }
        public virtual DbSet<TipoTarjeta> TipoTarjeta { get; set; }
        public virtual DbSet<Regalo> Regalo { get; set; }
        public virtual DbSet<RegaloXCuenta> RegaloXCuenta { get; set; }
    }
}
