using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;

namespace OccultCrescentHelper.Carrots;

public class Carrot
{
    private readonly IGameObject gameObject;

    public static Vector4 color = new Vector4(0.93f, 0.57f, 0.13f, 1f);

    public Carrot(IGameObject obj)
    {
        gameObject = obj;
    }

    public bool IsValid() => gameObject != null && !gameObject.IsDead && gameObject.IsValid();

    public Vector3 GetPosition() => gameObject.Position;
}
