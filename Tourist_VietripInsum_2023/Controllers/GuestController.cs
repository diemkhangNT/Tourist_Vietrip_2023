﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class GuestController : Controller
    {
        TouristEntities1 db = new TouristEntities1();
        // GET: Guest
        public ActionResult HomePageGuest()
        {
            return View();
        }
        
        private List<DiaDiemThamQuan> Lay_DiaDiem()
        {
            List<DiaDiemThamQuan> diaDiemThamQuans = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();

            var nhanvien = (from s in diaDiemThamQuans

                            join a in chiTietTours on s.MaDDTQ equals a.MaDDTQ
                            group s by s.MaDDTQ into g
                            select new
                            {
                                MaDD = g.FirstOrDefault().MaDDTQ,

                            }).ToList();
            List<DiaDiemThamQuan> d = new List<DiaDiemThamQuan>();

            foreach (var i in nhanvien)
            {
                foreach (var item in diaDiemThamQuans)
                {
                    if (item.MaDDTQ == i.MaDD)
                    {
                        d.Add(item);
                    }
                }

            }
            return d;
        }
        public ActionResult DiaDiemTour()
        {
            List<DiaDiemThamQuan> diadiemtim = Lay_DiaDiem();
            return PartialView(diadiemtim);
        }
        public ActionResult DiaDiemPartial()
        {

            List<DiaDiemThamQuan> diadiemtim = Lay_DiaDiem();

            List<TinhThanh> tinhThanhs = db.TinhThanhs.ToList();

            var item = (from s in diadiemtim

                            join a in tinhThanhs on s.MaTinh equals a.MaTinh
                            group a by a.MaTinh into g
                            select new
                            {
                                MaTinhThanh = g.FirstOrDefault().MaTinh,

                            }).ToList();

            List<TinhThanh> t = new List<TinhThanh>();

            foreach (var i in item)
            {
                foreach (var a in tinhThanhs)
                {
                    if (i.MaTinhThanh == a.MaTinh)
                    {
                        t.Add(a);
                    }
                }

            }

            //foreach (var item in diadiemtim)
            //{
            //    var c = db.TinhThanhs.Where(t => t.MaTinh == item.MaTinh).FirstOrDefault();
            //    tinhThanhs.Add(c);
            //}    
            
            return PartialView(t);
        }

        public ActionResult Tourinfomation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();       
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        //List tour tinh thanh

       
        private List<Tour> lay_Tour(string id)
        {
            List<Tour> tour = new List<Tour>();
            List<ChiTietTour> ct = db.ChiTietTours.Where(c => c.MaDDTQ == id).ToList();
            foreach (var item in ct)
            {
                
                var tourchon = db.Tours.Where(t => t.MaTour == item.MaTour).FirstOrDefault();
                if(tourchon.TrangThai!="Sắp ra mắt")
                {
                    tour.Add(tourchon);
                }    
                
            }
            return tour;
        }
       
        public ActionResult ListTourTinhThanh(string id)
        {
            
            List<Tour> tour = new List<Tour>();
            List<DiaDiemThamQuan> diaDiems = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> tours = db.Tours.ToList();
            Session["tentinh"] = id;
            var tinhthanh = (from s in diaDiems

                            join a in chiTietTours on s.MaDDTQ equals a.MaDDTQ
                            where s.MaTinh==id
                            group a by a.MaTour into g
                            select new
                            {
                                MaTour = g.FirstOrDefault().MaTour,

                            }).ToList();
            
            foreach (var item in tinhthanh)
            {
                var a = db.Tours.Where(s => s.MaTour == item.MaTour).FirstOrDefault();
                tour.Add(a);
                
            }


            return View(tour);
        }




        //dia danh

        public ActionResult ListTour_DiaDiem(string id)
        {
            var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
            List<Tour> tour = lay_Tour(id);
            
            Session["tendiadiem"] = iddiadiem.TenDDTQ;
            return View(tour);
        }

        public ActionResult LienHeGuest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LienHeGuest(PhanHoi phanHoi)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var idPH = "PHKH" + rd.Next(1, 1000);
                phanHoi.MaPhanHoi = idPH;

                var kh = db.KhachHangs.Where(k => k.SDT == phanHoi.Sdt).FirstOrDefault();
                if(kh!=null)
                {
                    phanHoi.MaKH = kh.MaKH;
                }    
                phanHoi.NgayPH = DateTime.Now;
                phanHoi.TrangThai = false;
                db.PhanHois.Add(phanHoi);
                TempData["thongbaoLH"] = "taothanhcong";
                db.SaveChanges();
            }
            else
            {
                return View();

            }
            return RedirectToAction("HomePageGuest");
           
        }
        //Anh Hau
        public ActionResult LienHe()
        {
            return View();
        }
        public ActionResult VeChungToi()
        {
            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
    }
    }
