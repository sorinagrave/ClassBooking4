using ClassBooking.Database;
using ClassBooking.Models;
using LBG.Insurance.Developer.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace ClassBooking.Authorisation
{
    public static class ClassBookingUser
    {
        public static bool IsAdmin(this IPrincipal user)
        {
            var gm = FindGymMember(user);
            return gm != null && gm.IsAdmin;
        }

        public static bool IsMember(this IPrincipal user)
        {
            return FindGymMember(user) != null;
        }
        public static string GetMemberName(this IPrincipal user)
        {
            GymMember member = FindGymMember(user);
            if(member == null)
            {
                return user.Identity.Name;
            }
            return member.FirstName + " " + member.LastName;
        }
        private static GymMember FindGymMember(IPrincipal user)
        {
            
            GymContext db = new GymContext();
            Tex.Log("Verifying Member",user.Identity.Name);
            var nameParts = user.Identity.Name.Split('\\');

            if (nameParts.Length < 1) {
                Tex.Log("incorrect number of name parts returned");
                return null;
            }
            string sName = nameParts[1];
            Tex.Log("checking for ", sName);
            
            return db.GymMembers.SingleOrDefault(m => m.StaffId == sName);
        } 
    }
}