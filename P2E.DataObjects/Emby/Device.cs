using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Devices;

namespace P2E.DataObjects.Emby
{
    public class Device : IDevice
    {
        public event EventHandler<EventArgs> ResumeFromSleep;

        public string DeviceName => Environment.MachineName;
        public string DeviceId => NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .Select(x => x.GetPhysicalAddress()
                .ToString())
                .FirstOrDefault();

        public Task<IEnumerable<LocalFileInfo>> GetLocalPhotos()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LocalFileInfo>> GetLocalVideos()
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(LocalFileInfo file, IApiClient apiClient, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}