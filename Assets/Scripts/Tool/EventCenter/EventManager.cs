using System;
using System.Collections.Generic;


namespace Farm.Tool
{
    interface IEvent { }

    public class Event : IEvent
    {
        public Action action;
    }
    public class Event<T> : IEvent
    {
        public Action<T> action;
    }
    public class Event<T1,T2> : IEvent
    {
        public Action<T1, T2> action;
    }
    public class Event<T1, T2,T3> : IEvent
    {
        public Action<T1, T2,T3> action;
    }
    public class Event<T1, T2, T3,T4> : IEvent
    {
        public Action<T1, T2, T3,T4> action;
    }
    public class Event<T1, T2, T3, T4, T5> : IEvent
    {
        public Action<T1, T2, T3, T4 ,T5> action;
    }

    public class EventManager 
    {
        private static Dictionary<string, IEvent> eventDict = new Dictionary<string, IEvent>();

        //添加监听
        public static void AddEventListener(string eventName,Action action)
        {
            if(!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName,new Event());
            }
            (eventDict[eventName] as Event).action += action;
        }
        public static void AddEventListener<T>(string eventName,Action<T> action)
        {
            if(!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName, new Event<T>());
            }
            (eventDict[eventName] as Event<T>).action += action;
        }
        public static void AddEventListener<T1,T2>(string eventName, Action<T1,T2> action)
        {
            if (!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName, new Event<T1,T2>());
            }
            (eventDict[eventName] as Event<T1,T2>).action += action;
        }
        public static void AddEventListener<T1, T2,T3>(string eventName, Action<T1, T2, T3> action)
        {
            if (!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName, new Event<T1, T2,T3>());
            }
          (eventDict[eventName] as Event<T1, T2, T3>).action += action;
        }
        public static void AddEventListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3,T4> action)
        {
            if (!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName, new Event<T1, T2, T3,T4>());
            }
            (eventDict[eventName] as Event<T1, T2, T3,T4>).action += action;
        }
        public static void AddEventListener<T1, T2, T3, T4,T5>(string eventName, Action<T1, T2, T3, T4,T5> action)
        {
            if (!eventDict.ContainsKey(eventName))
            {
                eventDict.Add(eventName, new Event<T1, T2, T3, T4,T5>());
            }
            (eventDict[eventName] as Event<T1, T2, T3, T4,T5>).action += action;
        }

        //移除监听
        public static void RemoveEventListener(string eventName, Action action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event).action -= action;
        }
        public static void RemoveEventListener<T>(string eventName, Action<T> action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T>).action -= action;
        }
        public static void RemoveEventListener<T1, T2>(string eventName, Action<T1, T2> action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2>).action -= action;
        }
        public static void RemoveEventListener<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
          (eventDict[eventName] as Event<T1, T2, T3>).action -= action;
        }
        public static void RemoveEventListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2, T3, T4>).action -= action;
        }
        public static void RemoveEventListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2, T3, T4, T5>).action -= action;
        }

        //调用
        public static void InvokeEventListener(string eventName)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event).action?.Invoke();
        }
        public static void InvokeEventListener<T>(string eventName,T t)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T>).action?.Invoke(t);
        }
        public static void InvokeEventListener<T1,T2>(string eventName, T1 t1,T2 t2)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1,T2>).action?.Invoke(t1,t2);
        }
        public static void InvokeEventListener<T1, T2,T3>(string eventName, T1 t1, T2 t2,T3 t3)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2,T3>).action?.Invoke(t1, t2,t3);
        }
        public static void InvokeEventListener<T1, T2, T3,T4>(string eventName, T1 t1, T2 t2, T3 t3,T4 t4)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2, T3,T4>).action?.Invoke(t1, t2, t3, t4);
        }
        public static void InvokeEventListener<T1, T2, T3, T4,T5>(string eventName, T1 t1, T2 t2, T3 t3, T4 t4,T5 t5)
        {
            if (!eventDict.ContainsKey(eventName))
                return;
            (eventDict[eventName] as Event<T1, T2, T3, T4,T5>).action?.Invoke(t1, t2, t3, t4,t5);
        }
        //清除
        public static void Clear()
        {
            eventDict.Clear();
        }
    }

}
