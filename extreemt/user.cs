//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace extreemt
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public user()
        {
            this.cheques = new HashSet<cheque>();
            this.products = new HashSet<product>();
            this.Transfers = new HashSet<Transfer>();
            this.Transfers1 = new HashSet<Transfer>();
            this.userPayProducts = new HashSet<userPayProduct>();
        }
    
        public int id { get; set; }
        public string mail { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string mobile { get; set; }
        public string homephone { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string ssn { get; set; }
        public string nationality { get; set; }
        public string relationship { get; set; }
        public string loginPassword { get; set; }
        public string walletPassword { get; set; }
        public string status { get; set; }
        public int rightActiveCount { get; set; }
        public int leftActiveCount { get; set; }
        public int rightInactiveCount { get; set; }
        public int leftInactiveCount { get; set; }
        public string position { get; set; }
        public int genNumber { get; set; }
        public Nullable<int> parentGenNum { get; set; }
        public Nullable<int> parentUserId { get; set; }
        public int userId { get; set; }
        public Nullable<int> registererId { get; set; }
        public int cashBank { get; set; }
        public int productBank { get; set; }
        public Nullable<System.DateTime> registerDate { get; set; }
        public int dailyLeftActiveCount { get; set; }
        public int dailyRightActiveCount { get; set; }
        public bool isActive { get; set; }
    
        public virtual ICollection<cheque> cheques { get; set; }
        public virtual ICollection<product> products { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
        public virtual ICollection<Transfer> Transfers1 { get; set; }
        public virtual ICollection<userPayProduct> userPayProducts { get; set; }
    }
}
