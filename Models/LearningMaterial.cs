using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class LearningMaterial
{
    public int LearningMaterialcd { get; set; }
    [Display(Name = "Название")]
    public string? Materialname { get; set; }

    public int Themecd { get; set; }
    [Display(Name = "Содержимое")]
    public string? Materialcontent { get; set; }

    public int? Materialvolume { get; set; }
    [ValidateNever]
    public virtual Theme ThemecdNavigation { get; set; } = null!;
}
