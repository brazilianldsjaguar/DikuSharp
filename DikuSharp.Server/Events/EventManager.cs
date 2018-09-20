using DikuSharp.Server.Characters;
using DikuSharp.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Events
{
    public class EventManager
    {
        private List<MudEvent> globalEventQueue;
        private List<MudEvent> eventQueueBucket;
        private Random rand;

        public EventManager()
        {
            rand = new Random();

            //initialize the big bucket
            eventQueueBucket = new List<MudEvent>();

            //ths is used for game-wide events (i.e. EventOwner = Game)
            globalEventQueue = new List<MudEvent>();
        }

        /// <summary>
        /// Loads game events as well as the area resets.
        /// </summary>
        public void Initialize()
        {
            MudEvent e;

            e = new MudEvent
            {
                Func = GameWeather,
                EventType = EventType.GameWeather
            };
            AddGameEvent(e, 15 * Mud.PULSE_PER_SECOND);

            foreach (var area in Mud.I.Areas)
            {
                e = new MudEvent
                {
                    Func = AreaReset,
                    EventType = EventType.AreaReset
                };
                AddEvent(e, area, rand.Next(1, 10)); //reset all areas within the next 10 heartbeats (virtually instantly at startup)
            }
        }

        /// <summary>
        /// Main entry point for running items in the queues
        /// </summary>
        public void Heartbeat()
        {
            //Prefer for-loop to avoid iterating over a changing list
            for (int i = 0; i < eventQueueBucket.Count; i++)
            {
                var e = eventQueueBucket[i];

                //if we haven't gotten passes down to 0 yet, the event is not ready to fire.
                if (e.Passes-- > 0) { continue; }

                //if a function returns false, it means it didn't dequeue itself - so we will
                if (!e.Func(e))
                {
                    DequeueEvent(e, false);
                }
            }
        }

        #region Queue-related functions
        public bool EnqueueEvent(MudEvent e, int game_pulses)
        {
            if (e.OwnerType == EventOwnerType.Unowned)
            {
                //log bug, no owner of event!
                return false;
            }

            if (game_pulses < 1) { game_pulses = 1; }

            e.Passes = game_pulses;

            eventQueueBucket.Add(e);

            return true;
        }

        /// <summary>
        /// Dequeues an event from the event queue.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dequeue_global">If true, removes from MUD-wide eventqueue bucket. Irregardless, attempts to remove from owner list.</param>
        public void DequeueEvent(MudEvent e, bool dequeueFromBuckets)
        {
            if (dequeueFromBuckets)
            {
                eventQueueBucket.Remove(e);
            }

            switch (e.OwnerType)
            {
                case EventOwnerType.Game:
                    globalEventQueue.Remove(e);
                    break;
                case EventOwnerType.Area:
                    e.Area.Events.Remove(e);
                    break;
                case EventOwnerType.Character:
                    e.Character.Events.Remove(e);
                    break;
                case EventOwnerType.Object:
                    e.Object.Events.Remove(e);
                    break;
                case EventOwnerType.Room:
                    e.Room.Events.Remove(e);
                    break;
                case EventOwnerType.Connection:
                    e.Connection.Events.Remove(e);
                    break;
                default:
                    //log bug
                    break;
            }
        }

        public void AddEvent(MudEvent e, EventOwnerType ownerType, IEventContainer owner, int delay)
        {
            if (e.Func == null) { return; }
            if (e.EventType == EventType.None) { return; }
            if (ownerType != EventOwnerType.Game && owner == null) { return; }

            e.OwnerType = ownerType;

            //if it's a game, add it to global queue...
            if (ownerType == EventOwnerType.Game)
            {
                globalEventQueue.Add(e);
            }
            //otherwise add it to respective container
            else
            {
                e.Owner = owner;
                owner.Events.Add(e);
            }

            //then add it to the main buckets
            EnqueueEvent(e, delay);
        }

        public void AddGameEvent(MudEvent e, int delay)
        {
            AddEvent(e, EventOwnerType.Game, null, delay);
        }

        public void AddEvent(MudEvent e, Character ch, int delay)
        {
            AddEvent(e, EventOwnerType.Character, ch, delay);
        }

        public void AddEvent(MudEvent e, Room r, int delay)
        {
            AddEvent(e, EventOwnerType.Room, r, delay);
        }

        public void AddEvent(MudEvent e, MudObject obj, int delay)
        {
            AddEvent(e, EventOwnerType.Object, obj, delay);
        }

        public void AddEvent(MudEvent e, Connection conn, int delay)
        {
            AddEvent(e, EventOwnerType.Connection, conn, delay);
        }

        public void AddEvent(MudEvent e, Area area, int delay)
        {
            AddEvent(e, EventOwnerType.Area, area, delay);
        }
        #endregion

        #region Core events
        bool GameWeather(MudEvent e)
        {
            return false;
        }

        bool AreaReset(MudEvent e)
        {
            Area area;
            if ((area = e.Area) == null)
            {
                return false;
            }

            //reset all the rooms in the area...
            area.Reset();

            //requeue the event for this area
            int newTime = rand.Next(180, 300) * Mud.PULSE_PER_SECOND;
            AddEvent(new MudEvent { Func = AreaReset, EventType = EventType.AreaReset }, area, newTime);

            return false;
        }
        #endregion
    }
}
