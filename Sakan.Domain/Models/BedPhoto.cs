﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Sakan.Domain.Models;

public partial class BedPhoto
{
    public int Id { get; set; }

    public int? BedId { get; set; }

    public string PhotoUrl { get; set; }

    public virtual Bed Bed { get; set; }
}