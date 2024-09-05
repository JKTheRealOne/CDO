using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CDO.Models;

public partial class Group
{

    [Display(Name = "Группа")]
    public int Groupcd { get; set; }
    [Display(Name = "Номер группы")]
    public int Number { get; set; }
    [Display(Name = "Дата начала обучения")]
    public DateOnly StartDate { get; set; }
    [Display(Name = "Дата окончания обучения")]
    public DateOnly? FinishDate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Discipline> Disciplinecds { get; set; } = new List<Discipline>();
}
