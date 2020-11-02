using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Content
{
    public static class ActionsManager
    {
        public static void PlayerInteraction(Tile target, Actor issuer)
        {
            int time = 0;
            if (target.Actor != null)
            {
                time = target.Actor.InteractionReaction(target.Actor, issuer);
            }
            else
            {
                time = target.InteractionReaction(target, issuer);
            }

            issuer.ActionDebt += time;
        }

        public static void GuardianInteraction(Tile target, Actor issuer)
        {
            // TODO
        }

        public static void SimpleMove(Tile target, Actor issuer, ActionInfo action)
        {
            issuer.CurrentTile.Actor = null;
            issuer.CurrentTile = target;
            target.Actor = issuer;
        }

        public static int SwitchLight(ILightBox target, Actor issuer)
        {
            var (noise, time) = target.SwitchLight();
            if (noise > 0)
            {
                issuer.IssueNoise(target.CurrentTile, noise, null);
            }

            return (int)(time * issuer.PickupTimeCost);
        }
    }
}