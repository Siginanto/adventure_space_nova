using System.Threading.Tasks;
using System.Net;

namespace Content.Server._Adventure.VpnGuard;

public interface IVpnGuardManager
{
    public void Initialize();
    public Task<bool> Check(IPAddress ip);
}
