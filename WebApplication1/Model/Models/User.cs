using Repository;
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class User 
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Contact { get; set; }
}
