using Repository;
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Address 
{
    public long Id { get; set; }

    public string? Address1 { get; set; }

    public string? Pincode { get; set; }
}
