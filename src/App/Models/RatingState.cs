using System;

namespace Alan.HeicConverter.Models
{
    public class RatingState
    {
        public int SuccessfulBatchCount { get; set; } = 0;
        public bool HasRated { get; set; } = false;
        public bool OptedOut { get; set; } = false;
        public int PromptCount { get; set; } = 0;
        public DateTimeOffset? LastPromptUtc { get; set; } = null;
    }
}
