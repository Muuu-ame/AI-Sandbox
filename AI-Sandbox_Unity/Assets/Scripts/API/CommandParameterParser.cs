using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CommandParameterParser
{
    public bool TryParse(
        GameCommand command,
        out object parsedParameters)
    {
        parsedParameters = null;

        if (command == null || command.parameters == null)
            return false;

        try
        {
            switch (command.type)
            {
                // Object
                case "create_object":
                case "delete_object":
                case "set_position":
                case "move_object":
                case "set_rotation":
                case "rotate_object":
                case "set_scale":
                case "set_color":
                case "set_active":
                    parsedParameters =
                        command.parameters.ToObject<ObjectParameters>();
                    break;

                // Physics
                case "set_gravity":
                case "set_mass":
                case "set_velocity":
                case "set_angular_velocity":
                case "set_linear_damping":
                case "set_angular_damping":
                case "set_use_gravity":
                case "set_kinematic":
                case "set_dynamic_friction":
                case "set_static_friction":
                case "set_bounciness":
                    parsedParameters =
                        command.parameters.ToObject<PhysicsParameters>();
                    break;

                // Game Rule
                case "set_jump_force":
                case "set_move_speed":
                case "set_rotation_speed":
                case "set_health":
                case "set_max_health":
                case "set_time_limit":
                case "set_win_condition":
                case "set_lose_condition":
                    parsedParameters =
                        command.parameters.ToObject<GameRuleParameters>();
                    break;

                // Environment
                case "set_light_color":
                case "set_light_intensity":
                case "set_ambient_color":
                case "set_fog":
                case "set_fog_density":
                case "set_sky_color":
                case "set_weather":
                    parsedParameters =
                        command.parameters.ToObject<EnvironmentParameters>();
                    break;

                // Camera
                case "set_camera_position":
                case "set_camera_rotation":
                case "set_field_of_view":
                case "set_camera_target":
                case "set_camera_mode":
                    parsedParameters =
                        command.parameters.ToObject<CameraParameters>();
                    break;

                default:
                    return false;
            }

            return parsedParameters != null;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
