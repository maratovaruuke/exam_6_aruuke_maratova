using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exam_6_aruuke_maratova
{
    internal class Task
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Performer { get; set; }
        DateOnly CreatedDate { get; set; }  
        DateOnly FinishedDate { get; set; }
        string Status { get; set; }

    }
}
