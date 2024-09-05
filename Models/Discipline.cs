using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Discipline
{

    [Display(Name = "Дисциплина")]
    public int Disciplinecd { get; set; }
    [Display(Name = "Дисциплина")]
    public string Disciplinename { get; set; } = null!;

    public int Usercd { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
    [ValidateNever]
    public virtual User UsercdNavigation { get; set; } = null!;

    public virtual ICollection<Group> Groupcds { get; set; } = new List<Group>();
}
