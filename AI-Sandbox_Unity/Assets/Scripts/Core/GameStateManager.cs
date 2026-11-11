using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private GameState currentState = GameState.Playing;

    // ó‘Ô•ÏX
    public void SetState(GameState state)
    {
        currentState = state;
    }

    // ó‘Ôæ“¾
    public GameState GetCurrentState()
    {
        return currentState;
    }

    // ŠÔ”{—¦•ÏX
    public void SetTimeScale(float scale)
    {
        if (scale < 0f) return;

        Time.timeScale = scale;
    }

    // ŠÔ”{—¦æ“¾
    public float GetTimeScale()
    {
        return Time.timeScale;
    }

    // ŠÔ”{—¦ƒŠƒZƒbƒg
    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
}