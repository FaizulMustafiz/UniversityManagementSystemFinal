using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniversityManagementSystemFinal.Models;

namespace UniversityManagementSystemFinal.Controllers
{
    public class ResultEntryController : Controller
    {
        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /ResultEntry/
        public ActionResult Index()
        {
            var resultentries = db.ResultEntries.Include(r => r.Course).Include(r => r.Grade).Include(r => r.Student);
            return View(resultentries.ToList());
        }

        // GET: /ResultEntry/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            return View(resultentry);
        }

        // GET: /ResultEntry/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "GradeName");
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId");
            return View();
        }

        // POST: /ResultEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResultEntry resultentry)
        {
            if (ModelState.IsValid)
            {
                var result =
                    db.ResultEntries.Count(
                        r => r.StudentId == resultentry.StudentId && r.CourseId == resultentry.CourseId) == 0;
                if (result)
                {
                    Grade aGrade = db.Grades.Where(g => g.GradeId == resultentry.GradeId).FirstOrDefault();

                    EnrollCourse resultEnrollCourse =
                        db.EnrollCourses.FirstOrDefault(r => r.CourseId == resultentry.CourseId && r.StudentId == resultentry.StudentId);
                    if (resultEnrollCourse!= null)
                    {
                        resultEnrollCourse.GradeName = aGrade.GradeName;
                    }
                    db.ResultEntries.Add(resultentry);
                    db.SaveChanges();
                    TempData["success"] = "Result Successfully Entered";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Already"] = "Result Of This Course has Already Been Assigned";
                    return RedirectToAction("Create");
                }

                
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "GradeName", resultentry.GradeId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            return View(resultentry);
        }


        public PartialViewResult StudentInfoLoad(int? studentId)
        {
            if (studentId!= null)
            {
                Student aStudent = db.Students.FirstOrDefault(s => s.StudentId == studentId);

                ViewBag.Name = aStudent.StudentName;
                ViewBag.Email = aStudent.StudentEmail;
                ViewBag.Dept = aStudent.Department.DepartmnetName;
                return PartialView("~/Views/Shared/_StudentInformation.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_StudentInformation.cshtml");
            }
        }




        // GET: /ResultEntry/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "GradeName", resultentry.GradeId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            return View(resultentry);
        }

        // POST: /ResultEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ResultEntryId,StudentId,CourseId,GradeId")] ResultEntry resultentry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resultentry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "GradeName", resultentry.GradeId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            return View(resultentry);
        }

        // GET: /ResultEntry/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            return View(resultentry);
        }

        // POST: /ResultEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResultEntry resultentry = db.ResultEntries.Find(id);
            db.ResultEntries.Remove(resultentry);
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

        public PartialViewResult CourseLoad(int? studentId)
        {
            List<EnrollCourse> enrollCourseList = new List<EnrollCourse>();
            List<Course> courseList = new List<Course>();

            if (studentId != null)
            {
                enrollCourseList = db.EnrollCourses.Where(e => e.StudentId == studentId).ToList();
                foreach (EnrollCourse anEnrollment in enrollCourseList)
                {
                    Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == anEnrollment.CourseId);
                    courseList.Add(aCourse);
                }
                ViewBag.CourseId = new SelectList(courseList, "CourseId", "CourseName");

            }
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");
        }

        public ActionResult ViewResult()
        {
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId");
            return View();
        }

        public PartialViewResult ResultInfoLoad(int? studentId)
        {
            List<EnrollCourse> enrollCourseList = new List<EnrollCourse>();

            if (studentId != null)
            {
                enrollCourseList = db.EnrollCourses.Where(r => r.StudentId == studentId).ToList();
                if (enrollCourseList.Count==0)
                {
                    Student aStudent = db.Students.Find(studentId);
                    ViewBag.NotEnrolled = aStudent.RegistrationId + "    : has not taken any course";
                }
            }

            return PartialView("~/Views/shared/_resultInformation.cshtml", enrollCourseList);
        }

    }
}
