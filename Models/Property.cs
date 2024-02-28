using System.ComponentModel.DataAnnotations;

namespace Fastigheterse.Models;

public class Property
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }


    public int? Size { get; set; }

    public string? Description { get; set; }


    [Range(1, int.MaxValue, ErrorMessage = "The price must be bigger than 0.")]
    public int? Price { get; set; }


    [Range(1, int.MaxValue, ErrorMessage = "The number of rooms must be bigger than 0.")]

    public int? Rooms { get; set; }

    public string? City { get; set; }

    public int? Zipcode { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public int PropertyCatId { get; set; }

    public PropertyCat? PropertyCat { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();

}

