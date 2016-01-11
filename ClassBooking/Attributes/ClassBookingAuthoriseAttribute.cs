using ClassBooking.Authorisation;
using System;
using System.Web.Mvc;
using System.Web;

namespace ClassBooking.Attributes
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple=false,Inherited=true)]
    public class ClassBookingAuthoriseAttribute : AuthorizeAttribute
    {
        public AuthorisationLevel Level { get; set; }
        public ClassBookingAuthoriseAttribute()
        {
            Level = AuthorisationLevel.Unknown;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Level == AuthorisationLevel.Administrator && httpContext.User.IsAdmin() ||
                Level == AuthorisationLevel.Member && httpContext.User.IsMember();
        }
    }
}