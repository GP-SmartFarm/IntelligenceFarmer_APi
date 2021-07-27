using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace IntelligenceFarmer.tableDb
{
    [Table("users")]

    public class Users
    {
        //public Users()
        //{
        //    farms = new HashSet<Farms>();
        //}
        [Key]
        public int User_Id { get; set; }
        public string FullName { get; set; }
        public string Ssn_Id { get; set; }
        public Byte[] Cartimage_ofAgriculture { get; set; }
        public Byte[] Front_Cardimage { get; set; }
        public Byte[] Back_Cardimage { get; set; }
        public string Phone_Number { get; set; }
        public string UserName { get; set; }
        public string Password_Number { get; set; }
        //public ICollection<Farms> farms { get; set; }
        //public IList<Farms> farms { get; set; }
    }
}
