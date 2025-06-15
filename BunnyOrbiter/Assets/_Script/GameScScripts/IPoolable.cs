public interface IPoolable
{
    void OnSpawn();  // Called when object is taken from pool
    void OnReturn(); // Called when object is returned to pool
}