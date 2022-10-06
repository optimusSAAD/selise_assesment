using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace test_pi.cls
{
    public class CommonClass
    {
        public void SaveDataSets(params DataSet[] dsRef)
        {
            //throw new Exception("test");
            bool IsTransactionStarted = false;
            ConnectionManager.DAL.ConManager objCon = null;
            try
            {
                objCon = new ConnectionManager.DAL.ConManager("1");
                objCon.OpenConnection("1");
                objCon.BeginTransaction();
                IsTransactionStarted = true;
                int i = 0;
                foreach (DataSet value in dsRef)
                {
                    if (dsRef[i] != null)
                        if (dsRef[i].Tables.Count > 0)
                            objCon.SaveDataSetThroughAdapter(ref dsRef[i], true, "1");
                    i++;
                }
                objCon.CommitTransaction();
                IsTransactionStarted = false;
            }
            catch (Exception ex)
            {
                try
                {
                    if (IsTransactionStarted)
                    {
                        objCon.RollBack();
                    }
                    objCon.CloseConnection();
                }
                catch (Exception exp)
                {
                    throw ex;
                }
            }
            finally
            {
                objCon = null;
            }
        }//End Function
        public void GenID(string strFieldName, out string strID)
        {
            ConnectionManager.DAL.ConManager objCoManager;
            string strSql, PK = "";
            DataSet dsLocal = null;
            int ID = 1;
            int Temp = 0;
            int FinalId, prime;

            try
            {
                strSql = "SELECT top 1 UserId FROM tblUser order by AddedDate desc";
                objCoManager = new ConnectionManager.DAL.ConManager("1");
                objCoManager.OpenDataSetThroughAdapter(strSql, out dsLocal, false, false, "", "1");
                if (dsLocal.Tables[0].Rows.Count > 0)
                {
                    Temp = Convert.ToInt32(dsLocal.Tables[0].Rows[0]["UserId"].ToString()) + ID;
                }
                else
                {
                    Temp = 0000001;
                }
                for (int i = 0; i < 9999999; i++)
                {
                    if (strFieldName.ToUpper() == "MALE")
                    {
                        if (Temp % 2 == 0)
                        {
                            FinalId = Temp;
                            finalPrimaryKey(FinalId.ToString().Length, FinalId, out PK);
                            break;
                        }
                        else
                        {
                            Temp++;
                        }
                    }
                    else if (strFieldName.ToUpper() == "FEMALE")
                    {
                        if (Temp % 2 != 0)
                        {
                            FinalId = Temp;
                            finalPrimaryKey(FinalId.ToString().Length, FinalId, out PK);
                            break;
                        }
                        else
                        {
                            Temp++;
                        }
                    }
                    else
                    {
                        for (int j = 2; j <= Temp - 1; j++)
                        {
                            if (Temp % j == 0)
                            {
                                Temp++;
                            }
                            else
                            {
                                FinalId = Temp;
                                finalPrimaryKey(FinalId.ToString().Length, FinalId, out PK);
                                break;
                            }
                        }
                    }
                    if (PK != "")
                    {
                        break;
                    }
                }
                strID = PK;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void finalPrimaryKey(int length, int Key, out string PrimaryKey)
        {
            if (length == 1)
            {
                PrimaryKey = "000000" + Key.ToString();
            }
            else if (length == 2)
            {
                PrimaryKey = "00000" + Key.ToString();
            }
            else if (length == 3)
            {
                PrimaryKey = "0000" + Key.ToString();
            }
            else if (length == 4)
            {
                PrimaryKey = "000" + Key.ToString();
            }
            else if (length == 5)
            {
                PrimaryKey = "00" + Key.ToString();
            }
            else if (length == 2)
            {
                PrimaryKey = "0" + Key.ToString();
            }
            else
            {
                PrimaryKey = Key.ToString();
            }
        }
    }
}