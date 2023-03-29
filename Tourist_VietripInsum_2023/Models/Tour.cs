//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tourist_VietripInsum_2023.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tour
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tour()
        {
            this.BookTours = new HashSet<BookTour>();
            this.ChiTietTours = new HashSet<ChiTietTour>();
        }
    
        public string MaTour { get; set; }
        public string MaLTour { get; set; }
        public string MaKS { get; set; }
        public string TenTour { get; set; }
        public string GioiThieu { get; set; }
        public string HinhMinhHoa_T { get; set; }
        public Nullable<System.DateTime> NgayKhoihanh { get; set; }
        public Nullable<System.DateTime> NgayTroVe { get; set; }
        public Nullable<int> SoNgay { get; set; }
        public string NoiKhoiHanh { get; set; }
        public Nullable<int> SoChoNull { get; set; }
        public Nullable<int> GiaTreEm { get; set; }
        public Nullable<int> GiaNguoiLon { get; set; }
        public string TrangThai { get; set; }
        public Nullable<System.DateTime> HanChotDatVe { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookTour> BookTours { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietTour> ChiTietTours { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual LoaiTour LoaiTour { get; set; }
    }
}
