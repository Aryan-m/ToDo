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

        public ActionResult ToDoList()
        {
            ApplicationUser cur_user = Get_Cur_User();
            IEnumerable<Models.ToDo> model = db.ToDos.ToList().Where(x => x.user == cur_user).
                OrderBy(x => x.row_number);

            int completed = 0;
            foreach (Models.ToDo todo in model)
            {
                if (todo.is_done)
                    completed++;
            }

            ViewBag.percent_complete = Math.Round(100 * ((float)completed / model.Count()));
            ViewBag.tasks_left = model.Count() - completed;

            return PartialView("_ToDoList", model);
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: ToDoes/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit_Order(string id_list)
        {
            ApplicationUser cur_user = Get_Cur_User();
            List<int> ids = new List<int>();
            int id_counter = 1;
            ids = id_list.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).
                Select(int.Parse).ToList();
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

        // POST: ToDoes/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
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
