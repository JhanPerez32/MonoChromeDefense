using System;

public interface IPoolable
{
    Object OriginalPrefab { get; set; }
    
    void OnSpawn();
    void OnDespawn(); 
}
