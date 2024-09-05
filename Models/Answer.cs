using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Answer
{
    [Display(Name = "Ответ")]
    public int Answercd { get; set; }

    [Display(Name = "Вопрос")]
    public int Questioncd { get; set; }
    [Display(Name = "Ответ")]
    public string Answernm { get; set; } = null!;
    [Display(Name = "Правильность")]
    public bool Isright { get; set; }
    [ValidateNever]
    public virtual Question QuestioncdNavigation { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
