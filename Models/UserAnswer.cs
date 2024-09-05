using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace CDO.Models;

public partial class UserAnswer
{
    public int Useranswercd { get; set; }

    public int Progresscd { get; set; }

    public int Questioncd { get; set; }

    public int? Answercd { get; set; }
    [ValidateNever]
    public virtual Answer? AnswercdNavigation { get; set; }
    [ValidateNever]
    public virtual Progress ProgresscdNavigation { get; set; } = null!;
    [ValidateNever]
    public virtual Question QuestioncdNavigation { get; set; } = null!;
}
