using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public struct Subject
    {
        public Type type;
        public IEventListener listener;

        public Subject(Type type, IEventListener listener)
        {
            this.type = type;
            this.listener = listener;
        }
    }

    public class EventManager : SingletonBase<EventManager>
    {
        private List<Subject> subjects;
        private List<Subject> removeRequests;
        private List<Tuple<Type, Event>> publishRequests;
        private List<Tuple<IEventListener, Event>> sendRequests;
        
        private void Update()
        {
            foreach(var req in removeRequests)
            {
                var type = req.type;
                var listener = req.listener;
                for(int i = subjects.Count - 1; i >= 0; --i)
                {
                    if(subjects[i].type.Equals(type) && ReferenceEquals(listener, subjects[i].listener))
                    {
                        subjects.RemoveAt(i);
                    }
                }
            }
            removeRequests.Clear();

            foreach(var req in publishRequests)
            {
                var type = req.Item1;
                var e = req.Item2;
                foreach(var subject in subjects)
                {
                    if(subject.type.Equals(type))
                    {
                        subject.listener.OnEvent(e);
                    }
                }
            }
            publishRequests.Clear();

            foreach(var req in sendRequests)
            {
                var listener = req.Item1;
                var e = req.Item2;
                listener.OnEvent(e);
            }
            sendRequests.Clear();
        }

        public void Subscribe<T>(IEventListener listener) where T : Event
        {
            subjects.Add(new Subject(typeof(T), listener));
        }

        public void UnSubscribe<T>(IEventListener listener) where T : Event
        {
            removeRequests.Add(new Subject(typeof(T), listener));
        }

        public void UnSubscribeAll(IEventListener listener)
        {
            foreach(var subject in subjects)
            {
                if(ReferenceEquals(subject.listener, listener))
                {
                    removeRequests.Add(subject);
                }
            }
        }

        public void Publish(Event e)
        {
            publishRequests.Add(new Tuple<Type, Event>(e.GetType(), e));
        }

        public void PublishSync(Event e)
        {
            foreach(var req in publishRequests)
            {
                foreach(var subject in subjects)
                {
                    if(subject.type.Equals(e.GetType()))
                    {
                        subject.listener.OnEvent(e);
                    }
                }
            }
        }

        public void Send(IEventListener listener, Event e)
        {
            sendRequests.Add(new Tuple<IEventListener, Event>(listener, e));
        }

        public void SendSync(IEventListener listener, Event e)
        {
            listener.OnEvent(e);
        }
    }
}