﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Tourist_VietripInsum_2023.common;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class GuestController : Controller
    {
        TouristEntities1 db = new TouristEntities1();
        // GET: Guest

        public ActionResult LoginGuest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginGuest(string username, string password)
        {
            var data = db.KhachHangs.Where(s => s.Username == username && s.UserPassword == password).FirstOrDefault();
            var taikhoan = db.KhachHangs.SingleOrDefault(s => s.Username == username && s.UserPassword == password);
            if (taikhoan == null)
            {
                TempData["error"] = "err";
                return View("LoginGuest");
            }
            else if (taikhoan != null)
            {
                //add session
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["UserKH"] = taikhoan;
                return RedirectToAction("HomePageGuest", "Guest");
            }
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();//remove session
            FormsAuthentication.SignOut();
            return RedirectToAction("HomePageGuest");
        }

        public ActionResult HomePageGuest()
        {
            return View();
        }
        
        private List<DiaDiemThamQuan> Lay_DiaDiem()
        {
            List<DiaDiemThamQuan> diaDiemThamQuans = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> tourChon = db.Tours.Where(n=>n.TrangThai!="Sắp ra mắt").ToList();

            var bangchon = (from diadiem in diaDiemThamQuans
                            join chitiet in chiTietTours on diadiem.MaDDTQ equals chitiet.MaDDTQ
                            join tour in tourChon on chitiet.MaTour equals tour.MaTour
                            group chitiet by chitiet.MaDDTQ into g
                            select new
                            {
                                MaDD = g.FirstOrDefault().MaDDTQ,
                                Matour= g.FirstOrDefault().MaTour

                            }).ToList();

            List<DiaDiemThamQuan> d = new List<DiaDiemThamQuan>();

            foreach (var i in bangchon)
            {
                var item = db.DiaDiemThamQuans.Where(m => m.MaDDTQ == i.MaDD).FirstOrDefault();
               d.Add(item);
                   

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
            List<DiaDiemThamQuan> diaDiemThamQuans = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> tourChon = db.Tours.Where(n => n.TrangThai != "Sắp ra mắt").ToList();

            var bangtinh = (from diadiem in diaDiemThamQuans
                            join chitiet in chiTietTours on diadiem.MaDDTQ equals chitiet.MaDDTQ
                            join tour in tourChon on chitiet.MaTour equals tour.MaTour
                            group diadiem by diadiem.MaTinh into g
                            select new
                            {
                                MaTinh = g.FirstOrDefault().MaTinh,
                            }).ToList();

            List<TinhThanh> tinhtour = new List<TinhThanh>();

            foreach (var i in bangtinh)
            {
                var item = db.TinhThanhs.Where(m => m.MaTinh == i.MaTinh).FirstOrDefault();
                tinhtour.Add(item);
            }

            return PartialView(tinhtour);
        }

        public ActionResult Tourinfomation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();
            Session["Matourchon"] = id;
            if (tour == null)
            {
                return HttpNotFound();
            }
            Session["mat"]=id;
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
                if(a.TrangThai!="Sắp ra mắt")
                {
                    tour.Add(a);
                }    
                
                
            }
            

            return View(tour);
        }

        [HttpPost]
        public ActionResult ListTourTinhThanh(string trangthai, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
        {
            //Xử lý số ngày, số người, ngày khơi hành
            if (songay == "1 - 3 ngày")
            {
                songaybd = 1;
                songaykt = 3;
            }
            else if (songay == "4 - 7 ngày")
            {
                songaybd = 4;
                songaykt = 7;
            }
            else if (songay == "8 - 10 ngày")
            {
                songaybd = 8;
                songaykt = 10;
            }
            else if (songay == "10+ ngày")
            {
                songaybd = 10;
                songaykt = 100;
            }
            //--------------------
            if (songuoi == "1 người")
            {
                songuoibd = 1;
            }
            else if (songay == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songay == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            //DateTime.Now.ToString("yyyy-MM-dd");
            var tours = db.Tours.Where(s => s.TrangThai == trangthai && s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) && s.SoChoNull >= songuoibd);
            return View(tours.ToList());
        }




        //dia danh

        public ActionResult ListTour_DiaDiem(string id)
        {
            var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
            List<Tour> tour = lay_Tour(id);
            
            Session["tendiadiem"] = iddiadiem.TenDDTQ;
            return View(tour);
        }
        [HttpPost]
        public ActionResult ListTour_DiaDiem(string trangthai, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
        {
            //Xử lý số ngày, số người, ngày khơi hành
            if (songay == "1 - 3 ngày")
            {
                songaybd = 1;
                songaykt = 3;
            } else if (songay == "4 - 7 ngày")
            {
                songaybd = 4;
                songaykt = 7;
            }
            else if (songay == "8 - 10 ngày")
            {
                songaybd = 8;
                songaykt = 10;
            }
            else if (songay == "10+ ngày")
            {
                songaybd = 10;
                songaykt = 100;
            }
            //--------------------
            if (songuoi == "1 người")
            {
                songuoibd = 1;
            }
            else if (songay == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songay == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            DateTime.Now.ToString("yyyy-MM-dd");
            var tours = db.Tours.Where(s => s.TrangThai == trangthai 
            && s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) 
             && s.SoChoNull >= songuoibd);
            return View(tours.ToList());
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind(Include = "Email,Sdt,NoiDung")]
        public ActionResult LienHeTour(PhanHoi phanHoi,string Email,string Sdt,string NoiDung)
        {
            
                Random rd = new Random();
                var idPH = "PHKH" + rd.Next(1, 1000);
                phanHoi.MaPhanHoi = idPH;
                phanHoi.Sdt = Sdt;
                phanHoi.Email = Email;
                phanHoi.NoiDung = NoiDung;
                var kh = db.KhachHangs.Where(k => k.SDT == phanHoi.Sdt).FirstOrDefault();
                if (kh != null)
                {
                    phanHoi.MaKH = kh.MaKH;
                }
                phanHoi.NgayPH = DateTime.Now;
                phanHoi.TrangThai = false;
                db.PhanHois.Add(phanHoi);
                TempData["thongbaoLH"] = "taothanhcong";
                db.SaveChanges();
           

            
            var idt = Session["Matourchon"];
            return RedirectToAction("HomePageGuest");

        }

        public ActionResult ListTour()
        {
            List<Tour> tour = db.Tours.Where(s => s.TrangThai == "Tour nổi bật").ToList();

            return View(tour);
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

        //XULYVE-DAT

        public int SoChoTrong(string matour)
        {
            int socho = 0;
            var tour = db.Tours.Where(t => t.MaTour == matour).FirstOrDefault();
            if(tour.SoChoNull == null || tour.SoChoNull == 0)
            {
                socho = 0;
            }
            else
            {
                socho = (int)tour.SoChoNull;
                List<BookTour> bt = db.BookTours.Where(t => t.MaTour == matour).ToList();
                foreach (var item in bt)
                {
                    socho = socho - (int)item.SoCho;
                }
            }
            
            
            return socho;
        }
        //public ActionResult BookTour(string id)
        //{
        //    string matour = (string)Session["Matourchon"];
        //    //ViewBag.chodamua = SoChoTrong(matour);
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookTour(BookTour booktour,string DiaChi,string SDT,string Email,string TenKH)
        {
            string matour = (string)Session["Matourchon"];
            Random rd = new Random();
            var khach = db.KhachHangs.Where(s => s.SDT == SDT).FirstOrDefault();
            if (khach == null)
            {

                KhachHang kh = new KhachHang();
                
                var idKH = "GS" + rd.Next(1, 1000);
                kh.MaKH = idKH;
                booktour.MaKH = idKH;


                kh.SDT = SDT;
                booktour.SdtKH = kh.SDT;
                kh.DiaChi = DiaChi;
                kh.Email = Email;
                kh.HoTenKH = TenKH;
                kh.MaLoaiKH = "TH";
                Session["TaiKhoan"] = kh;
                db.KhachHangs.Add(kh);
                db.SaveChanges();
            }
            else
            {
                Session["TaiKhoan"] = khach;
                booktour.MaKH = khach.MaKH;
                booktour.SdtKH = khach.SDT;
                khach.DiaChi = DiaChi;
                khach.Email = Email;
                khach.HoTenKH = TenKH;
            }
            var idDH = "DH" + rd.Next(1, 1000);
            booktour.MaDH = idDH;
            booktour.MaTour = matour;
            booktour.NgayLap = DateTime.Now;
            booktour.TrangThaiTT = false;
            var khachhang = db.KhachHangs.Where(s => s.SDT == SDT).FirstOrDefault();
            booktour.TotalPrice = 0.0;
            booktour.SoCho = 0;
            Session["madonhang"] = booktour.MaDH;
            
            db.BookTours.Add(booktour);
            db.SaveChanges();
            


            return RedirectToAction("Ticket");


        }

        


        public ActionResult Ticket()
        {
            string matour = (string)Session["Matourchon"];
            var tour = db.Tours.Where(t => t.MaTour == matour).FirstOrDefault();
            ViewBag.chodamua = tour.SoChoNull;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(string soluongdat, bool thanhtoan)
        {
            var maDH = Session["madonhang"].ToString();
            var dh = db.BookTours.Where(t => t.MaDH == maDH).FirstOrDefault();
            var tour = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            var ves = db.Ves.Where(s => s.MaDH == maDH).ToList();
            if(ves.Count() > 0)
            {
                foreach(var item in ves)
                {
                    db.Ves.Remove(item);
                    tour.SoChoNull += 1;
                }
            }
            double tongtien = 0;
            int m = 0;
            //null
            if (soluongdat == "")
            {
                TempData["noti"] = "errornull";
                return RedirectToAction("Ticket", "Guest");
            }
            else
            {
                m = int.Parse(soluongdat);
                if (m == 0)
                {
                    TempData["noti"] = "errornull";
                    return RedirectToAction("Ticket", "Guest");
                }
            }
            if (ModelState.IsValid)
            {
                Random rd = new Random();

                //Tạo vé theo số lượng khách đặt
                for (int i = 1; i <= m; i++)
                {
                    Ve ve = new Ve();
                    var maVe = "V" + maDH + rd.Next(1, 9) + rd.Next(10, 20) + rd.Next(100, 500);
                    ve.MaVe = maVe;
                    ve.MaDH = maDH;
                    ve.Hoten_KH = Request["HoTen_KH" + i];
                    ve.MaLVe = Request["MaLVe" + i];
                    ve.GioiTinh = Request["GioiTinh" + i];
                    var test = Request["NgaySinh" + i];
                    
                    ve.NgaySinh = DateTime.Parse(test);
                    ve.LuuY = Request["LuuY" + i];
                    

                    if (ve.MaLVe == "TICKET01")
                    {
                        tongtien = tongtien + (int)tour.GiaNguoiLon;
                    }
                    else if (ve.MaLVe == "TICKET02")
                    {
                        tongtien = tongtien + (int)tour.GiaTreEm;
                    }
                    db.Ves.Add(ve);
                    //db.SaveChanges();

                }
                //Update tổng tiền cho đơn đặt tour
                //var donhang = db.BookTours.Where(s => s.MaDH == maDH).FirstOrDefault();
                var kh = db.KhachHangs.Where(s => s.MaKH == dh.MaKH).FirstOrDefault();
                dh.TotalPrice = (int)tongtien - (tongtien * kh.LoaiKH.ChietKhau);
                dh.HinhThucThanhToan = thanhtoan;
                dh.SoCho = m;
                tour.SoChoNull -= dh.SoCho;
                db.SaveChanges();
                TongtienDAT(kh.MaKH);
                TempData["noti"] = "success";
                return RedirectToAction("Payment");
            }
            return View();
        }

        //Confirm payment view
        public ActionResult Payment()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string id)
        {
            //Email
            string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/mailconn.html"));
            var dh = db.BookTours.Where(s => s.MaDH == id).FirstOrDefault();
            var kh = db.KhachHangs.Where(s => s.MaKH == dh.MaKH).FirstOrDefault();
            var t = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            content = content.Replace("{{TenKH}}", kh.HoTenKH);
            content = content.Replace("{{Phoneno}}", dh.MaKH);
            content = content.Replace("{{MaDH}}", dh.MaDH);
            content = content.Replace("{{Email}}", kh.Email);
            content = content.Replace("{{Address}}", dh.MaKH);
            content = content.Replace("{{MaTour}}", t.MaTour);
            content = content.Replace("{{TenTour}}", t.TenTour);
            content = content.Replace("{{ngaykhoihanh}}", t.NgayKhoihanh.ToString());
            content = content.Replace("{{noikhoihanh}}", t.NoiKhoiHanh);
            content = content.Replace("{{hanchotve}}", t.HanChotDatVe.ToString());
            content = content.Replace("{{total}}", dh.TotalPrice.ToString());

            ////Gui mail
            var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
            new MailHelp().SendMail(kh.Email, "Xác nhận đặt tour", content);
            TempData["noti"] = "success";
            return RedirectToAction("HomePageGuest", "Guest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyVe(string id)
        {
            BookTour booktour = db.BookTours.Where(s => s.MaDH == id).FirstOrDefault();
            var ves = db.Ves.Where(s => s.MaDH == id).ToList();
            var tour = db.Tours.Where(s => s.MaTour == booktour.MaTour).FirstOrDefault();
            tour.SoChoNull += booktour.SoCho;
            foreach (var item in ves)
            {
                db.Ves.Remove(item);
            }
            db.BookTours.Remove(booktour);
            db.SaveChanges();
            return RedirectToAction("HomePageGuest", "Guest");
        }

        //Hàm tính tổng tiền đặt
        public ActionResult TongtienDAT(string makh)
        {
            var dh = db.BookTours.Where(s => s.MaKH == makh).ToList();
            var kh = db.KhachHangs.Where(s => s.MaKH == makh).FirstOrDefault();
            kh.TongTienDat = 0.0;
            foreach(var item in dh)
            {
                if(item.TrangThaiTT == true)
                {
                    kh.TongTienDat += item.TotalPrice;
                }
            }
            if(kh.TongTienDat >= 15000000)
            {
                kh.MaLoaiKH = "TT";
            }else if(kh.TongTienDat >= 50000000)
            {
                kh.MaLoaiKH = "VIP";
            }
            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
 
        public JsonResult LoadMore(int skip, int take)
        {
            var data = "Tui là Dĩm Khang nè!";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
