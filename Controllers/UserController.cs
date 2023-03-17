using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using UserCapturer.Models;

namespace UserCapturer.Controllers
{
    
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            List<User> users = new List<User>();
            var path = Server.MapPath("User\\Users.xml");
            if (System.IO.File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                foreach (XmlNode node in doc.SelectNodes("/Users/User"))
                {
                    var user = ConvertXMLToUserModel(node);
                    users.Add(user);
                }
            }
            return View(users);
        }

        public ActionResult Details(int id)
        {
            var userToView = GetUser(id);
            return View(userToView);
        }

        public ActionResult Delete(int id)
        {
            var userToDelete = GetUser(id);
            return View(userToDelete);
        }

        [HttpPost]
        public ActionResult Delete(User user)
        {
            var path = Server.MapPath("/User/Users.xml");
            XDocument doc = XDocument.Load(path);

            XElement element = (from x in doc.Descendants("User")
                                where x.Attribute("ID").Value == user.Id.ToString()
                                select x).First();
            element.Remove();
            doc.Save(path);

            return RedirectToAction("Index");
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid().GetHashCode();
                WriteXML(user);
                ModelState.Clear();
            }
            user = new User();
            return View(user);
        }
       
        public ActionResult Edit(int id)
        {
            var userToEdit = GetUser(id);
            return View(userToEdit);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            XmlDocument doc = new XmlDocument();
            var path = Server.MapPath("/User/Users.xml");
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("/Users/User");

            foreach (XmlNode node in nodes)
            {
                XmlAttribute idAttribute = node.Attributes["ID"];
                if (idAttribute != null && int.Parse(idAttribute.Value) == user.Id)
                {
                    node.Attributes["FirstName"].InnerText = user.FirstName;
                    node.Attributes["LastName"].InnerText = user.LastName;
                    node.Attributes["Cellphone"].InnerText = user.Cellphone;
                    node.Attributes["Email"].InnerText = user.Email;
                    node.Attributes["Gender"].InnerText = user.Sex.ToString();
                }
            }

            // save the XmlDocument back to disk
            doc.Save(path);

            return View(user);
        }

        private User ConvertXMLToUserModel(XmlNode node)
        {
            Enum.TryParse<Gender>(node.Attributes["Gender"].InnerText, out Gender gender);
            var newUser = new User
            {
                Id = int.Parse(node.Attributes["ID"].InnerText),
                FirstName = node.Attributes["FirstName"].InnerText,
                LastName = node.Attributes["LastName"].InnerText,
                Cellphone = node.Attributes["Cellphone"].InnerText,
                Email = node.Attributes["Email"].InnerText,
                Sex = gender,
            };

            return newUser;
        }

        private void WriteXML(User user)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(User));

            var path = Server.MapPath("Users.xml");

            if (!System.IO.File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Users");

                    xmlWriter.WriteStartElement("User");
                    xmlWriter.WriteAttributeString("ID", user.Id.ToString());
                    xmlWriter.WriteAttributeString("FirstName", user.FirstName);
                    xmlWriter.WriteAttributeString("LastName", user.LastName);
                    xmlWriter.WriteAttributeString("Cellphone", user.Cellphone);
                    xmlWriter.WriteAttributeString("Email", user.Email);
                    xmlWriter.WriteAttributeString("Gender", user.Sex.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            else
            {
                XDocument xDocument = XDocument.Load(path);
                XElement root = xDocument.Element("Users");
                IEnumerable<XElement> rows = root.Descendants("User");
                XElement firstRow = rows.First();
                firstRow.AddBeforeSelf(
                   new XElement("User",
                   new XAttribute("ID", user.Id),
                   new XAttribute("FirstName", user.FirstName),
                   new XAttribute("LastName", user.LastName),
                    new XAttribute("Cellphone", user.Cellphone),
                   new XAttribute("Email", user.Email),                
                   new XAttribute("Gender", user.Sex))
                   );
                xDocument.Save(path);
            }
        }

        private User GetUser(int id)
        {
            var user = new User();
            XmlDocument doc = new XmlDocument();
            var path = Server.MapPath("/User/Users.xml");
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("/Users/User");

            foreach (XmlNode node in nodes)
            {
                XmlAttribute idAttribute = node.Attributes["ID"];
                if (idAttribute != null && int.Parse(idAttribute.Value) == id)
                {
                    user = ConvertXMLToUserModel(node);
                }
            }

            // save the XmlDocument back to disk
            doc.Save(path);

            return user;
        }

    }
}