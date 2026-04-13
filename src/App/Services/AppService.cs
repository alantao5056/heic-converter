using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Services.Store;

namespace Alan.HeicConverter.Services
{
    internal class AppService
    {
        public async Task<bool> HasMandatoryUpdateAsync()
        {
            try
            {
                var package = Package.Current;
                var update = await package.CheckUpdateAvailabilityAsync();
                return update.Availability == PackageUpdateAvailability.Required;
            }
            catch (Exception)
            {
                // Catching exception in case the app is running unpacked or checking fails
                return false;
            }
        }

        public async Task<string> GetStoreProductIdAsync(IntPtr hwnd)
        {
            try
            {
                var storeContext = StoreContext.GetDefault();
                WinRT.Interop.InitializeWithWindow.Initialize(storeContext, hwnd);
                var result = await storeContext.GetStoreProductForCurrentAppAsync();

                if (result.ExtendedError == null && result.Product != null)
                {
                    return result.Product.StoreId;
                }
            }
            catch (Exception)
            {
                // App might not be running from the Store or properly associated in the current context
            }
            return string.Empty;
        }
    }
}
