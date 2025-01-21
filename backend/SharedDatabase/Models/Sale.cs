﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedDatabase.Models;

[Table("sale")]
public partial class Sale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idcustomer")]
    public int Idcustomer { get; set; }

    [Required]
    [Column("idproduct")]
    [StringLength(45)]
    public int Idproduct { get; set; }

    [Column("idorder")]
    public int? Idorder { get; set; }

    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Column("create_at", TypeName = "datetime")]
    public DateTime CreateAt { get; set; }

    [Column("idstatus")]
    public int Idstatus { get; set; }

    [Column("amount")]
    public int Amount { get; set; }
}