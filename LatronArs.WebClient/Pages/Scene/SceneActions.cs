using System;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.WebClient.Pages.Scene
{
    public partial class SceneComponent
    {
        public static class SceneActions
        {
            private static void PickupTile(SceneComponent sceneComponent, int x, int y)
            {
                sceneComponent.OpenPickupWindow(x, y);
            }

            public static void MoveUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    var tile = scene.Tiles[scene.Player.CurrentTile.X][scene.Player.CurrentTile.Y - 1];
                    if (tile.Actor != null)
                    {
                        PickupTile(sceneComponent, tile.X, tile.Y);
                        return;
                    }

                    scene.Move(tile.X, tile.Y);
                }
            }

            public static void MoveDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    var tile = scene.Tiles[scene.Player.CurrentTile.X][scene.Player.CurrentTile.Y + 1];
                    if (tile.Actor != null)
                    {
                        PickupTile(sceneComponent, tile.X, tile.Y);
                        return;
                    }

                    scene.Move(tile.X, tile.Y);
                }
            }

            public static void MoveLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    var tile = scene.Tiles[scene.Player.CurrentTile.X - 1][scene.Player.CurrentTile.Y];
                    if (tile.Actor != null)
                    {
                        PickupTile(sceneComponent, tile.X, tile.Y);
                        return;
                    }

                    scene.Move(tile.X, tile.Y);
                }
            }

            public static void MoveRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    var tile = scene.Tiles[scene.Player.CurrentTile.X + 1][scene.Player.CurrentTile.Y];
                    if (tile.Actor != null)
                    {
                        PickupTile(sceneComponent, tile.X, tile.Y);
                        return;
                    }

                    scene.Move(tile.X, tile.Y);
                }
            }

            public static void SprintUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y - 1);
                }
            }

            public static void SprintDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y + 1);
                }
            }

            public static void SprintLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.CurrentTile.X - 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void SprintRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Sprint(scene.Player.CurrentTile.X + 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void InteractUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y - 1);
                }
            }

            public static void InteractDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y + 1);
                }
            }

            public static void InteractLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.CurrentTile.X - 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void InteractRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.CurrentTile.X + 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void InteractCenter(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    scene.Interact(scene.Player.CurrentTile.X + 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void PickupUp(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    PickupTile(sceneComponent, scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y - 1);
                }
            }

            public static void PickupDown(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    PickupTile(sceneComponent, scene.Player.CurrentTile.X, scene.Player.CurrentTile.Y + 1);
                }
            }

            public static void PickupLeft(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    PickupTile(sceneComponent, scene.Player.CurrentTile.X - 1, scene.Player.CurrentTile.Y);
                }
            }

            public static void PickupRight(SceneComponent sceneComponent)
            {
                var scene = sceneComponent.GameService?.CurrentScene;
                if (scene != null)
                {
                    PickupTile(sceneComponent, scene.Player.CurrentTile.X + 1, scene.Player.CurrentTile.Y);
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