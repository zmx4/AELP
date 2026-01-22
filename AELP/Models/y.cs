using System;
using System.Collections.Generic;

namespace AELP.Models;

public partial class y : Word
{
    public string word { get; set; } = null!;

    public string? translation { get; set; }
}
