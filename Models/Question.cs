using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Question
{
    public int Questioncd { get; set; }
    [Display(Name = "Вопрос")]
    public string Questionnm { get; set; } = null!;

    public int Testcd { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    [ValidateNever]
    public virtual Test TestcdNavigation { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
