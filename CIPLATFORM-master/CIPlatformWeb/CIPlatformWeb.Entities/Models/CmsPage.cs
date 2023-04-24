using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIPlatformMain.Entities.Models;

public partial class CmsPage
{
    public long CmsPageId { get; set; }

    [Required(ErrorMessage = "Please enter a Title")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Please enter a Description")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Please enter a Slug")]
    public string Slug { get; set; } = null!;

    public int Status { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
