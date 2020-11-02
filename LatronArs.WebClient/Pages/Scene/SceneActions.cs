using System;

namespace LatronArs.WebClient.Pages.Scene
{
    public partial class SceneComponent
    {
        public static class SceneActions
        {
            public static void MoveUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Move(scene.Player.Parent.X, scene.Player.Parent.Y - 1);
                }
            }

            public static void MoveDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Move(scene.Player.Parent.X, scene.Player.Parent.Y + 1);
                }
            }

            public static void MoveLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Move(scene.Player.Parent.X - 1, scene.Player.Parent.Y);
                }
            }

            public static void MoveRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Move(scene.Player.Parent.X + 1, scene.Player.Parent.Y);
                }
            }

            public static void SprintUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.Parent.X, scene.Player.Parent.Y - 1);
                }
            }

            public static void SprintDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.Parent.X, scene.Player.Parent.Y + 1);
                }
            }

            public static void SprintLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.Parent.X - 1, scene.Player.Parent.Y);
                }
            }

            public static void SprintRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.Parent.X + 1, scene.Player.Parent.Y);
                }
            }

            public static void InteractUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.Parent.X, scene.Player.Parent.Y - 1);
                }
            }

            public static void InteractDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.Parent.X, scene.Player.Parent.Y + 1);
                }
            }

            public static void InteractLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.Parent.X - 1, scene.Player.Parent.Y);
                }
            }

            public static void InteractRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.Parent.X + 1, scene.Player.Parent.Y);
                }
            }

            public static void InteractCenter(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.Parent.X + 1, scene.Player.Parent.Y);
                }
            }

            public static void PickupUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    // TODO OpenModal
                }
            }

            public static void PickupDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                }
            }

            public static void PickupLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                }
            }

            public static void PickupRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                }
            }

            public static void PickupCenter(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                }
            }
        }
    }
}