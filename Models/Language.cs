using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string IsoCode { get; set; } = null!;

    public string EnglishName { get; set; } = null!;

    public string? NativeName { get; set; }
}
