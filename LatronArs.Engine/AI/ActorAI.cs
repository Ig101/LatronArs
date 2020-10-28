using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.AI
{
    public class ActorAI
    {
        public bool DoAnalysis { get; set; }

        public Actor Parent { get; set; }

        public AIState State { get; set; }

        public Memory[][] Memories { get; }

        public ActorAI(bool doAnalysis, int width, int height)
        {
            DoAnalysis = doAnalysis;
            Memories = new Memory[width][];
            for (int i = 0; i < width; i++)
            {
                Memories[i] = new Memory[height];
            }

            State = AIState.Neutral;
        }

        public ActorAI(bool doAnalysis, Memory[][] memories, AIState state)
        {
            DoAnalysis = doAnalysis;
            Memories = memories;
            State = state;
        }
    }
}