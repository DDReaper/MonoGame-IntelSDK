﻿#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleLib.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        List<PrimitiveLine> foundPoints = new List<PrimitiveLine>();

        CaptureType captureType;

        int scale = 1;
        Point baseViewSize = new Point(320,240);

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
            captureType = CaptureType.IMAGE_TYPE_COLOUR;
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
            foundPoints.Clear();
            if (capture != null && capture.Nodes != null)
            {
                var nodes = capture.Nodes;

                for (int i = 0; i < nodes.Length; i++)
                {
                    for (int j = 0; j < nodes[i].Length; j++)
                    {
                        var node = nodes[i][j];
                        if (node.body <= 0) continue;
                        float sz = (0 == 0) ? 10 : ((node.radiusImage > 5) ? node.radiusImage : 5);
                        PrimitiveLine brush = new PrimitiveLine(graphics.GraphicsDevice);
                        brush.Colour = j > 5 ? Color.Red : Color.Green;
                        brush.CreateCircle(sz, 10);
                        brush.Position = new Vector2(node.positionImage.x * scale,node.positionImage.y * scale);
                        foundPoints.Add(brush);
                    }
                }
            }

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
            foreach (var item in foundPoints)
            {
                item.Render(spriteBatch);
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
