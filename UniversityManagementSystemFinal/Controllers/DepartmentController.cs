﻿using System;
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
    public class DepartmentController : Controller
    {
        private UniversityDBFinalEntities db = new UniversityDBFinalEntities();

        // GET: /Department/
        public ActionResult Index()
        {
            return View(db.Departments.ToList());
        }

        // GET: /Department/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: /Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,DepartmentCode,DepartmnetName")] Department department)
        {
            if (ModelState.IsValid)
            {
                if (!db.Departments.Any(x => x.DepartmentCode == department.DepartmentCode))
                {
                    if (!db.Departments.Any(x => x.DepartmnetName == department.DepartmnetName))
                    {
                        db.Departments.Add(department);
                        db.SaveChanges();
                        TempData["Success"] = "Department Added";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ViewBag.Message = "Department Name Already Exists";
                    }
                }
                else
                {
                    ViewBag.Message = "Department Code Already Exists";
                }
            }

            return View(department);
        }

        // GET: /Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentCode,DepartmnetName")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: /Department/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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

        public JsonResult CheckCode(string DepartmentCode)
        {
            return Json((!db.Departments.Any(x => x.DepartmentCode == DepartmentCode)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckName(string DepartmnetName)
        {
            return Json((!db.Departments.Any(x => x.DepartmnetName == DepartmnetName)), JsonRequestBehavior.AllowGet);
        }
    }
}
