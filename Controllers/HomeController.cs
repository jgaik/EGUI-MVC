using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using mvcTest.Models;

namespace mvcTest.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(DateTime dateFrom, DateTime dateTo, string Category, int? page = 1)
        {
            this.ViewBag.PageSize = 3;
            var categories = GetAllCategories();
            var model = new RibonFilterModel();

            model.dateFrom = dateFrom;
            model.dateTo = dateTo;
            model.Category = Category;
            model.Categories = GetSelectListItems(categories);
            model.Notes = GetNotes(dateFrom, dateTo, Category).ToList();
            this.ViewBag.MaxPage = model.Notes.Count()/this.ViewBag.PageSize + (model.Notes.Count() % this.ViewBag.PageSize == 0 ? 0 : 1);
            if (this.ViewBag.MaxPage == 0) this.ViewBag.MaxPage = 1;
            this.ViewBag.Page = page;
            return View(model);
        }

        public IActionResult AddNote(RibonFilterModel model)
        {
            if (CheckNoteName(model.NoteEdit.Title))
            {
                model.NoteEdit.Add();
                return RedirectToAction("Index", new {
                    dateFrom = model.dateFrom == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateFrom),
                    dateTo = model.dateTo == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateTo),
                    category = model.Category == null ? "" : model.Category} );
            } else
            {
                ViewBag.Valid = false;
                return View("Note", model);
            }
        }

        public IActionResult SaveNote(RibonFilterModel model, string currentTitle)
        {
            if (currentTitle == model.NoteEdit.Title || CheckNoteName(model.NoteEdit.Title))
            {    
                model.NoteEdit.Save(currentTitle);
                return RedirectToAction("Index", new {
                    dateFrom = model.dateFrom == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateFrom),
                    dateTo = model.dateTo == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateTo),
                    category = model.Category == null ? "" : model.Category} );
            } else
            {
                ViewBag.Valid = false;
                return View("Note",model);
            }
        }

        public IActionResult DeleteNote(RibonFilterModel model, string noteTitle)
        {
            System.IO.File.Delete(@"Contents/Notes/" + noteTitle + ".txt");
            return RedirectToAction("Index", new {
                dateFrom = model.dateFrom == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateFrom),
                dateTo = model.dateTo == DateTime.MinValue ? "" : String.Format("{0:yyyy-MM-dd}", model.dateTo),
                category = model.Category == null ? "" : model.Category} );
        }

        public IActionResult Note(RibonFilterModel model, string noteTitle, string command)
        {
            if (model.NoteCommand == null && command != null)
                model.NoteCommand = command;
            if (noteTitle != null)
            {
                model.NoteEdit = new NoteModel(noteTitle);
                ViewBag.Valid = true;
                return View(model);
            } else
            {
                if (model.NoteEdit != null)
                {
                    if (model.NoteEdit.CategoryNew != null)
                    {
                        AddCategory(model.NoteEdit.CategoryNew);
                        if (model.NoteEdit.Categories == null)
                        {
                            model.NoteEdit.Categories = model.NoteEdit.CategoryNew;
                        } else
                        {
                            if (model.NoteEdit.Categories.Contains(model.NoteEdit.CategoryNew))
                            {
                                model.NoteEdit.RemoveCategory(model.NoteEdit.CategoryNew);
                            } else
                            {
                                model.NoteEdit.Categories += "," + model.NoteEdit.CategoryNew;
                            }
                        }
                        ModelState.Remove("NoteEdit.CategoryNew");
                        model.NoteEdit.CategoryNew = null;
                    }
                }
                return View(model);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void AddCategory(string category)
        {
            if (!GetAllCategories().Contains(category))
            {
                System.IO.File.AppendAllText(@"Contents/Categories.txt", Environment.NewLine + category);
            }
        }
        public IEnumerable<string> GetAllCategories() 
        {
            return System.IO.File.ReadAllLines(@"Contents/Categories.txt").ToList();
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }

            return selectList;
        }

        private IEnumerable<NoteModel> GetNotes(DateTime from, DateTime to, string cat)
        {
            if (to == DateTime.MinValue)
            {
                to = DateTime.Now;
            }
            List<NoteModel> notes = new List<NoteModel>();
            string[] filePaths = System.IO.Directory.GetFiles(@"Contents/Notes/", "*.txt");
            foreach(string file in filePaths)
            {
                NoteModel note = new NoteModel(System.IO.Path.GetFileNameWithoutExtension(file));

                if ((note.DateCreation >= from) && (note.DateCreation <= to))
                {
                    if ((cat == null) || (note.Categories.Contains(cat)))
                        notes.Add(note);
                }
            }
            return notes.OrderByDescending(n => n.DateCreation);
        }

        private bool CheckNoteName(string noteName)
        {
            if (noteName == null || noteName == "")
                return false;
            string[] filePaths = System.IO.Directory.GetFiles(@"Contents/Notes/", "*.txt");
            foreach(string file in filePaths)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(file) == noteName)
                    return false;
            }
            return true;
        }   
    }
}