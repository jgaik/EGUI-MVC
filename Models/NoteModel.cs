using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvcTest.Models;

namespace mvcTest.Models
{
    public class NoteModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreation { get; set; }

        [Display(Name = "In Categories: ")]
        [Required(ErrorMessage = "Please enter unique note title")]
        public string Categories { get; set; }

        [Display(Name = "Title of the note: ")]
        public string Title { get; set; }

        [Display(Name = "Content of the note: ")]
        public string Content { get; set; }

        [Display(Name = "Category name: ")]
        public string CategoryNew { get; set; }

        public NoteModel(){}
        public NoteModel(string fileNoteName)
        {
            string[] fileLines = System.IO.File.ReadAllLines(@"Contents/Notes/" + fileNoteName + ".txt");
            fileLines[0] = fileLines[0].Replace("category:", "").Replace(" ", "");
            fileLines[1] = fileLines[1].Replace("date:", "").Replace(" ", "");
            this.Title = fileNoteName;
            this.Categories = fileLines[0];
            this.DateCreation = Convert.ToDateTime(fileLines[1]);
            this.Content = String.Join(Environment.NewLine, fileLines.Skip(2));
        }
        public void RemoveCategory(string category)
        {
            var catList = this.Categories.Split(",").ToList();
            if (catList.Contains(category))
            {
                catList.Remove(category);
                this.Categories = String.Join(",", catList);
            }
        }

        public void Save(string originalName)
        {
            NoteModel orgNote = new NoteModel(originalName);
            if (this.Title != originalName)
                System.IO.File.Move(@"Contents/Notes/" + originalName + ".txt", @"Contents/Notes/" + this.Title + ".txt");
            List<string> fileContnet = new List<string>{
                "category: " + this.Categories,
                "date: " + orgNote.DateCreation.ToString("yyyy/MM/dd"),
                this.Content
            };

            System.IO.File.WriteAllLines(@"Contents/Notes/" + this.Title + ".txt", fileContnet);
        }

        public void Add()
        {
            List<string> fileContnet = new List<string>{
                "category: " + this.Categories,
                "date: " + DateTime.Now.ToString("yyyy/MM/dd"),
                this.Content
            };
            System.IO.File.WriteAllLines(@"Contents/Notes/" + this.Title + ".txt", fileContnet);
        }
    }
}