using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.AI
{
    public class ActorAI
    {
        public Actor Parent { get; set; }

        public AIState State { get; set; }

        public Memory[][] Memories { get; }

        public ActorAI(int width, int height)
        {
            Memories = new Memory[width][];
            for (int i = 0; i < width; i++)
            {
                Memories[i] = new Memory[height];
            }

            State = AIState.Neutral;
        }

        public ActorAI(Memory[][] memories, AIState state)
        {
            Memories = memories;
            State = state;
        }
    }
}