using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using test_pi.cls;

namespace test_pi.Controllers
{
    public class HomeController : Controller
    {
        CommonClass c = new CommonClass();
        ConnectionManager.DAL.ConManager con = new ConnectionManager.DAL.ConManager("1");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region
        [HttpPost]
        public ActionResult GetCategory()
        {
            DataSet dsMaster;
            con = new ConnectionManager.DAL.ConManager("1");
            con.OpenDataSetThroughAdapter("select *, null as [details] From category", out dsMaster, false, "1");

            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dsMaster.Tables[0]);
            var jsondata = Json(JSONString, JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = int.MaxValue;
            return jsondata;
        }

        [HttpPost]
        public ActionResult GetBookmark()
        {
            DataSet dsMaster;
            con = new ConnectionManager.DAL.ConManager("1");
            con.OpenDataSetThroughAdapter("select * From bookmark", out dsMaster, false, "1");
            
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dsMaster.Tables[0]);
            var jsondata = Json(JSONString, JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = int.MaxValue;
            return jsondata;
        }

        [HttpPost]
        public JsonResult SaveBookmark(Dictionary<string, object> data)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            try
            {
                DataSet dsMaster, dsCategory;

                #region Validation
                if (data["title"].ToString().Count() > 30)
                {
                    throw new Exception("Title must be in between 30 Characters!!!");
                }
                if (!Rgx.IsMatch(data["url"].ToString()))
                {                    
                    throw new Exception("Insert Valid URL Format!!!");
                }
                #endregion

                #region Save data into Category
                if (data["category_id"] == null && data["category_name"] != null)
                {
                    con.OpenDataSetThroughAdapter(@"INSERT INTO [dbo].[category] ([category_name]) VALUES ('" + data["category_name"].ToString() + "')", out dsCategory, false, "1");
                    con.OpenDataSetThroughAdapter(@"select * from [category] where category_name ='" + data["category_name"].ToString() + "'", out dsCategory, false, "1");
                    data["category_id"] = dsCategory.Tables[0].Rows[0]["id"].ToString();
                }
                #endregion

                #region Save data into Bookmark
                con.OpenDataSetThroughAdapter("select * from bookmark where 1 = 2", out dsMaster, false, "1");

                if (dsMaster.Tables[0].Rows.Count == 0)
                {
                    AddNewRow(dsMaster.Tables[0], data);
                }
                c.SaveDataSets(dsMaster);
                #endregion

                return Json(new { Error = false, Data = data, Message = "Inserted Successfully" });

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
            dt.Rows.Add(dr);
        }
        #endregion

    }
}