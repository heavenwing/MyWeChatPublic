using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyWeChatPublic.Handlers;
using Rabbit.WeiXin;
using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.MvcExtension.Results;

namespace MyWeChatPublic.Controllers
{
    public class WeChatController : Controller
    {
        [HttpGet]
        public string Index(string signature, string timestamp, string nonce, string echostr)
        {
            var signatureService = DefaultDependencyResolver.Instance.GetService<ISignatureService>();
            if (signatureService.Check(signature, timestamp, nonce, "weixin"))
                return echostr;

            throw new Exception("非法请求。");
        }

        [HttpPost]
        public async Task<ActionResult> Index()
        {
            IHandlerBuilder builder = new HandlerBuilder();

            builder
#if !DEBUG
                .Use<SignatureCheckHandlerMiddleware>() //验证签名中间件。
#endif
                .Use<CreateRequestMessageHandlerMiddleware>() //创建消息中间件（内置消息解密逻辑）。
                .Use<SessionSupportHandlerMiddleware>() //会话支持中间件。
                .Use<IgnoreRepeatMessageHandlerMiddleware>() //忽略重复的消息中间件。
                .Use<CommandMessageHandlerMiddleware>() //每日精华文章消息处理中间件。
                .Use<GenerateResponseXmlHandlerMiddleware>(); //生成相应XML处理中间件（内置消息加密逻辑）。
            //                            .Use<AgentHandlerMiddleware>(new AgentRequestModel(new Uri("http://localhost:22479/Mutual")));

            var context = new HandlerContext(Request);

            //设置基本信息。
            context
                .SetMessageHandlerBaseInfo(new MessageHandlerBaseInfo(
                    ConfigurationManager.AppSettings["wx:AppId"],
                    ConfigurationManager.AppSettings["wx:AppSecret"],
                    "weixin"));

            IWeiXinHandler weiXinHandler = new DefaultWeiXinHandler(builder);
            await weiXinHandler.Execute(context);

            return new WeiXinResult(context);
        }
    }
}