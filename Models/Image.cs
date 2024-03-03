using System.ComponentModel.DataAnnotations;

namespace Fastigheterse.Models;

public class Image
{
    public int Id { get; set; }

    [Display(Name = "Image file path")]

    public string? Url { get; set; }

    public int PropertyId { get; set; }

    public Property? Property { get; set; }

}

