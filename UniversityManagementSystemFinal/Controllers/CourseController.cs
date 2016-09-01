using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniversityManagementSystemFinal.Models;

namespace UniversityManagementSystemFinal.Controllers
{
    public class CourseController : Controller
    {
        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /Course/
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Department).Include(c => c.Semester);
            return View(courses.ToList());
        }

        // GET: /Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: /Course/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            ViewBag.SemesterId = new SelectList(db.Semesters, "SemesterId", "SemesterName");
            return View();
        }

        // POST: /Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CourseId,CourseCode,CourseName,CourseCradit,CourseDescription,SemesterId,DepartmentId,AssignTo,CourseStatus")] Course course)
        {
            if (ModelState.IsValid)
            {
                if (!db.Courses.Any(x => x.CourseCode == course.CourseCode))
                {
                    if (!db.Courses.Any(x => x.CourseName == course.CourseName))
                    {
                        db.Courses.Add(course);
                        db.SaveChanges();
                        TempData["Message"] = "Course Added Successfully";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ViewBag.Message = "Couse Name Already Exists";
                    }
                }
                else
                {
                    ViewBag.Message = "Couse Code Already Exists";
                }
                
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
            ViewBag.SemesterId = new SelectList(db.Semesters, "SemesterId", "SemesterName", course.SemesterId);
            return View(course);
        }

        // GET: /Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
            ViewBag.SemesterId = new SelectList(db.Semesters, "SemesterId", "SemesterName", course.SemesterId);
            return View(course);
        }

        // POST: /Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CourseId,CourseCode,CourseName,CourseCradit,CourseDescription,SemesterId,DepartmentId,AssignTo")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
            ViewBag.SemesterId = new SelectList(db.Semesters, "SemesterId", "SemesterName", course.SemesterId);
            return View(course);
        }

        // GET: /Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: /Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult CheckCode(string CourseCode)
        {
            return Json((!db.Courses.Any(x => x.CourseCode == CourseCode)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckName(string CourseName)
        {
            return Json((!db.Courses.Any(x=>x.CourseName==CourseName)), JsonRequestBehavior.AllowGet);
        }


        public ActionResult UnassignCourses()
        {
            return View();
        }


        public JsonResult UnassignAllCourses(bool decision)
        {
            var courses = db.Courses.Where(m => m.CourseStatus ==true ).ToList();
            if (courses.Count == 0)
            {
                return Json(false);
            }
            else
            {
                foreach (var course in courses)
                {
                    course.CourseStatus = false;
                    course.AssignTo = null;
                    db.Courses.AddOrUpdate(course);
                    db.SaveChanges();
                }
                return Json(true);
            }
        }



    }
}
