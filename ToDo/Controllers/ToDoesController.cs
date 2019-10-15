using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;
using Microsoft.AspNet.Identity;


namespace ToDo.Controllers
{
    [Authorize]
    public class ToDoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Gets the current user
        private ApplicationUser Get_Cur_User()
        {
            string user_Id = User.Identity.GetUserId();
            return db.Users.FirstOrDefault(x => x.Id == user_Id);
        }

        // GET: ToDoes
        public ActionResult Index()
        {
            return View();
        }

        // Returns a partial view of a list of to do items for the current user
        public ActionResult ToDoList()
        {
            ApplicationUser cur_user = Get_Cur_User();
            IEnumerable<Models.ToDo> model = db.ToDos.ToList().Where(x => x.user == cur_user).
                OrderBy(x => x.row_number);

            int completed = 0; // number of completed tasks
            foreach (Models.ToDo todo in model)
            {
                if (todo.is_done)
                    completed++;
            }

            // Calculations for the progress bar based on the "completed" variable
            ViewBag.percent_complete = Math.Round(100 * ((float)completed / model.Count()));
            ViewBag.tasks_left = model.Count() - completed;

            return PartialView("_ToDoList", model);
        }

        // Called onto by an AJAX request to create a new to do item and return the partial view of new list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,content,is_done,row_number")] ToDo.Models.ToDo toDo)
        {
            ApplicationUser cur_user = Get_Cur_User();
            if (ModelState.IsValid)
            {
                toDo.user = cur_user;
                toDo.is_done = false;
                toDo.row_number = toDo.ID;
                db.ToDos.Add(toDo);
                db.SaveChanges();
            }

            return ToDoList();
        }

        // Edits the Checked property of a to do item and returns the partial view of new list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Checked(int? id, bool val)
        {
            ApplicationUser cur_user = Get_Cur_User();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo.Models.ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else if (toDo.user != cur_user)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            else
            {
                toDo.is_done = val;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
            }

            return ToDoList();
        }

        // Edits the Important property of a to do item and returns the partial view of new list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Important(int? id, bool val)
        {
            ApplicationUser cur_user = Get_Cur_User();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo.Models.ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else if (toDo.user != cur_user)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            else
            {
                toDo.is_important = val;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
            }

            return ToDoList();
        }

        // When items are dragged using jQuery UI, this function preserves the new order by reflecting changes to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Order(string id_list)
        {
            ApplicationUser cur_user = Get_Cur_User();
            List<int> ids = new List<int>(); // will hold 
            int id_counter = 1; // Determines each row number of an item
            ids = id_list.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).
                Select(int.Parse).ToList();

            // sorts the list based on new row numbers
            foreach(var id in ids)
            {
                try
                {
                    Models.ToDo toDo = db.ToDos.Where(x => x.ID == id).FirstOrDefault();
                    if (toDo.user != cur_user)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
                    toDo.row_number = id_counter;
                    db.Entry(toDo).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    continue;
                }
                id_counter++;
            }
            
            return ToDoList();
        }

        // Deletes a to do item and returns new list as partial view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser cur_user = Get_Cur_User();

            ToDo.Models.ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else if (toDo.user != cur_user)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            db.ToDos.Remove(toDo);
            db.SaveChanges();
            return ToDoList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
