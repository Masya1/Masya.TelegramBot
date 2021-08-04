using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Masya.TelegramBot.Commands.Abstractions
{
    public interface IModule
    {
        ICommandContext Context { get; }
        Task<Message> ReplyAsync(
            string content,
            ParseMode parseMode = ParseMode.MarkdownV2,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null
            );
    }
}