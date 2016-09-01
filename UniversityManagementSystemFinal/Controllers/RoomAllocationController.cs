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
    public class RoomAllocationController : Controller
    {
        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /RoomAllocation/
        public ActionResult Index()
        {
            var roomallocations = db.RoomAllocations.Include(r => r.Course).Include(r => r.Day).Include(r => r.Department).Include(r => r.Room);
            return View(roomallocations.ToList());
        }

        // GET: /RoomAllocation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            return View(roomallocation);
        }

        // GET: /RoomAllocation/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode");
            ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName");
            return View();
        }

        // POST: /RoomAllocation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomAllocation roomallocation)
        {

            Room room = db.Rooms.Find(roomallocation.RoomId);
            Course course = db.Courses.Find(roomallocation.CourseId);
            Day day = db.Days.Find(roomallocation.DayId);


            if (ModelState.IsValid)
            {

                int givenForm, givenEnd;

                try
                {
                    givenForm = ConvertTimeIntoInt(roomallocation.StartTime);
                }
                catch (Exception exception)
                {
                    if (TempData["ErroeMessage3"] == null)
                    {
                        TempData["ErroeMessage1"] = exception.Message;
                    }
                    TempData["ErrorMessage4"] = TempData["ErrorMessage3"];
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);
                }

                try
                {
                    givenEnd = ConvertTimeIntoInt(roomallocation.EndTime);
                }
                catch (Exception anException)
                {
                    if (TempData["ErrorMessage3"] == null)
                    {
                        TempData["ErrorMessage2"] = anException.Message;
                    }
                    TempData["ErrorMessage5"] = TempData["ErrorMessage3"];
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);
                }


                if (givenEnd < givenForm)
                {
                    TempData["Message"] = "Class Should Start Before End (24 hours)";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);
                }
                if (givenEnd == givenForm)
                {
                    TempData["Message"] = "Class Should Start Before End (24 hours)";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);
                }

                if ((givenForm < 0) || (givenEnd >= (24 * 60)))
                {
                    TempData["Message"] = " 24 hours--format HH:MM";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);
                }

                List<RoomAllocation> overLapList = new List<RoomAllocation>();

                var DayRoomAlocationList =
                    db.RoomAllocations.Include(c => c.Course)
                        .Include(d => d.Day)
                        .Include(r => r.Room)
                        .Where(r => r.RoomId == roomallocation.RoomId && r.DayId == roomallocation.DayId).ToList();

                foreach (var aDayRoomAllocatin in DayRoomAlocationList)
                {
                    int DbForm = ConvertTimeIntoInt(aDayRoomAllocatin.StartTime);
                    int DbEnd = ConvertTimeIntoInt(aDayRoomAllocatin.EndTime);

                    if (
                        ((DbForm <= givenForm) && (givenForm < DbEnd))
                        || ((DbForm < givenEnd) && (givenEnd <= DbEnd))
                        || ((givenForm <= DbForm) && (DbEnd <= givenEnd))
                        )
                    {
                        overLapList.Add(aDayRoomAllocatin);
                    }
                }

                if (overLapList.Count == 0)
                {
                    roomallocation.RoomStatus = "Allocated";
                    db.RoomAllocations.Add(roomallocation);
                    db.SaveChanges();
                    TempData["Message"] = "Room : " + room.RoomName + " Allocated "
                    + " for course : " + course.CourseCode + " From " + roomallocation.StartTime
                    + " to " + roomallocation.EndTime + " on " + day.DayName;
                }
                else
                {
                    string message = "Room : " + room.RoomName + " Overlapped ";

                    foreach (var anOverlappedRoom in overLapList)
                    {
                        int dbForm = ConvertTimeIntoInt(anOverlappedRoom.StartTime);
                        int dbEnd = ConvertTimeIntoInt(anOverlappedRoom.EndTime);

                        string overlapStart, overlapEnd;

                        if ((dbForm <= givenForm) && (givenForm < dbEnd))
                            overlapStart = roomallocation.StartTime;

                        else
                            overlapStart = anOverlappedRoom.StartTime;

                        if ((dbForm < givenEnd) && (givenEnd <= dbEnd))
                            overlapEnd = anOverlappedRoom.EndTime;

                        else
                            overlapEnd = anOverlappedRoom.EndTime;
                        message += " Course : " + anOverlappedRoom.Course.CourseCode + " Start Time : "
                       + anOverlappedRoom.StartTime + " End Time : "
                       + anOverlappedRoom.EndTime + " Overlapped from : ";
                        message += overlapStart + " To " + overlapEnd;
                    }
                    TempData["Message"] = message + " on " + day.DayName;

                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "CourseCode", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
                    return View(roomallocation);

                }
                return RedirectToAction("Create");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", roomallocation.CourseId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", roomallocation.DepartmentId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
            return View(roomallocation);
        }


        private int ConvertTimeIntoInt(string timeStr)
        {
            try
            {
                if (TimeFormateCheck(timeStr))
                {
                    if (timeStr.Length == 4)
                    {
                        timeStr = "0" + timeStr;
                    }
                    string hour = timeStr[0].ToString() + timeStr[1];
                    string minute = timeStr[3].ToString() + timeStr[4];

                    if (Convert.ToInt32(minute) > 59 || Convert.ToInt32(minute) < 0)
                    {
                        TempData["Erroemessage3"] = "Minutes Must be Equal Or Less Then 59";
                        throw new Exception();
                    }
                    int time = (Convert.ToInt32(hour) * 60);
                    time += Convert.ToInt32(minute);
                    return time;
                }
                else
                {
                    throw new Exception("24 Houres Formate HH:MM");
                }

            }
            catch (FormatException aFormatException)
            {
                throw new FormatException(
                    "24 Houres Formate HH:MM", aFormatException);
            }
            catch (IndexOutOfRangeException aRangeException)
            {
                throw new IndexOutOfRangeException(
                    "24 Houres Formate HH:MM", aRangeException);
            }
            catch (Exception exception)
            {
                throw new Exception("24 Houres Formate HH:MM", exception);
            }
        }

        private bool TimeFormateCheck(string timeStr)
        {
            char[] list = timeStr.ToCharArray();
            foreach (var aChar in list)
            {
                if (aChar == ':')
                {
                    return true;
                }
            }
            return false;
        }


        public ActionResult RoomAllocationView()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode");
            List<Course> CourseList = db.Courses.ToList();

            foreach (var aCourse in CourseList)
            {
                aCourse.RoomAllocations
                    = db.RoomAllocations.Include(a => a.Room).Include(a => a.Day)
                    .Where(a => a.CourseId == aCourse.CourseId).ToList();
            }

            return View(CourseList);
        }


        // GET: /RoomAllocation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", roomallocation.CourseId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", roomallocation.DepartmentId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
            return View(roomallocation);
        }

        // POST: /RoomAllocation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomAllocationId,DepartmentId,CourseId,RoomId,DayId,StartTime,EndTime")] RoomAllocation roomallocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomallocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseCode", roomallocation.CourseId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "DayName", roomallocation.DayId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentCode", roomallocation.DepartmentId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "RoomName", roomallocation.RoomId);
            return View(roomallocation);
        }

        // GET: /RoomAllocation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            return View(roomallocation);
        }

        // POST: /RoomAllocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            db.RoomAllocations.Remove(roomallocation);
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

        public ActionResult LoadCourse(int? departmentId)
        {
            var courseList = db.Courses.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.CourseId = new SelectList(courseList, "CourseId", "CourseName");
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");
        }

        public PartialViewResult AllocatedRoomLoad(int? departmentId)
        {
            List<Course> courseList = null;
            if (departmentId != null)
            {
                courseList = db.Courses.Where(c => c.DepartmentId == departmentId).ToList();
            }

            foreach (var aCourse in courseList)
            {
                aCourse.RoomAllocations =
                    db.RoomAllocations.Include(a => a.Room)
                        .Include(a => a.Day)
                        .Where(a => a.CourseId == aCourse.CourseId).ToList();
            }
            if (courseList.Count == 0)
            {
                ViewBag.NoCourse = "Department Empty";
            }
            return PartialView("~/Views/shared/_RoomAllocationView.cshtml", courseList);
        }


        public ActionResult UnallocateRoom()
        {
            return View();
        }



        public JsonResult UnallocateAllRooms(bool name)
        {
            var roomInfo = db.RoomAllocations.Where(r => r.RoomStatus == "Allocated").ToList();
            if (roomInfo.Count == 0)
            {
                return Json(false);
            }
            else
            {
                foreach (var room in roomInfo)
                {
                    room.RoomStatus = null;
                    db.RoomAllocations.AddOrUpdate(room);
                    db.SaveChanges();
                }
                return Json(true);
            }
        }







    }
}
