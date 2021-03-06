﻿#region Using Statements
using IntelPCSDK_Manager;
using IntelPCSDK_Manager.Helpers;
using IntelPCSDK_Manager.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


#endregion

namespace SimpleLib
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D DepthDisplayImage;
        Texture2D ColourDisplayImage;

        IVideoCapture capture;

        InputHandler inputHandler = new InputHandler();

        //List<PrimitiveLine> foundPoints = new List<PrimitiveLine>();

        CaptureType captureType;

        SpriteFont font;

        int scale = 1;
        int inputTolerance = 2;
        Point baseViewSize = new Point(320,240);

        PrimitiveLine touchBrush;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ///Comment out or add each implementation to test it's use
            ///* Note each lib has different capabilities, sone only deal with video streams whilst others have gesture and recognition capabilities
            ///

            ///Ultil M capture - colour only
            //captureType = CaptureType.IMAGE_TYPE_COLOUR;
            //capture = new UtilMCaptureSession();

            ///Util M capture - depth display with gesture support
            //captureType = CaptureType.IMAGE_TYPE_DEPTH;
            //capture = new UtilMCaptureDepthSession();

            ///Util M Pipeline - stream plus capture
            captureType = CaptureType.NONE;
            capture = new UtilMPipelineSession();

            ///PXC M Pipeline - only evaluating streams at present
            //capture = new PXCMCaptureSession();

            ///PXC M Pipeline (varient) - only evaluating streams at present
            //capture = new PXCMCaptureSessionMultiple();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            capture.Initialise(captureType);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            DepthDisplayImage = new Texture2D(GraphicsDevice, capture.Width, capture.Height);
            ColourDisplayImage = new Texture2D(GraphicsDevice, capture.Width, capture.Height);
            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>(@"DebugFont");
            inputHandler.Hands.EnableDebug(GraphicsDevice);

            touchBrush = new PrimitiveLine(GraphicsDevice);
            touchBrush.Colour = Color.Yellow;
            touchBrush.CreateCircle(10, 10);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (capture.DepthFrame != null && capture.DepthFrame.Length > 0)
            {
                DepthDisplayImage = capture.DepthFrame.ToTexture2D(graphics.GraphicsDevice, capture.Width, capture.Height);
            }
            if (capture.ColourFrame != null && capture.ColourFrame.Length > 0)
            {
                ColourDisplayImage = capture.ColourFrame.ConvertBetweenBGRAandRGBA(capture.Width,capture.Height).ToTexture2D(graphics.GraphicsDevice, capture.Width, capture.Height);
            }
            inputHandler.Hands.Recognise(capture.Nodes);

            base.Update(gameTime);
        }
  
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(DepthDisplayImage, new Rectangle(0, 0, (baseViewSize.X * scale), (baseViewSize.Y * scale)), Color.White);
            spriteBatch.Draw(ColourDisplayImage, new Rectangle((baseViewSize.X * scale), 0, (baseViewSize.X * scale), (baseViewSize.Y * scale)), Color.White);

            ///Draw Debug Text
            spriteBatch.DrawString(font, "Left hand Image Pos - Raw: " + inputHandler.Hands.PrimaryHandImagePositionRaw(), new Vector2(20, (baseViewSize.Y * scale) + 20), Color.White);
            spriteBatch.DrawString(font, "Left hand Image Pos - Smooth: " + inputHandler.Hands.PrimaryHandImagePositionSmoothed(), new Vector2(20, (baseViewSize.Y * scale) + 40), Color.White);
            spriteBatch.DrawString(font, "Left hand Image Pos - Within Tolerance: " + inputHandler.Hands.PrimaryHandImagePositionTolerance(inputTolerance), new Vector2(20, (baseViewSize.Y * scale) + 60), Color.White);
            spriteBatch.DrawString(font, "Left hand World Pos - Raw: " + inputHandler.Hands.PrimaryHandWorldPositionRaw(), new Vector2(20, (baseViewSize.Y * scale) + 100), Color.White);
            spriteBatch.DrawString(font, "Left hand World Pos - Smooth: " + inputHandler.Hands.PrimaryHandWorldPositionSmoothed(), new Vector2(20, (baseViewSize.Y * scale) + 120), Color.White);
            spriteBatch.DrawString(font, "Left hand World Pos - Within Tolerance: " + inputHandler.Hands.PrimaryHandWorldPositionTolerance(inputTolerance), new Vector2(20, (baseViewSize.Y * scale) + 140), Color.White);

            spriteBatch.DrawString(font, "Right hand Image Pos - Raw " + inputHandler.Hands.SecondaryHandImagePositionRaw(), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 20), Color.White);
            spriteBatch.DrawString(font, "Right hand Image Pos - Smooth " + inputHandler.Hands.SecondaryHandImagePositionSmoothed(), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 40), Color.White);
            spriteBatch.DrawString(font, "Right hand Image Pos - Within Tolerance: " + inputHandler.Hands.SecondaryHandImagePositionTolerance(inputTolerance), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 60), Color.White);
            spriteBatch.DrawString(font, "Right hand World Pos - Raw: " + inputHandler.Hands.SecondaryHandWorldPositionRaw(), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 100), Color.White);
            spriteBatch.DrawString(font, "Right hand World Pos - Smooth: " + inputHandler.Hands.SecondaryHandWorldPositionSmoothed(), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 120), Color.White);
            spriteBatch.DrawString(font, "Right hand World Pos - Within Tolerance: " + inputHandler.Hands.SecondaryHandWorldPositionTolerance(inputTolerance), new Vector2((baseViewSize.X * scale) + 100, (baseViewSize.Y * scale) + 140), Color.White);

            ///Draw all recognised inputs (note, turn on debug in recognise function to enable)
            //inputHandler.Hands.DebugDraw(spriteBatch);

            ///Draw selective body parts from nodes
            /*
            var primaryHand = inputHandler.Hands.GetBodyPart(PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_PRIMARY);
            if (primaryHand.body > 0)
            {
                primaryHand.DrawGeoNode(GraphicsDevice).Render(spriteBatch);
            }
            var secondaryHand = inputHandler.Hands.GetBodyPart(PXCMGesture.GeoNode.Label.LABEL_BODY_HAND_SECONDARY);
            if (secondaryHand.body > 0)
            {
                secondaryHand.DrawGeoNode(GraphicsDevice).Render(spriteBatch);
            }
            */

            ///Draw specific input handler parts
            inputHandler.Hands.PrimaryHandImagePositionTolerance(inputTolerance).DrawGeoNode(GraphicsDevice, Color.Red).Render(spriteBatch);
            inputHandler.Hands.SecondaryHandImagePositionTolerance(inputTolerance).DrawGeoNode(GraphicsDevice, Color.Green).Render(spriteBatch);

            foreach (var touch in inputHandler.Hands.Touches)
            {
                if (touch.State == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved)
                {
                    touchBrush.Position = touch.Position;
                    touchBrush.Render(spriteBatch);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            capture.Dispose(disposing);
            if(capture.State != null) while (capture.State != System.Threading.ThreadState.Stopped) { }
            base.Dispose(disposing);
        }


    }


}
