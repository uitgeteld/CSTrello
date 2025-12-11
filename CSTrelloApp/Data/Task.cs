using System.ComponentModel.DataAnnotations;

namespace CSTrelloApp.Data;
public class Task
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string AssignedTo  { get; set; }
}

