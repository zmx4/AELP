using System;

namespace AELP.Data;

public class FavoritesDataModel
{
    public WordDataModel Word { get; set; }

    public bool IsCet4 { get; set; }

    public bool IsCet6 { get; set; }

    public bool IsHs { get; set; }

    public bool IsPh { get; set; }

    public bool IsTf { get; set; }

    public bool IsYs { get; set; }
    
    public bool IsFavorite { get; set; }
    
}