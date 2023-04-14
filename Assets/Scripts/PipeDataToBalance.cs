/******************************************************************
 * Created by: Jacob Welch (Technically most of this code was made by anirban but I put it here and cleaned it up some)
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/11/2023 4:35:05 PM
 * 
 * Description: Handles the piping of data between Unity applications
 *              and the Balance application.
******************************************************************/
using Assets.Scripts;
using System;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class PipeDataToBalance : Singleton<PipeDataToBalance>
{
    #region Fields
    /// <summary>
    /// The process used to pipe data between Unity and Balance.
    /// </summary>
    private Subprocess pipe;

    /// <summary>
    /// Holds true if the message has already been sent through the pipe.
    /// </summary>
    bool sentMessageToParent = false;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Calls for the pipe to be initialized.
    /// </summary>
    protected override void Awake()
    {
        if (Instance == null) PipingInitialization();

        base.Awake();
    }

    /// <summary>
    /// Initializes a named pipe if the game has been launched by Balance application
    /// </summary>
    private void PipingInitialization()
    {
        print("Initialization");

        var args = Environment.GetCommandLineArgs();
        Application.runInBackground = true;

        string pipeName = null;

        // Ugly hack
        if (args.Length >= 2)
        {
            if (args[1] == "-adapter")
            {
                if (args.Length >= 4)
                {
                    pipeName = args[3];
                }
            }
        }

        if (pipeName != null)
        {
            pipe = new Subprocess(pipeName);
            pipe.Read();

            var msg = pipe.DequeueMessage();
        }
    }
    #endregion

    #region Sending Data
    /// <summary>
    /// Sends the data when the game ends.
    /// </summary>
    public static void SendEndGameData()
    {
        if (IsntValid(Instance)) return;

        var xml = XmlUtil.GetXml(w => {

            var Wec = XmlUtil.MakeWecClosure(w);
            var Wes = XmlUtil.MakeWesClosure(w);

            // TODO Set which data should be sent
            /*
            Wec("activity", () => {
                Wes("plugin", "Kinect2");
                Wes("activity_id", "PhaseComplete");
                Wes("test_type", "game_AF");
                Wes("goodApples", goodApples.ToString());
                Wes("badApples", badApples.ToString());
                Wes("goodApplesMissed", goodApplesMissed.ToString());
                Wes("bestStreak", streak.ToString());
                Wes("score", score.ToString());
                Wec("difficulty", () => {
                    Wes("movementType", BasketMovement.GetMovementType());
                    Wes("movementDifficulty", BasketMovement.GetMovementDifficulty());
                    Wes("gameTime", currentTimer.ToString());
                });

            });*/
        });

        Instance.SendPhaseComplete(xml);
    }

    /// <summary>
    /// Sends the data for skipped gameplay. This is usually called when exiting through the pause menu.
    /// </summary>
    public static void SendSkippedXml()
    {
        if (IsntValid(Instance)) return;

        var plugin = BodySourceManager.IsAzureKinnect ? "AzureKinect" : "Kinect2";

        var xml = XmlUtil.GetXml(w => {

            var Wec = XmlUtil.MakeWecClosure(w);
            var Wes = XmlUtil.MakeWesClosure(w);

            Wec("activity", () => {
                Wes("plugin", "Kinect2");
                Wes("activity_id", "PhaseComplete");
                Wes("test_type", "Project Template");
                Wes("skipped", "true");
            });
        });

        Instance.SendPhaseComplete(xml);
    }

    /// <summary>
    /// Sends the xml file data to the balance applications and disposes of the pipe connection.
    /// </summary>
    /// <param name="xml">The string of game data that was created in an xml format.</param>
    private void SendPhaseComplete(string xml)
    {
        if (pipe != null)
        {
            if (!sentMessageToParent)
            {
                pipe.Write(System.Text.Encoding.UTF8.GetBytes(xml));
                sentMessageToParent = true;
            }

            //pipe.Update ();

            byte[] incomingMsg = null;
            while ((incomingMsg = pipe.DequeueMessage()) != null)
            {

            }

            print("Disposing IPC client...");

            try
            {
                pipe.Dispose();
                print("Disposed IPC client.");
            }
            catch (Exception e)
            {
                print(e.Message);
                print(e.StackTrace);
            }
        }

        print("Send Phase Complete");
        pipe = null;
    }
    #endregion
    #endregion
}
