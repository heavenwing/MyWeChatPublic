using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyWeChatPublic.Managers;
using MyWeChatPublic.Models;
using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.MP.Messages.Response;

namespace MyWeChatPublic.Handlers
{
    public class CommandMessageHandlerMiddleware : MessageHandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public CommandMessageHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        public override IResponseMessage OnEvent_SubscribeRequest(SubscribeEventMessage requestMessage)
        {
            using (var manager = new ArticleManager())
            {
                return manager.GetWelcome();
            }
        }

        public override IResponseMessage OnTextRequest(RequestMessageText requestMessage)
        {
            var content = requestMessage.Content.Trim().ToLower();
            if (content == "help")
            {
                using (var manager = new ArticleManager())
                {
                    return manager.GetHelp();
                }
            }
            if (content == "about")
            {
                using (var manager = new ArticleManager())
                {
                    return manager.GetAbout();
                }
            }
            if (content == "top")
            {
                using (var manager = new ArticleManager())
                {
                    return manager.GetTop();
                }
            }
            if (content == "next")
                return new ResponseMessageText("此命令功能尚未开发完成，请稍后再试");
            if (content == "tags")
            {
                using (var manager = new ArticleManager())
                {
                    return manager.GetAllTags();
                }
            }
            if (content.Length == 8)
            {
                long publishDate;
                if (long.TryParse(content, out publishDate))
                {
                    using (var manager = new ArticleManager())
                    {
                        return manager.GetByPublishDate(content);
                    }
                }
            }
            IResponseMessage responseForTag;
            using (var manager = new ArticleManager())
            {
                responseForTag = manager.GetByTag(content);
            }

            return responseForTag ?? new ResponseMessageText("无此命令或关键字，请发送 help 来获得帮助");
        }


    }
}
