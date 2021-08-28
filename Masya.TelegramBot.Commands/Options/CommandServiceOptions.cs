namespace Masya.TelegramBot.Commands.Options
{
    public sealed class CommandServiceOptions
    {
        public char ArgsSeparator { get; set; } = ' ';
        public int StepCommandTimeout { get; set; } = 30;
        public int MaxMenuColumns { get; set; } = 3;
    }
}