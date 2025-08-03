using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace FYP.ReceiverApp
{
    public static class ActionExecutor
    {
        private static InputSimulator sim = new InputSimulator();
        private static HashSet<string> heldKeys = new();

        public static void Execute(string action, string state)
        {
            switch (state)
            {
                case "down":
                    if (!heldKeys.Contains(action))
                    {
                        Press(action);
                        heldKeys.Add(action);
                    }
                    break;

                case "hold":
                    if (!heldKeys.Contains(action))
                    {
                        Press(action);
                        heldKeys.Add(action);
                    }
                    break;

                case "up":
                    if (heldKeys.Contains(action))
                    {
                        Release(action);
                        heldKeys.Remove(action);
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown state: {state}");
                    break;
            }
        }

        private static void Press(string action)
        {
            Console.WriteLine($"→ Press {action}");

            switch (action)
            {
                case "Key_W": sim.Keyboard.KeyDown(VirtualKeyCode.VK_W); break;
                case "Key_A": sim.Keyboard.KeyDown(VirtualKeyCode.VK_A); break;
                case "Key_S": sim.Keyboard.KeyDown(VirtualKeyCode.VK_S); break;
                case "Key_D": sim.Keyboard.KeyDown(VirtualKeyCode.VK_D); break;
                case "LeftClick": sim.Mouse.LeftButtonDown(); break;
                case "RightClick": sim.Mouse.RightButtonDown(); break;
                default: Console.WriteLine($"Unknown press action: {action}"); break;
            }
        }

        private static void Release(string action)
        {
            Console.WriteLine($"→ Release {action}");

            switch (action)
            {
                case "Key_W": sim.Keyboard.KeyUp(VirtualKeyCode.VK_W); break;
                case "Key_A": sim.Keyboard.KeyUp(VirtualKeyCode.VK_A); break;
                case "Key_S": sim.Keyboard.KeyUp(VirtualKeyCode.VK_S); break;
                case "Key_D": sim.Keyboard.KeyUp(VirtualKeyCode.VK_D); break;
                case "LeftClick": sim.Mouse.LeftButtonUp(); break;
                case "RightClick": sim.Mouse.RightButtonUp(); break;
                default: Console.WriteLine($"Unknown release action: {action}"); break;
            }
        }
    }
}
