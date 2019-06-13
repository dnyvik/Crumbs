using System;

namespace Crumbs.Core.Command
{
    public interface ICommandSerializer
    {
        string Serialize(ICommand command);
        T Deserialize<T>(string data) where T : ICommand;
        ICommand Deserialize(string data, Type type);
    }
}