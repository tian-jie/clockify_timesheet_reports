using System;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Service;
using Infrastructure.Utility.Data;
using Infrastructure.Core.Data;
using Infrastructure.Web.UI;
using Infrastructure.Core.Logging;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Configuration;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;

namespace Innocellence.WeChatMeeting.Controllers
{
    public class SamlController : Controller
    {
        public const string CERT_FILENAME = "ChinaWeChat-cert.crt";
        ISysUserService _userManager;


        public SamlController(ISysUserService userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SAML(string SAMLResponse, string RelayState)
        {
            try
            {
                LogManager.GetLogger(this.GetType()).Error("SAML  Parameter :" + SAMLResponse);
                LogManager.GetLogger(this.GetType()).Error("RelayState  Parameter :" + RelayState);

                if (!string.IsNullOrEmpty(SAMLResponse))
                {
                    string samlResponseXml = Encoding.Default.GetString(Convert.FromBase64String(SAMLResponse));

                    string certFileName = GetCertFileName();
                    if (System.IO.File.Exists(certFileName))
                    {
                        string pfxPassword = string.Empty;
                        X509Certificate2 certificate = new X509Certificate2(certFileName, pfxPassword);

                        // TODO: verify the SAMLResponse and RelayState.
                        XmlDocument doc = new XmlDocument { PreserveWhitespace = true };

                        // Load the passed XML file using its name.
                        doc.Load(new StringReader(samlResponseXml));

                        // Create a SignedXml object.
                        if (CheckSignature(doc, SAMLResponse, samlResponseXml, certificate))
                        {
                            string userName = GetResponseNameID(doc, samlResponseXml);

                            if (!string.IsNullOrWhiteSpace(userName))
                            {
                                var rtnURL = RelayState;

                                var user = new SysUser()
                                {
                                    //WeChatUserID = userName
                                };

                                await SignInAsync(user, true);

                                return Redirect(Session["ReturnUrl"].ToString());
                            }
                        }
                        else
                        {
                            return HttpNotFound();
                        }
                    }
                    else
                    {
                        return Content("Configuration is not correct.");
                    }
                }
                else
                {
                    LogManager.GetLogger(this.GetType()).Error("SAMLResponse is null or empty.");
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType()).Error("SAML Login failed, Request Url: {0}, UserHostAddress: {1}, UserAgent: {2}.", Request.Url, Request.UserHostAddress, Request.UserAgent);
            }

            return null;
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
                LogManager.GetLogger(this.GetType()).Error("CheckSignature: {0}, {1}", doc, certificate);
            }
            return false;
        }

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
        
        private string GetCertFileName()
        {
            var certFileName = CERT_FILENAME;
            var fileName = Server.MapPath("App_Data/" + certFileName);
            return fileName;
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
                LogManager.GetLogger(this.GetType()).Error("GetResponseNameID: {0}, {1}", doc, samlResponseXml);
            }
            return null;
        }

        private static readonly string[] allowedTransforms = new string[]
        {
            SignedXml.XmlDsigEnvelopedSignatureTransformUrl,
            SignedXml.XmlDsigExcC14NTransformUrl,
            SignedXml.XmlDsigExcC14NWithCommentsTransformUrl
        };

        private async Task SignInAsync(SysUser user, bool isPersistent)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.UserContext.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            Session["UserInfo"] = user;
        }
    }
}
