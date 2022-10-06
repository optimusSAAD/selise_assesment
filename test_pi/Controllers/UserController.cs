using Library.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using test_pi.cls;
using Newtonsoft.Json;
namespace test_pi.Controllers
{
    public class UserController : Controller
    {
        CommonClass c = new CommonClass();
        ConnectionManager.DAL.ConManager con = new ConnectionManager.DAL.ConManager("1");
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LoginUser()
        {
            return View();
        }

        #region Get Data

        [HttpPost]
        public ActionResult GetCountry()
        {
            DataSet dsMaster;
            con = new ConnectionManager.DAL.ConManager("1");
            con.OpenDataSetThroughAdapter("select * From Country", out dsMaster, false, "1");
            //var Country = new List<County>();
            var Country = dsMaster.Tables[0].AsEnumerable();

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dsMaster.Tables[0]);
            var jsondata = Json(JSONString, JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = int.MaxValue;
            return jsondata;
        }
        [HttpPost]
        public ActionResult City(string Id)
        {
            DataSet dsMaster;
            con = new ConnectionManager.DAL.ConManager("1");
            con.OpenDataSetThroughAdapter("select * From City where CountryId=" + Id + "", out dsMaster, false, "1");
            var Country = dsMaster.Tables[0].AsEnumerable();

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dsMaster.Tables[0]);
            var jsondata = Json(JSONString, JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = int.MaxValue;
            return jsondata;
        }

        #endregion

        #region User Login
        [HttpPost]
        public JsonResult LoginUser(Dictionary<string, object> User)
        {
            string Status = string.Empty;
            con.OpenDataSetThroughAdapter("select * from tblUser where Email='" + User["Email"] + "'", out DataSet dsMaster, false, "1");
            if (dsMaster.Tables[0].Rows.Count > 0)
            {
                string password = Decrypt(dsMaster.Tables[0].Rows[0]["Password"].ToString());
                if (password == User["Password"].ToString())
                {
                    Status = "Successful";
                    return Json(new { Error = false, Data = dsMaster.Tables[0], Message = Status });
                }
                else
                {
                    Status = "Wrong Password";
                    return Json(new { Message = Status });
                }
            }
            else
            {
                Status = "No user found";
                return Json(new { Message = Status });
            }

        }

        #endregion

        #region Register User
        [HttpPost]
        public JsonResult createUser(Dictionary<string, object> data)
        {
            try
            {
                DataSet dsMaster;

                #region Validation
                con.OpenDataSetThroughAdapter("select * from tblUser where Email='" + data["Email"] + "' AND  UserId<>'" + data["UserId"] + "'", out dsMaster, false, "1");
                if (dsMaster.Tables[0].Rows.Count > 0)
                    throw new Exception("Same User Email already exists!!!");

                con.OpenDataSetThroughAdapter("select * from tblUser where Phone='" + data["Phone"] + "' AND  UserId<>'" + data["UserId"] + "'", out dsMaster, false, "1");
                if (dsMaster.Tables[0].Rows.Count > 0)
                    throw new Exception("Same User Phone Number already exists!!!");
                if (data["Phone"].ToString().Length>11)
                {
                    throw new Exception("Phone number is more than 11 digite");
                }
                if (string.IsNullOrEmpty(data["Password"].ToString()))
                {
                    throw new Exception("Insert Password");
                }
                if (string.IsNullOrEmpty(data["Gender"].ToString()))
                {
                    throw new Exception("Select Gender");
                }
                con.OpenDataSetThroughAdapter("select * from tblUser where UserId='" + data["UserId"] + "'", out dsMaster, false, "1");
                #endregion
                
                string _Id = "";

                #region data update
                if (dsMaster.Tables[0].Rows.Count == 0)
                {
                    c.GenID(data["Gender"].ToString(), out _Id);
                    data["UserId"] = _Id;
                    data["Password"] = Encrypt(data["Password"].ToString());
                    AddNewRow(dsMaster.Tables[0], data);
                }
                else
                {
                    _Id = data["Id"].ToString();
                    EditRow(dsMaster.Tables[0].Rows[0], data);
                }
                #endregion data update


                c.SaveDataSets(dsMaster);

                return Json(new { Error = false, Data = data, Message = "User Created Successfully" });

            }
            catch (Exception ex)
            {

                return Json(new { Error = true, Message = ex.Message });

            }
        }
        [HttpPost]
        public JsonResult ImageFile(Dictionary<string, object> data, HttpPostedFileBase file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            try
            {
                var files = Request.Files["file"];
                if (files != null)
                {
                    var path = "/User/USerPic/";
                    var File_Paths = "/User/USerPic/" + fileName + extension;
                    if (System.IO.File.Exists(path))
                    {
                        file.SaveAs(File_Paths);
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        file.SaveAs(File_Paths);
                    }

                }


                return Json(new { Error = false, Message = "User Created Successfully" });

            }
            catch (Exception ex)
            {

                return Json(new { Error = true, Message = ex.Message });

            }
        }

        private void AddNewRow(DataTable dt, Dictionary<string, object> sourceData)
        {
            DataRow dr = dt.NewRow();
            foreach (var item in sourceData.Keys)
            {
                try
                {
                    dr[item] = sourceData[item];
                }
                catch (Exception)
                {
                }
            }
            dr["AddedDate"] = System.DateTime.Now.ToString();
            dt.Rows.Add(dr);
        }
        private void EditRow(DataRow dr, Dictionary<string, object> sourceData)
        {

            dr.BeginEdit();
            foreach (var item in sourceData.Keys)
            {
                try
                {
                    dr[item] = sourceData[item];
                }
                catch (Exception)
                {
                }
            }
            dr["UpdatedDate"] = System.DateTime.Now.ToString();
            dr.EndEdit();
        }
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;

        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion


    }

}

public class County
{
    public int Id { get; set; }
    public string Name { get; set; }
}