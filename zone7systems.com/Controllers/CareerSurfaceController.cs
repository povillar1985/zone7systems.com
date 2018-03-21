using System.IO;
using System.Web.Mvc;
using Umbraco.Core.Models;
using System.Net.Mail;
using System.Configuration;
using zone7systems.com.Models;
using Umbraco.Web.Mvc;

namespace zone7systems.com.Controllers
{
    public class CareerSurfaceController : SurfaceController
    {
        // GET: CareerSurface
        public ActionResult Index()
        {
            return PartialView("ApplyUs", new Career());
        }

        public ActionResult HandleFormPost(Career model)
        {
            var newApplication = Services.ContentService.CreateContentWithIdentity(model.FullName, CurrentPage.Id, "applicationForm");

            model.Position = string.IsNullOrWhiteSpace(model.Position) ? (Request.Form["Position"] ?? string.Empty).ToString() : model.Position;
            newApplication.SetValue("fullName", model.FullName);
            newApplication.SetValue("position", model.Position);
            newApplication.SetValue("email", model.Email);
            newApplication.SetValue("contactNumber", model.ContactNumber);

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    //file.SaveAs(path);

                    //MemoryStream target = new MemoryStream();
                    //file.InputStream.CopyTo(target);
                    //model.ResumeCV = target.ToArray();

                    var ms = Services.MediaService;
                    var mediaFile = ms.CreateMedia(Path.GetFileName(file.FileName), 1217, "File");
                    //mediaFile.SetValue("ResumeFiles", file);
                    //FileStream s = new FileStream(file.FileName, FileMode.Open);
                    mediaFile.SetValue("umbracoFile", Path.GetFileName(file.FileName), file.InputStream);
                    ms.Save(mediaFile);

                    newApplication.SetValue("resumeCV", mediaFile.GetValue("umbracoFile").ToString()); // + " / " + Path.GetFileName(file.FileName)); //model.ResumeCV);
                }

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    using (var client = new SmtpClient())
                    {
                        var mail = new MailMessage();
                        var config = ConfigurationManager.AppSettings;
                        mail.From = new MailAddress(config["mailSender"].ToString());
                        mail.To.Add(config["applicationReceiver"].ToString());
                        mail.Subject = "Application for " + model.Position;
                        mail.Body = "";
                        if (file != null && file.ContentLength > 0)
                        {
                            var attachment = new Attachment(file.InputStream, Path.GetFileName(file.FileName));
                            mail.Attachments.Add(attachment);
                        }
                        client.Send(mail);
                    }
                });
            }

            Services.ContentService.Save(newApplication);

            return CurrentUmbracoPage();
        }
    }
}