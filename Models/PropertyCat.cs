﻿using System.ComponentModel.DataAnnotations;

namespace Fastigheterse.Models;

public class PropertyCat
{

    public int Id { get; set; }


    [Required]
    public string? Name { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();

}

