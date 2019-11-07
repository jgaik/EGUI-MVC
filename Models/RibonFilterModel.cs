
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvcTest.Models
{
    public class RibonFilterModel 
    {
        [Display(Name = "From: ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dateFrom { get; set; }

        [Display(Name = "To: ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dateTo { get; set; }

    
        [Display(Name = "Category: ")]
        public string Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public List<NoteModel> Notes { get; set; }
        public NoteModel NoteEdit { get; set; }

        public string NoteCommand { get; set; }
    }
}