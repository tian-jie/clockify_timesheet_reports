using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;

namespace Innocellence.Authentication.Controllers
{
    public class SsoController : Controller
    {

        ILogger log;
        ISysUserService _userManager;
        private IAuthenticationService _authService;
        ISysUserService _sysUserService;

        public SsoController(ISysUserService userManager, IAuthenticationService authService, ISysUserService sysUserService)
        {
            _userManager = userManager;
            _authService = authService;
            _sysUserService = sysUserService;
            log = LogManager.GetLogger(typeof(OWinLoginController));
        }
        //
        //[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SsoResult(string samlResponse, string RelayState)
        {
            // LogManager.GetLogger(this.GetType()).Debug("Entering SsoResult... RelayState=" + RelayState);
            // string XXX = GetUserIdFromSaml(samlResponse, RelayState);
            // LogManager.GetLogger(this.GetType()).Debug("SsoResult - XXX : " + XXX);

            // TODO: 应该在生产服务器上完成，但现在DMZ不能主动连外网，只能搞到外网的测试服务器上测试了。
            //ViewBag.samlResponse = samlResponse;
            //ViewBag.RelayState = RelayState;
            //return View();


            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                //return RedirectToAction("Login");

                var claims = AuthenticationManager.AuthenticationResponseGrant.Identity.Claims.FirstOrDefault(a =>
                    a.Type == ClaimTypes.NameIdentifier);
                if (claims != null)
                {

                    loginInfo = new Microsoft.AspNet.Identity.Owin.ExternalLoginInfo()
                    {
                        Login = new Microsoft.AspNet.Identity.UserLoginInfo("SAML2", claims.Value)
                    };
                }


            }

            if (string.IsNullOrEmpty(RelayState))
            {
                return Redirect("~/Subscribed/Subscrib/Subscribed?WeChatId=" + loginInfo.Login.ProviderKey);
            }

            var user = _sysUserService.AutoLogin(loginInfo.Login.ProviderKey);
            if (user == null)
            {
                string strMsg = string.Format("login OK,but user [{0}] not found!", loginInfo.Login.ProviderKey);
                log.Error(strMsg);
                return View("~/Views/Shared/Error.cshtml", new HandleErrorInfo(new Exception(strMsg), "OWinLogin", "SsoResult"));
            }
            await _authService.SignInAsync(user, true);


