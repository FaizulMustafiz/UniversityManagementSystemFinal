using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniversityManagementSystemFinal.Models;

namespace UniversityManagementSystemFinal.Controllers
{
    public class AssignCourseController : Controller
    {

        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /AssignCourse/
        public ActionResult Index()
        {
            var assigncourses = db.AssignCourses.Include(a => a.Department).Include(a => a.Course).Include(a => a.Teacher);
            return View(assigncourses.ToList());
        }

        // GET: /AssignCourse/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            return View(assigncourse);
        }

        // GET: /AssignCourse/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode");
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "TeacherName");
            return View();
        }

        // POST: /AssignCourse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AssignCourse assigncourse)
        {
            if (ModelState.IsValid)
            {
                var assignCourseList =
                    db.AssignCourses.Where(t => t.CourseId == assigncourse.CourseId && t.Course.CourseStatus == true)
                        .ToList();
                if (assignCourseList.Count > 0)
                {
                    TempData["Already"] = "Course Has Already Been Assigned";
                    return RedirectToAction("Create");
                }
                else
                {
                    Teacher aTeacher = db.Teachers.FirstOrDefault(t => t.TeacherId == assigncourse.TeacherId);
                    Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == assigncourse.CourseId);
                    List<AssignCourse> assignTeacher =
                        db.AssignCourses.Where(t => t.TeacherId == assigncourse.TeacherId).ToList();
                    AssignCourse assign = null;

                    if (assignTeacher.Count != 0)
                    {
                        assign = assignTeacher.Last();
                        assigncourse.RemaingCredit = assign.RemaingCredit;
                    }
                    else
                    {
                        assigncourse.RemaingCredit = aTeacher.CraditTaken;
                    }

                    if (assigncourse.RemaingCredit < aCourse.CourseCradit)
                    {
                        Session["Teacher"] = aTeacher;
                        Session["Course"] = aCourse;
                        Session["AssignedCourse"] = assigncourse;
                        Session["AssigneddCourseCheck"] = assign;
                        return RedirectToAction("AskToAssign");
                    }

                    assigncourse.CreditTaken = aTeacher.CraditTaken;

                    if (assign == null)
                    {
                        assigncourse.RemaingCredit = aTeacher.CraditTaken - aCourse.CourseCradit;
                    }
                    else
                    {
                        assigncourse.RemaingCredit = assign.RemaingCredit - aCourse.CourseCradit;
                    }
                    aCourse.CourseStatus = true;
                    aCourse.AssignTo = aTeacher.TeacherName;

                    db.AssignCourses.Add(assigncourse);
                    db.SaveChanges();
                    TempData["success"] = "Course Successfully Assigned!";
                    return RedirectToAction("Create");
                }
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", assigncourse.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", assigncourse.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "TeacherName", assigncourse.TeacherId);
            return View(assigncourse);
        }


        public ActionResult AskToAssign()
        {
            Teacher aTeacher = (Teacher)Session["Teacher"];
            Course aCourse = (Course)Session["Course"];
            AssignCourse assign = (AssignCourse)Session["AssigneddCourseCheck"];
            double remainingCredite = 0.0;
            if (assign == null)
                remainingCredite = aTeacher.CraditTaken;
            else
            {
                remainingCredite = assign.RemaingCredit;
            }
            if (remainingCredite < 0)
            {
                ViewBag.Message = aTeacher.TeacherName
                + " Credit Limit Is Over. And The Course Credit  : " + aCourse.CourseCode
                + " Is " + aCourse.CourseCradit
                + "  ! Still You Want To Assign This Course To This Teacher ?";
            }
            else
            {
                ViewBag.Message = aTeacher.TeacherName
                + " has only " + remainingCredite
                + " Credits Remaining . But, The Credit  : " + aCourse.CourseCode
                + " Is " + aCourse.CourseCradit
                + "  ! Still You Want To Assign This Course To This Teacher ?";
            }

            return View();
        }


        public ActionResult AssignConfirmed()
        {
            Teacher aTeacher = (Teacher)Session["Teacher"];

            AssignCourse assigncourse = (AssignCourse)Session["AssignedCourse"];
            AssignCourse assign = (AssignCourse)Session["AssigneddCourseCheck"];
            Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == assigncourse.CourseId);


            assigncourse.CreditTaken = aTeacher.CraditTaken;
            if (assign == null)
            {
                assigncourse.RemaingCredit = aTeacher.CraditTaken - aCourse.CourseCradit;
            }
            else
            {
                assigncourse.RemaingCredit = assign.RemaingCredit - aCourse.CourseCradit;
            }

            aCourse.AssignTo = aTeacher.TeacherName;

            db.AssignCourses.Add(assigncourse);
            db.SaveChanges();
            TempData["success"] = "Course Is Assigned";
            return View();


        }



        // GET: /AssignCourse/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", assigncourse.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", assigncourse.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "TeacherName", assigncourse.TeacherId);
            return View(assigncourse);
        }

        // POST: /AssignCourse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssignCourseId,DepartmentId,TeacherId,CreditTaken,RemaingCredit,CourseId")] AssignCourse assigncourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assigncourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", assigncourse.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", assigncourse.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "TeacherName", assigncourse.TeacherId);
            return View(assigncourse);
        }

        // GET: /AssignCourse/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            return View(assigncourse);
        }

        // POST: /AssignCourse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            db.AssignCourses.Remove(assigncourse);
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

        public ActionResult LoadTeacher(int? departmentId)
        {
            var teacherList = db.Teachers.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.TeacherId = new SelectList(teacherList, "TeacherId", "TeacherName");
            return PartialView("~/Views/Shared/_FillteredTeacher.cshtml");
        }

        public ActionResult LoadCourse(int? departmentId)
        {
            var courseList = db.Courses.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.CourseId = new SelectList(courseList, "CourseId", "CourseName");
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");

        }


        public PartialViewResult TeacherInfoLoad(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher aTeacher = db.Teachers.FirstOrDefault(s => s.TeacherId == teacherId);
                ViewBag.Credit = aTeacher.CraditTaken;
                List<AssignCourse> assignTeachers =
                        db.AssignCourses.Where(t => t.TeacherId == teacherId).ToList();
                AssignCourse assign = null;
                if (assignTeachers.Count != 0)
                {
                    assign = assignTeachers.Last();
                }
                if (assign == null)
                {
                    ViewBag.RemainingCredit = aTeacher.CraditTaken;
                }
                else
                {
                    ViewBag.RemainingCredit = assign.RemaingCredit;
                }

                return PartialView("~/Views/Shared/_TeachersCreditInfo.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_TeachersCreditInfo.cshtml");
            }

        }

        public PartialViewResult CourseInfoLoad(int? courseId)
        {
            if (courseId != null)
            {
                Course aCourse = db.Courses.FirstOrDefault(s => s.CourseId == courseId);
                ViewBag.Code = aCourse.CourseCode;
                ViewBag.Credit = aCourse.CourseCradit;
                return PartialView("~/Views/Shared/_CourseInfo.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_CourseInfo.cshtml");
            }

        }

        public ActionResult ViewCourseStatus()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            return View();
        }

        public PartialViewResult CourseStatusLoad(int? departmentId)
        {
            List<Course> courseList = new List<Course>();
            if (departmentId != null)
            {
                courseList = db.Courses.Where(r => r.DepartmentId == departmentId).ToList();
                if (courseList.Count == 0)
                {
                    ViewBag.NotAssigned = "Department Empty";
                }
            }


            return PartialView("~/Views/shared/_coursestatus.cshtml", courseList);
        }

    }
}
