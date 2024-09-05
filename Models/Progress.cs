using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Progress
{
    public int Progresscd { get; set; }

    public int Usercd { get; set; }

    public int Testcd { get; set; }
    [Display(Name = "Оценка")]
    public int? Progressgrade { get; set; }
    [Display(Name = "Дата прохождения")]
    public DateTime Progressdate { get; set; }
    [Display(Name = "Длительность")]
    public TimeSpan? Progressduration { get; set; }
    [ValidateNever]
    public virtual Test TestcdNavigation { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    [ValidateNever]
    public virtual User UsercdNavigation { get; set; } = null!;
}
