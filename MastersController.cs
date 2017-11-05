using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OracleRuleBook_Library;
using OracleRuleBook_Library.Models;
using OracleRuleBook_Library.Models.Masters;
using OracleRuleBook_Library.Serializers;
using System.IO;
using System.Xml;
using OracleRuleBook_Library.Serializers.Masters;
using System.Web;
using System.Collections;
using OracleRuleBook_WebAPI.Models;

namespace OracleRuleBook_WebAPI.Controllers.Masters
{
    public class MastersController : ApiController
    {

        #region fields
        public static string QueryString = "";
        Utilities objUtilities = new Utilities();
        XMLHelper objXMLHelper = new XMLHelper();
        #endregion

        public bool checkTokenID(string TokenID, string userid)
        {
            InterCompany ObjInterCompany = new InterCompany();
            return ObjInterCompany.checkUser(TokenID, userid);
        }
        //Intercompany Master 
        //api/Masters/SaveInterCompanyData
        [HttpPost]
        public HttpResponseMessage SaveInterCompanyData(InterCompany ObjInterCompany)
        {
            try
            {
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }
                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }

                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    if (checkTokenID(TokenID, userid))
                    {
                        ResponseMessage objResponseMsg = new ResponseMessage();
                        if (ObjInterCompany.INTERCOMPANY_CODE.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "InterCompany CODE is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjInterCompany.INTERCOMPANY_DESC.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "InterCompany Description is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjInterCompany.INTERCOMPANY_NAME.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "InterCompany Name is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjInterCompany.ISACTIVE == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "IsActive Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        try
                        {
                            Request.Properties.Add("RequiredJSONConvertion", 0);
                            ResponseMessage objResponseMessage = new ResponseMessage();
                            if (ObjInterCompany != null)
                            {

                                InterCompany ObjInterCompanyData = new InterCompany();

                                ObjInterCompanyData.INTERCOMPANY_ID = ObjInterCompany.INTERCOMPANY_ID;
                                ObjInterCompanyData.INTERCOMPANY_CODE = ObjInterCompany.INTERCOMPANY_CODE;
                                ObjInterCompanyData.INTERCOMPANY_NAME = ObjInterCompany.INTERCOMPANY_NAME;
                                ObjInterCompanyData.INTERCOMPANY_DESC = ObjInterCompany.INTERCOMPANY_DESC;
                                ObjInterCompanyData.ISACTIVE = ObjInterCompany.ISACTIVE;


                                string xmlString = string.Empty;
                                string isValid = string.Empty;

                                string strXmlText = objXMLHelper.CreateXML(ObjInterCompanyData);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(strXmlText);
                                StringWriter sw = new StringWriter();
                                XmlTextWriter tx = new XmlTextWriter(sw);
                                xmlDoc.WriteTo(tx);
                                objResponseMessage = ObjInterCompanyData.SaveData(xmlDoc, userid);//, objlogin);
                                //objResponseMessage.code = 100;
                                //objResponseMessage.Message = "Visitor Details Saved Successfully";
                                xmlString = objXMLHelper.CreateXML(objResponseMessage);
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                            else
                            {
                                objResponseMessage.code = 108;
                                objResponseMessage.Message = CommonConstants.SaveErrorMsg;
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;//  return Request.CreateResponse(HttpStatusCode.Created, objUtilities.HandelAPIResponseException(ex));
                        }
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }
        }

        //api/Masters/GetToMeetUsers
        [HttpGet]
        public HttpResponseMessage GetInterCompanyData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetInterCompanyData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                string TokenID = string.Empty;
                string a = Helper.GetQueryString(this.Request, "dataType");
                userid = Helper.GetHeader(this.Request, "USER_ID");
                TokenID = Helper.GetHeader(this.Request, "TOKEN_ID");

               
                
                
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string SearchText = string.Empty;
                        //string OutputType = string.Empty;
                        if (headers.Contains("SearchText"))
                        {
                            SearchText = headers.GetValues("SearchText").First();
                        }
                        //if (headers.Contains("OutputType"))
                        //{
                        //    OutputType = headers.GetValues("OutputType").First();
                        //}
                        //if (OutputType == "" || OutputType == null)
                        //{
                        //    ResponseMessage objResponseMessage = new ResponseMessage();
                        //    objResponseMessage.code = 108;
                        //    objResponseMessage.Message = "Output Parameter Is Missing!!";
                        //    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                        //}
                        //else
                        //{
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        InterCompany ObjInterCompany = new InterCompany();
                        // string[,] param = new string[2, 2] { { "SearchText", SearchText }, { "OutputType", OutputType } };
                        string[,] param = new string[1, 2] { { "SearchText", SearchText } };
                        QueryString = CommonConstants.createXML(param, 1);
                        SerializeInterCompany ObjserializeInterCompany = ObjInterCompany.GetInterCompanyData(QueryString, userid);//, objlogin);
                        ObjserializeInterCompany.code = 100;
                        ObjserializeInterCompany.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeInterCompany);
                        return objHttpResponseMessage;
                        //}
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //Account Code Master
        //api/Masters/SaveData
        [HttpPost]
        public HttpResponseMessage SaveMISHeadAccountMapping(MISHeadAccountMapping ObjMISHeadAccountMapping)
        {
            try
            {
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }
                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }

                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    if (checkTokenID(TokenID, userid))
                    {
                        ResponseMessage objResponseMsg = new ResponseMessage();
                        if (ObjMISHeadAccountMapping.MIS_HEAD_ID == 0)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "MIS Head ID is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjMISHeadAccountMapping.ACCOUNT_CODE.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Account Code is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjMISHeadAccountMapping.ACCOUNT_NAME.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Account Name is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }

                        try
                        {
                            Request.Properties.Add("RequiredJSONConvertion", 0);
                            ResponseMessage objResponseMessage = new ResponseMessage();
                            if (ObjMISHeadAccountMapping != null)
                            {

                                MISHeadAccountMapping ObjMISHeadAccountMappingData = new MISHeadAccountMapping();

                                ObjMISHeadAccountMappingData.MISHEAD_ACCODE_MAPPING_ID = ObjMISHeadAccountMapping.MISHEAD_ACCODE_MAPPING_ID;
                                ObjMISHeadAccountMappingData.MIS_HEAD_ID = ObjMISHeadAccountMapping.MIS_HEAD_ID;
                                ObjMISHeadAccountMappingData.ACCOUNT_CODE = ObjMISHeadAccountMapping.ACCOUNT_CODE;
                                ObjMISHeadAccountMappingData.ACCOUNT_NAME = ObjMISHeadAccountMapping.ACCOUNT_NAME;

                                string xmlString = string.Empty;
                                string isValid = string.Empty;

                                string strXmlText = objXMLHelper.CreateXML(ObjMISHeadAccountMappingData);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(strXmlText);
                                StringWriter sw = new StringWriter();
                                XmlTextWriter tx = new XmlTextWriter(sw);
                                xmlDoc.WriteTo(tx);
                                objResponseMessage = ObjMISHeadAccountMappingData.SaveData(xmlDoc, userid);//, objlogin);
                                //objResponseMessage.code = 100;
                                //objResponseMessage.Message = "Visitor Details Saved Successfully";
                                xmlString = objXMLHelper.CreateXML(objResponseMessage);
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                            else
                            {
                                objResponseMessage.code = 108;
                                objResponseMessage.Message = CommonConstants.SaveErrorMsg;
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;//  return Request.CreateResponse(HttpStatusCode.Created, objUtilities.HandelAPIResponseException(ex));
                        }
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }
        }

        //api/Masters/GetMISHeadData
        [HttpGet]
        public HttpResponseMessage GetMISHeadData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetMISHeadData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string Flag = string.Empty;
                        if (headers.Contains("Flag"))
                        {
                            Flag = headers.GetValues("Flag").First();
                        }
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        MISHeadAccountMapping ObjMISHeadAccountMapping = new MISHeadAccountMapping();
                        string[,] param = new string[1, 2] { { "Flag", Flag } };
                        QueryString = CommonConstants.createXML(param, 1);
                        SerializeMISHeadAccountMapping ObjserializeMISHeadAccountMapping = ObjMISHeadAccountMapping.GetMISHeadData(QueryString, userid);//, objlogin);
                        ObjserializeMISHeadAccountMapping.code = 100;
                        ObjserializeMISHeadAccountMapping.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeMISHeadAccountMapping);
                        return objHttpResponseMessage;
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //api/Masters/GetMISAccountList
        [HttpGet]
        public HttpResponseMessage GetMISAccountList()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetMISAccountList API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string Flag = string.Empty;
                        if (headers.Contains("Flag"))
                        {
                            Flag = headers.GetValues("Flag").First();
                        }

                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        MISHeadAccountMapping ObjMISHeadAccountMapping = new MISHeadAccountMapping();
                        string[,] param = new string[1, 2] { { "Flag", Flag } };
                        QueryString = CommonConstants.createXML(param, 1);
                        SerializeMISHeadAccountMapping ObjserializeMISHeadAccountMapping = ObjMISHeadAccountMapping.GetMISAccountList(QueryString, userid);//, objlogin);
                        ObjserializeMISHeadAccountMapping.code = 100;
                        ObjserializeMISHeadAccountMapping.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeMISHeadAccountMapping);
                        return objHttpResponseMessage;
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //api/Masters/SearchMISHEADData
        [HttpGet]
        public HttpResponseMessage SearchMISHEADData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetMISHEADData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string SearchText = string.Empty;
                        string OutputType = string.Empty;
                        if (headers.Contains("SearchText"))
                        {
                            SearchText = headers.GetValues("SearchText").First();
                        }

                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        MISHeadAccountMapping ObjMISHeadAccountMapping = new MISHeadAccountMapping();
                        string[,] param = new string[1, 2] { { "SearchText", SearchText } };
                        QueryString = CommonConstants.createXML(param, 1);
                        SerializeMISHeadAccountMapping ObjserializeMISHeadAccountMapping = ObjMISHeadAccountMapping.SearchMISHEADData(QueryString, userid);//, objlogin);
                        ObjserializeMISHeadAccountMapping.code = 100;
                        ObjserializeMISHeadAccountMapping.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeMISHeadAccountMapping);
                        return objHttpResponseMessage;

                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //Rule Master
        //api/Masters/SaveRuleMasterData
        [HttpPost]
        public HttpResponseMessage SaveRuleMasterData(RuleMaster ObjRuleMaster)
        {
            try
            {
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }
                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }

                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    if (checkTokenID(TokenID, userid))
                    {
                        ResponseMessage objResponseMsg = new ResponseMessage();
                        if (ObjRuleMaster.RuleName.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Rule Name is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjRuleMaster.RuleDescription.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Rule Description is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjRuleMaster.RuleCode.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Rule Code is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjRuleMaster.DefaultStatus == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Default Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjRuleMaster.IsActive == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "IsActive Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        try
                        {
                            Request.Properties.Add("RequiredJSONConvertion", 0);
                            ResponseMessage objResponseMessage = new ResponseMessage();
                            if (ObjRuleMaster != null)
                            {

                                RuleMaster ObjRuleMasterData = new RuleMaster();
                                ObjRuleMasterData.RuleID = ObjRuleMaster.RuleID;
                                ObjRuleMasterData.RuleName = ObjRuleMaster.RuleName;
                                ObjRuleMasterData.RuleDescription = ObjRuleMaster.RuleDescription;
                                ObjRuleMasterData.RuleCode = ObjRuleMaster.RuleCode;
                                ObjRuleMasterData.RuleOrder = ObjRuleMaster.RuleOrder;
                                ObjRuleMasterData.DefaultStatus = ObjRuleMaster.DefaultStatus;
                                ObjRuleMasterData.IsActive = ObjRuleMaster.IsActive;


                                string xmlString = string.Empty;
                                string isValid = string.Empty;

                                string strXmlText = objXMLHelper.CreateXML(ObjRuleMasterData);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(strXmlText);
                                StringWriter sw = new StringWriter();
                                XmlTextWriter tx = new XmlTextWriter(sw);
                                xmlDoc.WriteTo(tx);
                                objResponseMessage = ObjRuleMasterData.SaveData(xmlDoc, userid);//, objlogin);
                                //objResponseMessage.code = 100;
                                //objResponseMessage.Message = "Visitor Details Saved Successfully";
                                xmlString = objXMLHelper.CreateXML(objResponseMessage);
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                            else
                            {
                                objResponseMessage.code = 108;
                                objResponseMessage.Message = CommonConstants.SaveErrorMsg;
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;//  return Request.CreateResponse(HttpStatusCode.Created, objUtilities.HandelAPIResponseException(ex));
                        }
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }
        }

        //Rule Creation Page
        //api/Masters/RuleCreation
        [HttpGet]
        public HttpResponseMessage RuleCreation()
        {
            try
            {
                CommonConstants.WriteLog("Calling RuleCreation API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    //if (checkTokenID(TokenID, userid))
                    if(1==1)
                    {
                        string Type = string.Empty;
                        string TableName = string.Empty;
                        if (headers.Contains("Type"))
                        {
                            Type = headers.GetValues("Type").First();
                        }

                        if (headers.Contains("TableName"))
                        {
                            TableName = headers.GetValues("TableName").First();
                        }
                        if (Type.ToString() != "")
                        {
                            CommonConstants.WriteLog("Token ID verification complete");
                            Request.Properties.Add("RequiredJSONConvertion", 0);
                            HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                            RuleMaster ObjRuleMaster = new RuleMaster();
                            string[,] param = new string[2, 2] { { "Type", Type }, { "TableName", TableName } };
                            QueryString = CommonConstants.createXML(param, 2);
                            SerializeRuleMaster ObjserializeRuleMaster = ObjRuleMaster.RuleCreation(QueryString, userid);//, objlogin);
                            ObjserializeRuleMaster.code = 100;
                            ObjserializeRuleMaster.Message = "Success";
                            objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeRuleMaster);
                            return objHttpResponseMessage;
                        }
                        else
                        {
                            ResponseMessage objResponseMessage = new ResponseMessage();
                            objResponseMessage.code = 108;
                            objResponseMessage.Message = "Type Parameter is Missing!!";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                        }


                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //api/Masters/GetToMeetUsers
        [HttpGet]
        public HttpResponseMessage GetRuleMasterData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetRuleCreationData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string SearchText = string.Empty;
                        //string OutputType = string.Empty;
                        if (headers.Contains("SearchText"))
                        {
                            SearchText = headers.GetValues("SearchText").First();
                        }
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        RuleMaster ObjRuleMaster = new RuleMaster();
                        // string[,] param = new string[2, 2] { { "SearchText", SearchText }, { "OutputType", OutputType } };
                        string[,] param = new string[1, 2] { { "SearchText", SearchText } };
                        QueryString = CommonConstants.createXML(param, 1);
                        SerializeRuleMaster ObjserializeRuleMaster = ObjRuleMaster.GetRuleMasterData(QueryString, userid);//, objlogin);
                        ObjserializeRuleMaster.code = 100;
                        ObjserializeRuleMaster.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeRuleMaster);
                        return objHttpResponseMessage;
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

        //MIS Operand 
        //api/Masters/GetMISOperandData
        [HttpGet]
        public HttpResponseMessage GetMISOperandData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetMISOperandData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string SearchText = string.Empty;
                        //if (headers.Contains("SearchText"))
                        //{
                        //    SearchText = headers.GetValues("SearchText").First();
                        //}
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        MISOperand ObjMISOperand = new MISOperand();
                        SerializeMISOperand ObjserializeMISOperand = ObjMISOperand.GetMISOperandData(userid);//, objlogin);
                        ObjserializeMISOperand.code = 100;
                        ObjserializeMISOperand.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeMISOperand);
                        return objHttpResponseMessage;
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }


        //Batch Master 
        //api/Masters/SaveBatchMasterData
        [HttpPost]
        public HttpResponseMessage SaveBatchMasterData(BatchMaster ObjBatchMaster)
        {
            try
            {
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }
                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }

                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    if (checkTokenID(TokenID, userid))
                    {
                        ResponseMessage objResponseMsg = new ResponseMessage();
                        if (ObjBatchMaster.Month.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Month is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjBatchMaster.Year.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Year is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjBatchMaster.IsPrevious == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "IsPrevious is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjBatchMaster.IsDelete == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "IsDelete Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }

                        if (ObjBatchMaster.Status.Trim() == "")
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjBatchMaster.PrePorcessTransCount == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "PrePorcessTransCount is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }
                        if (ObjBatchMaster.PostPorcessTransCount == null)
                        {
                            objResponseMsg.code = 108;
                            objResponseMsg.Message = "PostPorcessTransCount Status is Missing";
                            return Request.CreateResponse(HttpStatusCode.Created, objResponseMsg);
                        }

                        try
                        {
                            Request.Properties.Add("RequiredJSONConvertion", 0);
                            ResponseMessage objResponseMessage = new ResponseMessage();
                            if (ObjBatchMaster != null)
                            {

                                BatchMaster ObjBatchMasterData = new BatchMaster();

                                ObjBatchMasterData.BatchID = ObjBatchMaster.BatchID;
                                ObjBatchMasterData.Month = ObjBatchMaster.Month;
                                ObjBatchMasterData.Year = ObjBatchMaster.Year;
                                ObjBatchMasterData.IsPrevious = ObjBatchMaster.IsPrevious;
                                ObjBatchMasterData.IsDelete = ObjBatchMaster.IsDelete;
                                ObjBatchMasterData.Status = ObjBatchMaster.Status;
                                ObjBatchMasterData.PrePorcessTransCount = ObjBatchMaster.PrePorcessTransCount;
                                ObjBatchMasterData.PostPorcessTransCount = ObjBatchMaster.PostPorcessTransCount;



                                string xmlString = string.Empty;
                                string isValid = string.Empty;

                                string strXmlText = objXMLHelper.CreateXML(ObjBatchMasterData);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(strXmlText);
                                StringWriter sw = new StringWriter();
                                XmlTextWriter tx = new XmlTextWriter(sw);
                                xmlDoc.WriteTo(tx);
                                objResponseMessage = ObjBatchMasterData.SaveData(xmlDoc, userid);//, objlogin);
                                //objResponseMessage.code = 100;
                                //objResponseMessage.Message = "Visitor Details Saved Successfully";
                                xmlString = objXMLHelper.CreateXML(objResponseMessage);
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                            else
                            {
                                objResponseMessage.code = 108;
                                objResponseMessage.Message = CommonConstants.SaveErrorMsg;
                                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;//  return Request.CreateResponse(HttpStatusCode.Created, objUtilities.HandelAPIResponseException(ex));
                        }
                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }
        }

        //api/Masters/GetBatchMaster
        [HttpGet]
        public HttpResponseMessage GetBatchMasterData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetBatchMasterData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string Month = string.Empty;
                        string Year = string.Empty;
                        if (headers.Contains("Month"))
                        {
                            Month = headers.GetValues("Month").First();
                        }
                        if (headers.Contains("Year"))
                        {
                            Year = headers.GetValues("Year").First();
                        }
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        BatchMaster ObjBatchMaster = new BatchMaster();
                        // string[,] param = new string[2, 2] { { "SearchText", SearchText }, { "OutputType", OutputType } };
                        string[,] param = new string[2, 2] { { "Month", Month }, { "Year", Year } };
                        QueryString = CommonConstants.createXML(param, 2);
                        SerializeBatchMaster ObjserializeBatchMaster = ObjBatchMaster.GetBatchMasterData(QueryString, userid);//, objlogin);
                        ObjserializeBatchMaster.code = 100;
                        ObjserializeBatchMaster.Message = "Success";
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeBatchMaster);
                        return objHttpResponseMessage;

                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }


        //api/Masters/GetOracleData
        [HttpGet]
        public HttpResponseMessage GetOracleData()
        {
            try
            {
                CommonConstants.WriteLog("Calling GetOracleData API");
                System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
                string userid = string.Empty;
                if (headers.Contains("USER_ID"))
                {
                    userid = headers.GetValues("USER_ID").First();
                }

                string TokenID = string.Empty;
                if (headers.Contains("TOKEN_ID"))
                {
                    TokenID = headers.GetValues("TOKEN_ID").First();
                }
                if (userid.ToString() != "" && TokenID.ToString() != "")
                {
                    CommonConstants.WriteLog("Checking Token ID in DB");
                    if (checkTokenID(TokenID, userid))
                    {
                        string Month = string.Empty;
                        string Year = string.Empty;
                        if (headers.Contains("Month"))
                        {
                            Month = headers.GetValues("Month").First();
                        }
                        if (headers.Contains("Year"))
                        {
                            Year = headers.GetValues("Year").First();
                        }
                        CommonConstants.WriteLog("Token ID verification complete");
                        Request.Properties.Add("RequiredJSONConvertion", 0);
                        HttpResponseMessage objHttpResponseMessage = new HttpResponseMessage();
                        BatchMaster ObjBatchMaster = new BatchMaster();
                        // string[,] param = new string[2, 2] { { "SearchText", SearchText }, { "OutputType", OutputType } };
                        string[,] param = new string[2, 2] { { "Month", Month }, { "Year", Year } };
                        QueryString = CommonConstants.createXML(param, 2);
                        SerializeBatchMaster ObjserializeBatchMaster = new SerializeBatchMaster();
                        string result = ObjBatchMaster.GetOracleData(QueryString, userid);//, objlogin);
                        ObjserializeBatchMaster.code = 100;
                        ObjserializeBatchMaster.Message = result;
                        objHttpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, ObjserializeBatchMaster);
                        return objHttpResponseMessage;

                    }
                    else
                    {
                        ResponseMessage objResponseMessage = new ResponseMessage();
                        objResponseMessage.code = 108;
                        objResponseMessage.Message = "Token Expired!!";
                        return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                    }
                }
                else
                {
                    ResponseMessage objResponseMessage = new ResponseMessage();
                    objResponseMessage.code = 108;
                    objResponseMessage.Message = "User ID or Token ID is Missing!!";
                    return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
                }
            }
            catch (Exception ex)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                objResponseMessage.code = 108;
                objResponseMessage.Message = ex.Message.ToString();
                return Request.CreateResponse(HttpStatusCode.Created, objResponseMessage);
            }

        }

    }
}