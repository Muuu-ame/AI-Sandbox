
public enum CommandType
{
    // Object
    CreateObject,
    DeleteObject,
    SetPosition,
    MoveObject,
    SetRotation,
    RotateObject,
    SetScale,
    SetColor,
    SetActive,

    // Physics
    SetGravity,
    SetMass,
    SetVelocity,
    SetAngularVelocity,
    SetLinearDamping,
    SetAngularDamping,
    SetUseGravity,
    SetKinematic,
    SetDynamicFriction,
    SetStaticFriction,
    SetBounciness,

    // Game Rule
    SetJumpForce,
    SetMoveSpeed,
    SetRotationSpeed,
    SetHealth,
    SetMaxHealth,
    SetTimeLimit,
    SetWinCondition,
    SetLoseCondition,

    // Environment
    SetLightColor,
    SetLightIntensity,
    SetAmbientColor,
    SetFog,
    SetFogDensity,
    SetSkyColor,
    SetWeather,

    // Camera
    SetCameraPosition,
    SetCameraRotation,
    SetFieldOfView,
    SetCameraTarget,
    SetCameraMode,

    // Game State
    SetGameState,
    SetTimeScale,
    PauseGame,
    ResumeGame
}
