using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Test
{

    [Display(Name = "Тест")]
    public int Testcd { get; set; }

    [Display(Name = "Дисциплина")]
    public int Disciplinecd { get; set; }

    [Display(Name = "Тема")]
    public int Themecd { get; set; }

    [Display(Name = "Пользователь")]
    public int Usercd { get; set; }
    [Display(Name = "Тест")]
    public string Testname { get; set; } = null!;
    [Display(Name = "Количество вопросов")]
    public int Testnumquest { get; set; }
    [Display(Name = "Продолжительность")]
    public TimeSpan? Testduration { get; set; }
    [ValidateNever]
    public virtual Discipline DisciplinecdNavigation { get; set; } = null!;

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    [ValidateNever]
    public virtual Theme ThemecdNavigation { get; set; } = null!;
    [ValidateNever]
    public virtual User UsercdNavigation { get; set; } = null!;
}