            //从主网站跳转过来
            if (RelayState.IndexOf("RelayState") < 0)
            {
                return Redirect(RelayState);
            }
            else if (RelayState.IndexOf(Request.Url.Host) >= 0)
            { //跳转的网站和当前网站是一个网站

                var index = RelayState.IndexOf("RelayState");
                return Redirect(RelayState.Substring(index + 11));

            }
            else
            {
                // 如果是其他网站的请求，直接返回给对方网站
                ViewBag.samlResponse = samlResponse;
                ViewBag.RelayState = RelayState;
                return View();


            }


        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private string GetUserIdFromSaml(string saml, string RelayState)
        {
            LogManager.GetLogger(this.GetType()).Debug("Entering GetUserIdFromSaml...");
            // TODO: ddd
            ViewBag.Saml = "Empty";
            ViewBag.UserID = "Empty";
            ViewBag.RtnURL = "Empty";
            try
            {
                if (!string.IsNullOrEmpty(saml))
                {
                    string samlResponseXml = Encoding.Default.GetString(Convert.FromBase64String(saml));

                    string certFileName = GetCertFileName();
                    //if (System.IO.File.Exists(certFileName))
                    {
                        string pfxPassword = string.Empty;
                        X509Certificate2 certificate = new X509Certificate2(certFileName, pfxPassword);

                        // TODO: verify the saml and RelayState.
                        XmlDocument doc = new XmlDocument { PreserveWhitespace = true };

                        // Load the passed XML file using its name.
                        doc.Load(new StringReader(samlResponseXml));

                        ViewBag.Saml = doc.ToString();
                        LogManager.GetLogger(this.GetType()).Debug("GetUserIdFromSaml - Saml is :" + ViewBag.Saml);
                        // Create a SignedXml object.
                        var checkSignature = CheckSignature(doc, saml, samlResponseXml, certificate);

                        if (checkSignature)
                        {
                            string userName = GetResponseNameID(doc, samlResponseXml);
                            LogManager.GetLogger(this.GetType()).Debug("GetUserIdFromSaml - GetResponseNameID is :" + userName);
                            ViewBag.UserID = userName;
                            string host = "";
                            if (!string.IsNullOrEmpty(RelayState))
                            {
                                var rtnURL = RelayState;
                                Uri u = new Uri(rtnURL);
                                ViewBag.RtnURL = rtnURL;
                                // 取出returnUrl的网站部分，和后面的sso/ssoresult合并
                                int end = rtnURL.IndexOf('/', 10);
                                host = rtnURL.Substring(0, end);
                                //host = u.Host;
                                ViewBag.targetAction = host + "/sso/ssoresult";
                            }
                            else
                            {
                                ViewBag.targetAction = "";
                            }
                            return userName;

                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    //else
                    //{
                    //    return "Configuration is not correct. - " + certFileName;
                    //}
                }
                else
                {
                    //Logger.Error("saml is null or empty.");
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, "SAML Login failed, Request Url: {0}, UserHostAddress: {1}, UserAgent: {2}.", Request.Url, Request.UserHostAddress, Request.UserAgent);
            }

            return string.Empty;
        }

        private string GetCertFileName()
        {
            var fileName = Server.MapPath("~/cert.crt");
            var f1 = Server.MapPath("/consent_content/hm.gif");
            LogManager.GetLogger(this.GetType()).Debug("fileName is :" + fileName + " - " + System.IO.File.Exists(fileName));
            LogManager.GetLogger(this.GetType()).Debug("other is :" + f1 + " - " + System.IO.File.Exists(f1));
            return fileName;
        }

        private bool CheckSignature(XmlDocument doc, string SAMLResponse, string samlResponseXml, X509Certificate2 certificate)
        {
            try
            {
                XmlElement samlAssertionNode = doc.GetElementsByTagName("saml:Assertion")[0] as XmlElement;
                var responseSignature = doc.GetElementsByTagName("ds:Signature")[0];
                if (samlAssertionNode != null && responseSignature != null)
                {
                    return CheckSignature(samlAssertionNode, certificate.PublicKey.Key, certificate);
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, "CheckSignature: {0}, {1}", doc, certificate);
            }
            return false;
        }

        private static readonly string[] allowedTransforms = new string[]
        {
            SignedXml.XmlDsigEnvelopedSignatureTransformUrl,
            SignedXml.XmlDsigExcC14NTransformUrl,
            SignedXml.XmlDsigExcC14NWithCommentsTransformUrl
        };

        private static bool CheckSignature(XmlElement signedRootElement, AsymmetricAlgorithm idpKey, X509Certificate2 certificate)
        {
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(signedRootElement.OuterXml);

            var signature = xmlDocument.GetElementsByTagName("ds:Signature")[0] as XmlElement;
            if (signature == null)
            {
                return false;
            }

            var signedXml = new SignedXml(xmlDocument);
            signedXml.LoadXml(signature);

            var signedRootElementId = "#" + signedRootElement.GetAttribute("ID");
            var reference = signedXml.SignedInfo.References.Cast<Reference>().FirstOrDefault();
            if (signedXml.SignedInfo.References.Count != 1 || reference.Uri != signedRootElementId)
            {
                return false;
            }

            foreach (Transform transform in reference.TransformChain)
            {
                if (!allowedTransforms.Contains(transform.Algorithm))
                {
                    return false;
                }
            }

            return signedXml.CheckSignature(idpKey);
        }

        private string GetResponseNameID(XmlDocument doc, string samlResponseXml)
        {
            try
            {
                var nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                nsmgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
                nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
                // TODO: check <samlp:StatusCode Value="urn:oasis:names:tc:SAML:2.0:status:Success"/>.
                var nameIDNode = doc.SelectSingleNode("//saml:NameID", nsmgr);
                if (nameIDNode != null && nameIDNode.FirstChild != null)
                {
                    return nameIDNode.FirstChild.Value;
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, "GetResponseNameID: {0}, {1}", doc, samlResponseXml);
            }
            return null;
        }


    }
}
