using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class User
{
    public int Usercd { get; set; }
    [Display(Name = "Роль")]
    public int Rolecd { get; set; }
    [Display(Name = "Группа")]
    public int? Groupcd { get; set; }

    public bool? Teacher { get; set; }
    [Display(Name = "ФИО")]
    public string? Fio { get; set; }
    [Display(Name = "Логин")]
    public string? Login { get; set; }
    [Display(Name = "Пароль")]
    public string? Password { get; set; }
    [Display(Name = "Почта")]
    public string? Email { get; set; }

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
    [ValidateNever]
    public virtual Group? GroupcdNavigation { get; set; }

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    [ValidateNever]
    public virtual Role RolecdNavigation { get; set; } = null!;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
