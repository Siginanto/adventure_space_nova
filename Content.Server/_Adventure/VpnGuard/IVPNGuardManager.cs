using System.Threading.Tasks;
using System.Net;

namespace Content.Server._c4llv07e.VpnGuard;

public interface IVPNGuardManager
{
    public void Initialize();
    public Task<bool> IsConnectionVpn(IPAddress ip);
}
