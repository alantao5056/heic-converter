using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Alan.HeicConverter.Services
{
    /// <summary>
    /// Decides when to ask the user to rate the app and orchestrates the soft-ask
    /// dialog plus the Store review call. State (counts, opt-out, cooldown) is persisted
    /// through <see cref="SettingsService"/>; an in-memory guard limits prompts to once
    /// per app session.
    /// </summary>
    internal class RatingService
    {
        private const int MinSuccessfulBatches = 3;
        private const int MaxPrompts = 3;
        private const int CooldownDays = 14;

        private readonly AppService _appService = new();
        private bool _promptedThisSession = false;

        /// <summary>
        /// Records that a conversion run completed with at least one successful file.
        /// </summary>
        public void RecordSuccessfulBatch()
        {
            SettingsService.IncrementSuccessfulBatchCount();
        }

        /// <summary>
        /// Returns true when all conditions to show the rating prompt are satisfied.
        /// </summary>
        public bool ShouldRequestReview()
        {
            if (_promptedThisSession)
            {
                return false;
            }

            var state = SettingsService.LoadRatingState();

            if (state.HasRated || state.OptedOut)
            {
                return false;
            }

            if (state.SuccessfulBatchCount < MinSuccessfulBatches)
            {
                return false;
            }

            if (state.PromptCount >= MaxPrompts)
            {
                return false;
            }

            if (state.LastPromptUtc.HasValue &&
                DateTimeOffset.UtcNow - state.LastPromptUtc.Value < TimeSpan.FromDays(CooldownDays))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Shows the soft-ask dialog and, if the user chooses to rate, invokes the Store
        /// review UI. Updates persisted state based on the user's choice.
        /// </summary>
        public async Task TryRequestReviewAsync(XamlRoot xamlRoot, IntPtr hwnd)
        {
            _promptedThisSession = true;

            var dialog = new ContentDialog
            {
                Title = "Enjoying HEIC Batch Converter?",
                Content = "If this app has been useful, a quick rating on the Microsoft Store " +
                          "really helps. It only takes a moment.",
                PrimaryButtonText = "Rate it",
                SecondaryButtonText = "Maybe later",
                CloseButtonText = "Don't ask again",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await dialog.ShowAsync();

            switch (result)
            {
                case ContentDialogResult.Primary:
                    SettingsService.RecordPromptShown(DateTimeOffset.UtcNow);
                    bool rated = await _appService.RequestRateAndReviewAsync(hwnd);
                    if (rated)
                    {
                        SettingsService.SetHasRated();
                    }
                    break;

                case ContentDialogResult.Secondary:
                    SettingsService.RecordPromptShown(DateTimeOffset.UtcNow);
                    break;

                case ContentDialogResult.None:
                    // Close button -> "Don't ask again"
                    SettingsService.SetOptedOut();
                    break;
            }
        }
    }
}
