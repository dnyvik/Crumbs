using System;

namespace Crumbs.Core.Exceptions
{
    public class InvalidCompositeIdException : Exception
    {
        public InvalidCompositeIdException() 
            : base("A composite id must either have a server id or a global id.") {}
    }
}