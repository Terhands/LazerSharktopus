using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class InputHandler
    {
        private KeyboardState keyboardState;
        private KeyboardState oldKeyboardState;
        private GamePadState gamePadState;
        private GamePadState oldGamePadState;

        private static Keys KEYUP = Keys.W;
        private static Keys KEYDOWN = Keys.S;
        private static Keys KEYLEFT = Keys.A;
        private static Keys KEYRIGHT = Keys.D;
        private static Keys KEYSTART = Keys.Enter;
        private static Keys KEYJUMP = Keys.Space;
        private static Keys KEYPULL = Keys.F;
        private static Keys KEYBOLT = Keys.E;
        private static Keys KEYGATHER = Keys.G;
        private static Keys KEYQUIT = Keys.Escape;

        private static float STICK_THRESHOLD = 0.3f;

        public enum InputTypes {up, down, left, right, start, jump, pull, bolt, gather, quit};
        public InputHandler()
        {
            keyboardState = Keyboard.GetState();
            oldKeyboardState = keyboardState;
            gamePadState = GamePad.GetState(PlayerIndex.One);
            oldGamePadState = gamePadState;
        }

        public void Update()
        {
            oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;
            keyboardState = Keyboard.GetState(); 
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public bool isPressed(InputTypes action)
        {
            switch (action)
            {
                case InputTypes.up:
                    return (keyboardState.IsKeyDown(KEYUP) || gamePadState.ThumbSticks.Left.Y > STICK_THRESHOLD);
                case InputTypes.down:
                    return (keyboardState.IsKeyDown(KEYDOWN) || gamePadState.ThumbSticks.Left.Y < -STICK_THRESHOLD);
                case InputTypes.left:
                    return (keyboardState.IsKeyDown(KEYLEFT) || gamePadState.ThumbSticks.Left.X < -STICK_THRESHOLD);
                case InputTypes.right:
                    return (keyboardState.IsKeyDown(KEYRIGHT) || gamePadState.ThumbSticks.Left.X > STICK_THRESHOLD);
                case InputTypes.start:
                    return (keyboardState.IsKeyDown(KEYSTART) || gamePadState.Buttons.Start == ButtonState.Pressed);
                case InputTypes.jump:
                    return (keyboardState.IsKeyDown(KEYJUMP) || gamePadState.Buttons.A == ButtonState.Pressed);
                case InputTypes.pull:
                    return (keyboardState.IsKeyDown(KEYPULL) || gamePadState.Buttons.B == ButtonState.Pressed);
                case InputTypes.bolt:
                    return (keyboardState.IsKeyDown(KEYBOLT) || gamePadState.Buttons.X == ButtonState.Pressed);
                case InputTypes.gather:
                    return (keyboardState.IsKeyDown(KEYGATHER) || gamePadState.Buttons.Y == ButtonState.Pressed); 
                case InputTypes.quit:
                    return (keyboardState.IsKeyDown(KEYQUIT));
                default:
                    return false;
            }
        }

        public bool isNewlyPressed(InputTypes action)
        {
            switch (action)
            {
                case InputTypes.up:
                    return ((keyboardState.IsKeyDown(KEYUP) && oldKeyboardState.IsKeyUp(KEYUP)) || (gamePadState.ThumbSticks.Left.Y > STICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.Y <= STICK_THRESHOLD));
                case InputTypes.down:
                    return ((keyboardState.IsKeyDown(KEYDOWN) && oldKeyboardState.IsKeyUp(KEYDOWN)) || (gamePadState.ThumbSticks.Left.Y < -STICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.Y >= -STICK_THRESHOLD));
                case InputTypes.left:
                    return ((keyboardState.IsKeyDown(KEYLEFT) && oldKeyboardState.IsKeyUp(KEYLEFT)) || (gamePadState.ThumbSticks.Left.X < -STICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.X >= -STICK_THRESHOLD));
                case InputTypes.right:
                    return ((keyboardState.IsKeyDown(KEYRIGHT) && oldKeyboardState.IsKeyUp(KEYRIGHT)) || (gamePadState.ThumbSticks.Left.X > STICK_THRESHOLD && oldGamePadState.ThumbSticks.Left.X <= STICK_THRESHOLD));
                case InputTypes.start:
                    return ((keyboardState.IsKeyDown(KEYSTART) && oldKeyboardState.IsKeyUp(KEYSTART)) || (gamePadState.Buttons.Start == ButtonState.Pressed && oldGamePadState.Buttons.Start == ButtonState.Released));
                case InputTypes.jump:
                    return ((keyboardState.IsKeyDown(KEYJUMP) && oldKeyboardState.IsKeyUp(KEYJUMP)) || (gamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released));
                case InputTypes.pull:
                    return ((keyboardState.IsKeyDown(KEYPULL) && oldKeyboardState.IsKeyUp(KEYPULL)) || (gamePadState.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released)); 
                case InputTypes.bolt:
                    return ((keyboardState.IsKeyDown(KEYBOLT) && oldKeyboardState.IsKeyUp(KEYBOLT)) || (gamePadState.Buttons.X == ButtonState.Pressed && oldGamePadState.Buttons.X == ButtonState.Released));
                case InputTypes.gather:
                    return ((keyboardState.IsKeyDown(KEYGATHER) && oldKeyboardState.IsKeyUp(KEYGATHER)) || (gamePadState.Buttons.Y == ButtonState.Pressed && oldGamePadState.Buttons.Y == ButtonState.Released));
                default:
                    return false;
            }
        }

        public bool isNewlyReleased(InputTypes action)
        {
            switch (action)
            {
                case InputTypes.up:
                    return ((oldKeyboardState.IsKeyDown(KEYUP) && keyboardState.IsKeyUp(KEYUP)) || (oldGamePadState.ThumbSticks.Left.Y > STICK_THRESHOLD && gamePadState.ThumbSticks.Left.Y <= STICK_THRESHOLD));
                case InputTypes.down:
                    return ((oldKeyboardState.IsKeyDown(KEYDOWN) && keyboardState.IsKeyUp(KEYDOWN)) || (oldGamePadState.ThumbSticks.Left.Y < -STICK_THRESHOLD && gamePadState.ThumbSticks.Left.Y >= -STICK_THRESHOLD));
                case InputTypes.left:
                    return ((oldKeyboardState.IsKeyDown(KEYLEFT) && keyboardState.IsKeyUp(KEYLEFT)) || (oldGamePadState.ThumbSticks.Left.X < -STICK_THRESHOLD && gamePadState.ThumbSticks.Left.X >= -STICK_THRESHOLD));
                case InputTypes.right:
                    return ((oldKeyboardState.IsKeyDown(KEYRIGHT) && keyboardState.IsKeyUp(KEYRIGHT)) || (oldGamePadState.ThumbSticks.Left.X > STICK_THRESHOLD && gamePadState.ThumbSticks.Left.X <= STICK_THRESHOLD));
                case InputTypes.start:
                    return ((oldKeyboardState.IsKeyDown(KEYSTART) && keyboardState.IsKeyUp(KEYSTART)) || (oldGamePadState.Buttons.Start == ButtonState.Pressed && gamePadState.Buttons.Start == ButtonState.Released));
                case InputTypes.jump:
                    return ((oldKeyboardState.IsKeyDown(KEYJUMP) && keyboardState.IsKeyUp(KEYJUMP)) || (oldGamePadState.Buttons.A == ButtonState.Pressed && gamePadState.Buttons.A == ButtonState.Released));
                case InputTypes.pull:
                    return ((oldKeyboardState.IsKeyDown(KEYPULL) && keyboardState.IsKeyUp(KEYPULL)) || (oldGamePadState.Buttons.B == ButtonState.Pressed && gamePadState.Buttons.B == ButtonState.Released));
                case InputTypes.bolt:
                    return ((oldKeyboardState.IsKeyDown(KEYBOLT) && keyboardState.IsKeyUp(KEYBOLT)) || (oldGamePadState.Buttons.X == ButtonState.Pressed && gamePadState.Buttons.X == ButtonState.Released));
                case InputTypes.gather:
                    return ((oldKeyboardState.IsKeyDown(KEYGATHER) && keyboardState.IsKeyUp(KEYGATHER)) || (oldGamePadState.Buttons.Y == ButtonState.Pressed && gamePadState.Buttons.Y == ButtonState.Released));
                default:
                    return false;
            }
            return false;
        }
    }
}