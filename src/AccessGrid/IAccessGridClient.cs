using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Interface for the AccessGrid API client
    /// </summary>
    public interface IAccessGridClient : IDisposable
    {
        /// <summary>
        /// Service for managing access cards
        /// </summary>
        AccessCardsService AccessCards { get; }

        /// <summary>
        /// Service for managing console operations (enterprise features)
        /// </summary>
        ConsoleService Console { get; }
    }
}