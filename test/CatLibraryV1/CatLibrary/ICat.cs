﻿using System;

namespace CatLibrary
{
    /// <summary>
    /// Cat's interface
    /// </summary>
    public interface ICat : IAnimal
    {
        //event
        /// <summary>
        /// eat event of cat. Every cat must implement this event.
        /// </summary>
        event EventHandler eat;
    }
}
