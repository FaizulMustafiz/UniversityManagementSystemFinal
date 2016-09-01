using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UniversityManagementSystemFinal.Models;

namespace UniversityManagementSystemFinal.Controllers
{
    public class StudentController : Controller
    {
        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /Student/
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Department);
            return View(students.ToList());
        }

        // GET: /Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /Student/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            return View();
        }

        // POST: /Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                Regex phoneFormate = new Regex(@"^([0-9\(\)\/\+ \-]*)$");

                if (phoneFormate.IsMatch(student.StudentContact))
                {
                    student.RegistrationId = CreateRegistrationNo(student);
                    var regId = student.StudentName + " Resistration Id: " + student.RegistrationId;
                    TempData["regId"] = regId;
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Message"] = "Contact Number format is not valid";
                    RedirectToAction("Create");
                }

            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", student.DepartmentId);
            return View(student);
        }



        private string CreateRegistrationNo(Student student)
        {
            int id =
                db.Students.Count(s => (s.DepartmentId == student.DepartmentId) && (s.Date.Year == student.Date.Year)) +
                1;

            Department aDepartment = db.Departments.Where(d => d.DepartmentId == student.DepartmentId).FirstOrDefault();
            string registrationId = aDepartment.DepartmentCode + "-" + student.Date.Year + "-";

            string addZero = "";
            int len = 3 - id.ToString().Length;
            for (int i = 0; i < len; i++)
            {
                addZero = "0" + addZero;
            }
            return registrationId + addZero + id;
        }





        // GET: /Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", student.DepartmentId);
            return View(student);
        }

        // POST: /Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentId,StudentName,StudentEmail,StudentContact,Date,StudentAddress,RegistrationId,DepartmentId")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", student.DepartmentId);
            return View(student);
        }

        // GET: /Student/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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


        public JsonResult CheckEmail(string StudentEmail)
        {
            return Json((!db.Students.Any(s => s.StudentEmail == StudentEmail)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPhone(string StudentContact)
        {
            return Json((!db.Students.Any(s => s.StudentContact == StudentContact)), JsonRequestBehavior.AllowGet);
        }
    }
}
