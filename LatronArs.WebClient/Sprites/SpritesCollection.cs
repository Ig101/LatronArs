using System.Collections.Generic;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Models;

namespace LatronArs.WebClient.Sprites
{
    public static class SpritesCollection
    {
        private static IEnumerable<SpriteVariation> LoadThief()
        {
            return new[]
            {
                new SpriteVariation
                {
                    Direction = Direction.Top,
                    Path = "sprites/thiefTop",
                    Mask = true
                },
                new SpriteVariation
                {
                    Direction = Direction.Top,
                    Path = "sprites/thiefBottom",
                    Mask = true
                },
                new SpriteVariation
                {
                    Direction = Direction.Right,
                    Path = "sprites/thiefRight",
                    Mask = true,
                    Mirrored = true
                }
            };
        }

        private static IEnumerable<SpriteVariation> LoadGuardian()
        {
            return new[]
            {
                new SpriteVariation
                {
                    Direction = Direction.Top,
                    Path = "sprites/guardianTop",
                    Mask = true
                },
                new SpriteVariation
                {
                    Direction = Direction.Top,
                    Path = "sprites/guardianBottom",
                    Mask = true
                },
                new SpriteVariation
                {
                    Direction = Direction.Right,
                    Path = "sprites/guardianRight",
                    Mask = true,
                    Mirrored = true
                }
            };
        }

        public static void FillSpritesCollection(this IDictionary<string, IEnumerable<SpriteVariation>> collection)
        {
            collection.Add("empty", new[]
            {
                new SpriteVariation
                {
                    Path = "sprites/empty",
                    Mask = true,
                }
            });
            collection.Add("guardian", LoadGuardian());
            collection.Add("thief", LoadThief());
            collection.Add("counter", new[]
            {
                new SpriteVariation
                {
                    Path = "sprites/counter",
                    Mask = true,
                }
            });
            collection.Add("torch", new[]
            {
                new SpriteVariation
                {
                    Path = "sprites/torch",
                    Mask = true,
                }
            });
            collection.Add("floor", new[]
            {
                new SpriteVariation
                {
                    Path = "sprites/floor",
                    IsSquare = true
                }
            });
            collection.Add("stoneFloor", new[]
            {
                new SpriteVariation
                {
                    Path = "sprites/stoneFloor",
                    IsSquare = true
                }
            });
        }
    }
}