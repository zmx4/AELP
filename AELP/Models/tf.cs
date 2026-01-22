using System;
using System.Collections.Generic;

namespace AELP.Models;

public partial class tf : Word
{
    public string word { get; set; } = null!;

    public string? translation { get; set; }
}
