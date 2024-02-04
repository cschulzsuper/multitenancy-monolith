using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public interface IPreBuildOutput
{
    Task WriteAsync(string target, byte[] bytes);
}