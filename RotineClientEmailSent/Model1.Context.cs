﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RotineClientEmailSent
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BB_DB_DEVEntities : DbContext
    {
        public BB_DB_DEVEntities()
            : base("name=BB_DB_DEVEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BB_Clientes> BB_Clientes { get; set; }
        public virtual DbSet<BB_Proposal> BB_Proposal { get; set; }
        public virtual DbSet<LD_Contrato> LD_Contrato { get; set; }
        public virtual DbSet<LD_PA5_DocumentProposal> LD_PA5_DocumentProposal { get; set; }
        public virtual DbSet<LD_PA5_DocumentType> LD_PA5_DocumentType { get; set; }
        public virtual DbSet<LD_PA5_EmailConfigSent> LD_PA5_EmailConfigSent { get; set; }
        public virtual DbSet<LD_Email_Log> LD_Email_Log { get; set; }
    }
}
