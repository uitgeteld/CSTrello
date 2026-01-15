namespace CSTrelloApp.Data;
using System.ComponentModel.DataAnnotations;

public class Task
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [StringLength(100)]
    public string AssignedTo { get; set; }
}