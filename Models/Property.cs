using System.ComponentModel.DataAnnotations;

namespace Fastigheterse.Models;

public class Property
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Display(Name = "Size in m2")]
    public int? Size { get; set; }

    public string? Description { get; set; }


    [Range(1, int.MaxValue, ErrorMessage = "The price must be bigger than 0.")]

    public int? Price { get; set; }


    [Range(1, int.MaxValue, ErrorMessage = "The number of rooms must be bigger than 0.")]

    public int? Rooms { get; set; }

    public string? City { get; set; }

    [Display(Name = "Zip code")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip code must be exactly 5 numbers long")]

    public int? Zipcode { get; set; }

    [Display(Name = "Creation date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Display(Name = "Created by user")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Property Category")]
    public int PropertyCatId { get; set; }

    [Display(Name = "Property Category")]
    public PropertyCat? PropertyCat { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();

}

