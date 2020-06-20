using Infrastructure.Web.Domain.Service;
using Innocellence.Activity.Model;
using Innocellence.Activity.Services;
using Innocellence.Activity.ViewModel;
using Innocellence.WeChat.Domain;
using System;
using System.Web.Mvc;


namespace Innocellence.Activity.Controllers
{
    public class SignInController : BaseController<WXScreen, WXScreenView>
    {
        public SignInController(IWXScreenService objService)
            : base(objService)
        {
            ViewBag.TitlePic = GetSignInTitlePic();
        }

        /// <summary>
        /// 微信签到大屏幕
        /// </summary>
        /// <param name="width">每行XX个点</param>
        /// <param name="height">共XX行</param>
        /// <param name="word">字母拼写</param>
        /// <param name="eventId">活动ID</param>
        /// <param name="screenType">屏幕尺寸</param>
        /// <returns></returns>
        public ActionResult SignInScreen(int width, int height, string word, string eventId, string screenType)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(screenType))
            {
                return RedirectToAction("Failed");
            }
            ViewBag.EventId = eventId;
            ViewBag.ScreenType = screenType;

            return View(CheckinScreenService.CreateCheckinScreenConfig(width, height, word));
        }

        public ActionResult SignInScreenTest(int width, int height, string word, string eventId, string screenType)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(screenType))
            {
                return RedirectToAction("Failed");
            }

            ViewBag.EventId = eventId;
            ViewBag.ScreenType = screenType;

            return View(CheckinScreenService.CreateCheckinScreenConfig(width, height, word));
        }

        public ActionResult Failed()
        {
            return View();
        }

        private static string GetSignInTitlePic()
        {
            var config = CommonService.GetSysConfig("signInTitlePic", null);

            if (!string.IsNullOrEmpty(config) && config.Equals("null", StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            return config;
        }
    }
}