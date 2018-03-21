using zone7systems.com.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using umbraco;
using Umbraco.Web.Mvc;

namespace zone7systems.com.Controllers
{
    public class ContactSurfaceController : SurfaceController
    {
        // GET: ContactSurface
        public ActionResult Index()
        {
            return PartialView("./Forms/_ContactForm", new Contact());
        }

        [HttpPost]
        public ActionResult HandleFormPost(Contact model) {
            try
            {
                if (ModelState.IsValid)
                {
                    var newContact = Services.ContentService.CreateContentWithIdentity(model.FullName, CurrentPage.Id, "contactForm");
                    newContact.SetValue("fullName", model.FullName);
                    newContact.SetValue("email", model.Email);
                    newContact.SetValue("phone", model.Phone);
                    newContact.SetValue("message", model.Message);
                    Services.ContentService.SaveAndPublishWithStatus(newContact);

                    var config = ConfigurationManager.AppSettings;

                    Task.Factory.StartNew(() =>
                    {
                        library.SendMail(fromMail: config.Get("mailSender").ToString(), toMail: config.Get("inquiryReceiver").ToString(), subject: model.FullName, body: model.Message, isHtml: true);
                    });
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception)
            {
                return RedirectToCurrentUmbracoPage();
            }
        }
    }
}