﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedDatabase.Models;

[Table("order")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idcustomer")]
    public int Idcustomer { get; set; }

    [Column("idproduct")]
    public int Idproduct { get; set; }

    [Column("Price")]
    [Precision(16, 2)]
    public decimal Price { get; set; }

    [Column("create_at", TypeName = "datetime")]
    public DateTime CreateAt { get; set; }

    [Column("idstatus")]
    public int Idstatus { get; set; }

    [Column("amount")]
    public int Amount { get; set; }
}