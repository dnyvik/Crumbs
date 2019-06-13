using System;

namespace Crumbs.Core.Mediation
{
    public interface IMessageHandlerRegistry
    {
        void RegisterHandler(Type messageType, Type handlerType);
    }
}