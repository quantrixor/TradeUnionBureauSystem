//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradeUnionBureauSystem.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Members
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Members()
        {
            this.Events = new HashSet<Events>();
        }
    
        public int MemberID { get; set; }
        public Nullable<int> NumberProfcard { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string AcademicGroup { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<int> PositionID { get; set; }
        public Nullable<int> CommissionID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string VKLink { get; set; }
        public byte[] Photo { get; set; }
    
        public virtual Commissions Commissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Events> Events { get; set; }
        public virtual Positions Positions { get; set; }
        public virtual Users Users { get; set; }
    }
}
