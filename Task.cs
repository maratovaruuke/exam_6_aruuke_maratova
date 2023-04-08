using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exam_6_aruuke_maratova
{
    internal class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Performer { get; set; }
        public string? CreatedDate { get; set; }
        public string? FinishedDate { get; set; }
        public string Status { get; set; }

        public Task() { }
        public Task(int id, string title, string description, string performer)
        {
            Id = id;
            Title = title;
            Description = description;
            Performer = performer;
            CreatedDate = DateTime.Now.ToString();
            FinishedDate = null;
            Status = "new";
        }

    }
}
