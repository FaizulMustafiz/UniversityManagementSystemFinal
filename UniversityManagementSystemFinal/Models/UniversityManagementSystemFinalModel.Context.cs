﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniversityManagementSystemFinal.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UniversityDBFinalEntities : DbContext
    {
        public UniversityDBFinalEntities()
            : base("name=UniversityDBFinalEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AssignCourse> AssignCourses { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Day> Days { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<EnrollCourse> EnrollCourses { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<ResultEntry> ResultEntries { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomAllocation> RoomAllocations { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
    }
}
