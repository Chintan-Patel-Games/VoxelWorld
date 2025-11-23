using System;

namespace VoxelWorld.Core.Events
{
    public class EventController<T>
    {
        public event Action<T> baseEvent;
        public void InvokeEvent(T value) => baseEvent?.Invoke(value);
        public void AddListener(Action<T> listener) => baseEvent += listener;
        public void RemoveListener(Action<T> listener) => baseEvent -= listener;
    }
}