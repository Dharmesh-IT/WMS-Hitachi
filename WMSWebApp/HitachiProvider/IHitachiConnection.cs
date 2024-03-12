using System.Threading.Tasks;
using System;

namespace WMSWebApp.HitachiProvider
{
    public interface IHitachiConnection
    {
        public Task<String> SendGRNToHitachi(string request);
        public Task<LoginResponse> Auth();
        public Task<HitachiResponse> SubmitGrnNotification(string grnObject);
        public Task<HitachiResponse> SubmitOrderNotification(string orderObject);
    }
}
