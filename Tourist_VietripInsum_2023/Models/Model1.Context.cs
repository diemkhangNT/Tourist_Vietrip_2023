﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tourist_VietripInsum_2023.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TouristEntities1 : DbContext
    {
        public TouristEntities1()
            : base("name=TouristEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer_Guest> Customer_Guest { get; set; }
        public virtual DbSet<DetailTour> DetailTours { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<OrderCu> OrderCus { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Tour> Tours { get; set; }
        public virtual DbSet<TourType> TourTypes { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<TypeCu> TypeCus { get; set; }
        public virtual DbSet<TypeTicket> TypeTickets { get; set; }
        public virtual DbSet<VistLocation> VistLocations { get; set; }
    }
}
