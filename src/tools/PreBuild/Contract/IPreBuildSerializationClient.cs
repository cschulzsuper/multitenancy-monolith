using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public interface IPreBuildSerializationClient
{
    Task WriteAsync<T>(T @object);
}