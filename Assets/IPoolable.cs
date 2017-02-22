using System;

public interface IPoolable
{
    event EventHandler Recycling;
}