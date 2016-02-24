using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using RockSatC_2016.Abstract;
using RockSatC_2016.Event_Data;
using RockSatC_2016.Flight_Computer;

namespace RockSatC_2016.Utility {
    public static class ThreadPool {

        private static readonly object Locker = new object();
        private static readonly ArrayList AvailableThreads = new ArrayList();
        private static readonly Queue ThreadActions = new Queue();
        private static readonly ManualResetEvent ThreadSynch = new ManualResetEvent(false);
        private const int MaxThreads = 3;
        //private static readonly Hashtable PersistantThreads = new Hashtable();
        public static int PersistantThreadCount { get; private set; }

        public static void Start(ThreadStart start) {
            try {
                var thread = new Thread(start);
                thread.Start();
            }
            catch (Exception e) {
                Debug.Print("ThreadPool: Error starting thread: " + e.Message + e.InnerException);
                Debug.Print("Stack trace: " + e.StackTrace);
            }
        }

        public class WorkItem {
            public readonly ThreadStart Action = null;
            public readonly EventType EventType = EventType.None; 
            public readonly IEventData EventData = null;
            public WorkItem() {}

            public WorkItem(ThreadStart action, EventType type = EventType.None, IEventData eventData = null, bool isPersistent = false) {
                Action = action;
                EventType = type;
                EventData = eventData;
                IsPersistent = isPersistent;
            }

            public bool IsPersistent { get; private set; }

            public void SetRepeat(bool b) {
                IsPersistent = b;
            }
        }

        
        public static void QueueWorkItem(WorkItem workItem) {
          
                //queue the work action 
                lock (ThreadActions) {
                    ThreadActions.Enqueue(workItem);
                }


                //if we have less ThreadWorkers working than our MaxThreads, go ahead and spin one up.
                if (AvailableThreads.Count < MaxThreads)
                {
                    var thread = new Thread(ThreadWorker);

                    AvailableThreads.Add(thread);
                    thread.Start();
                }

                //pulse all ThreadWorkers
                lock (Locker) {
                    ThreadSynch.Set();
                }
            //}
        }


        private static void ThreadWorker()
        {

            while (true)
            {
                //Wait for pulse from ThreadPool, signifying a new work item has been queued
                // ReSharper disable once InconsistentlySynchronizedField
                ThreadSynch.WaitOne();

                var workItem = new WorkItem();
                //critical section
                lock (ThreadActions) {
                    if (ThreadActions.Count > 0)
                        //pull the next work item off of the queue
                        workItem = ThreadActions.Dequeue() as WorkItem;
                    else {
                        //the thread is empty, and we're in a locked section.. 
                        //reset the mutex so that this thread waits for the next pulse(when the next action is queued)
                        lock (ThreadSynch) {
                            ThreadSynch.Reset();
                            //continue;
                        }
                    }
                }
                //if we didn't get a work item out of the queue (it was empty) go back to waiting
                if (workItem?.Action == null) continue;

                //if we did get a work item, execute it's action.
                try
                {
                    workItem.Action();
                    //if the action was an event, eventData that the event has completed
                    if (workItem.EventType != EventType.None)
                        FlightComputer.Instance.TriggerEvent(workItem.EventType, workItem.EventData);

                    if (workItem.IsPersistent) QueueWorkItem(workItem);
                }
                catch (Exception e)
                {
                    Debug.Print("ThreadPool: Unhandled error executing action - " + e.Message + e.InnerException);
                    Debug.Print("StackTrace: " + e.StackTrace);
                }
            }

        }
        //private static void ThreadWorker_old() {

        //    while (true) {
        //        //Wait for pulse from ThreadPool, signifying a new work item has been queued
        //        ThreadSynch.WaitOne();

        //        ThreadStart workItem = null;

        //        //critical section
        //        lock (Locker) {
        //            if (ThreadActions.Count > 0)
        //                //pull the next action off of the queue
        //                workItem = ThreadActions.Dequeue() as ThreadStart;
        //            else {
        //                //the thread is empty, and we're in a locked section.. 
        //                //reset the mutex so that this loop waits for the next pulse, or till the next action is queued
        //                ThreadSynch.Reset();
        //            }
        //        }
        //        //if we didn't get a work item out of the queue (it was empty) go back to waiting
        //        if (workItem == null) continue;

        //        //if we did get a work item, execute it
        //        try {
        //            workItem();
        //        }
        //        catch (Exception e) {
        //            Debug.Print("ThreadPool: Unhandled error executing action - " + e.Message + e.InnerException);
        //            Debug.Print("StackTrace: " + e.StackTrace);
        //        }
        //    }

        //}
    }
}