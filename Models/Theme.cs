using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Theme
{
    public int Themecd { get; set; }
    [Display(Name = "Тема")]
    public string Themename { get; set; } = null!;
    [Display(Name = "Объем")]
    public int Themevolume { get; set; }
    [Display(Name = "Дисциплина")]
    public int Disciplinecd { get; set; }
    [ValidateNever]
    [Display(Name = "Дисциплина")]
    public virtual Discipline DisciplinecdNavigation { get; set; } = null!;

    public virtual ICollection<LearningMaterial> LearningMaterials { get; set; } = new List<LearningMaterial>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
